using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Assertions;

// This file is a fork of the GeometryPool used by the GPU Driven Pipeline
// TODO: remove that file and use GeometryPool v2 (written in C++)

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    // Initial description of geometry pool, contains memory limits to hold cluster / index & vertex data.

    internal struct GeometryPoolDesc
    {
        public int vertexPoolByteSize;
        public int indexPoolByteSize;
        public int meshChunkTablesByteSize;

        public static GeometryPoolDesc NewDefault()
        {
            return new GeometryPoolDesc()
            {
                vertexPoolByteSize = 256 * 1024 * 1024, //256 mb
                indexPoolByteSize = 32 * 1024 * 1024, //32 mb
                meshChunkTablesByteSize = 4 * 1024 * 1024
            };
        }
    }

    // Handle to a piece of geo. Geometry meshes registered are ref counted.
    // Each handle allocation must be deallocated manually.
    internal struct GeometryPoolHandle : IEquatable<GeometryPoolHandle>
    {
        public int index;
        public static GeometryPoolHandle Invalid = new GeometryPoolHandle() { index = -1 };
        public bool valid => index != -1;
        public bool Equals(GeometryPoolHandle other) => index == other.index;
    }

    // Entry information of a geometry handle.
    // Use this helper to check validity, material hashes and active refcounts.
    internal struct GeometryPoolEntryInfo
    {
        public bool valid;
        public uint refCount;

        public static GeometryPoolEntryInfo NewDefault()
        {
            return new GeometryPoolEntryInfo()
            {
                valid = false,
                refCount = 0
            };
        }
    }

    // Descriptor of piece of geometry (the submesh information).
    internal struct GeometryPoolSubmeshData
    {
        public int submeshIndex;
        public Material material;
    }

    // Description of the geometry pool entry.
    // Contains master list and the submesh data information.
    internal struct GeometryPoolEntryDesc
    {
        public Mesh mesh;
        public GeometryPoolSubmeshData[] submeshData;
    }

    // Geometry pool container. Contains a global set of geometry accessible from the GPU.
    internal class GeometryPool : IDisposable
    {
        private const int kMaxThreadGroupsPerDispatch = 65535; // Counted in groups, not threads.
        private const int kThreadGroupSize = 256; // Counted in threads
        private static class GeoPoolShaderIDs
        {
            // MainUpdateIndexBuffer32 and MainUpdateIndexBuffer16 Kernel Strings
            public static readonly int _InputIBBaseOffset = Shader.PropertyToID("_InputIBBaseOffset");
            public static readonly int _DispatchIndexOffset = Shader.PropertyToID("_DispatchIndexOffset");
            public static readonly int _InputIBCount = Shader.PropertyToID("_InputIBCount");
            public static readonly int _OutputIBOffset = Shader.PropertyToID("_OutputIBOffset");
            public static readonly int _InputFirstVertex = Shader.PropertyToID("_InputFirstVertex");
            public static readonly int _InputIndexBuffer = Shader.PropertyToID("_InputIndexBuffer");
            public static readonly int _OutputIndexBuffer = Shader.PropertyToID("_OutputIndexBuffer");

            // MainUpdateVertexBuffer Kernel Strings
            public static readonly int _InputVBCount = Shader.PropertyToID("_InputVBCount");
            public static readonly int _InputBaseVertexOffset = Shader.PropertyToID("_InputBaseVertexOffset");
            public static readonly int _DispatchVertexOffset = Shader.PropertyToID("_DispatchVertexOffset");
            public static readonly int _OutputVBSize = Shader.PropertyToID("_OutputVBSize");
            public static readonly int _OutputVBOffset = Shader.PropertyToID("_OutputVBOffset");
            public static readonly int _InputPosBufferStride = Shader.PropertyToID("_InputPosBufferStride");
            public static readonly int _InputPosBufferOffset = Shader.PropertyToID("_InputPosBufferOffset");
            public static readonly int _InputUv0BufferStride = Shader.PropertyToID("_InputUv0BufferStride");
            public static readonly int _InputUv0BufferOffset = Shader.PropertyToID("_InputUv0BufferOffset");
            public static readonly int _InputUv1BufferStride = Shader.PropertyToID("_InputUv1BufferStride");
            public static readonly int _InputUv1BufferOffset = Shader.PropertyToID("_InputUv1BufferOffset");
            public static readonly int _InputNormalBufferStride = Shader.PropertyToID("_InputNormalBufferStride");
            public static readonly int _InputNormalBufferOffset = Shader.PropertyToID("_InputNormalBufferOffset");
            public static readonly int _InputTangentBufferStride = Shader.PropertyToID("_InputTangentBufferStride");
            public static readonly int _InputTangentBufferOffset = Shader.PropertyToID("_InputTangentBufferOffset");
            public static readonly int _PosBuffer = Shader.PropertyToID("_PosBuffer");
            public static readonly int _Uv0Buffer = Shader.PropertyToID("_Uv0Buffer");
            public static readonly int _Uv1Buffer = Shader.PropertyToID("_Uv1Buffer");
            public static readonly int _NormalBuffer = Shader.PropertyToID("_NormalBuffer");
            public static readonly int _TangentBuffer = Shader.PropertyToID("_TangentBuffer");
            public static readonly int _OutputVB = Shader.PropertyToID("_OutputVB");
            public static readonly int _AttributesMask = Shader.PropertyToID("_AttributesMask");
        }

        // Geometry slot represents a set of pointers to the blobs of vertex, index and cluster information
        private const int InvalidHandle = -1;

        public struct MeshChunk
        {
            public BlockAllocator.Allocation vertexAlloc;
            public BlockAllocator.Allocation indexAlloc;

            public GeoPoolMeshChunk EncodeGPUEntry()
            {
                return new GeoPoolMeshChunk() {
                    indexOffset = indexAlloc.block.offset,
                    indexCount = indexAlloc.block.count,
                    vertexOffset = vertexAlloc.block.offset,
                    vertexCount = vertexAlloc.block.count,
                };
            }

            public static MeshChunk Invalid => new MeshChunk()
            {
                vertexAlloc = BlockAllocator.Allocation.Invalid,
                indexAlloc = BlockAllocator.Allocation.Invalid
            };
        }

        public struct GeometrySlot
        {
            public uint refCount;
            public uint hash;
            public BlockAllocator.Allocation meshChunkTableAlloc;
            public NativeArray<MeshChunk> meshChunks;
            public bool hasGPUData;

            public static GeometrySlot Invalid = new GeometrySlot()
            {
                meshChunkTableAlloc = BlockAllocator.Allocation.Invalid,
                hasGPUData = false,
            };

            public bool valid => meshChunkTableAlloc.valid;
        }

        private struct GeoPoolEntrySlot
        {
            public uint refCount;
            public uint hash;
            public int geoSlotHandle;
            public static GeoPoolEntrySlot Invalid = new GeoPoolEntrySlot()
            {
                refCount = 0u,
                hash = 0u,
                geoSlotHandle = InvalidHandle,
            };

            public bool valid => geoSlotHandle != InvalidHandle;
        }
        private struct VertexBufferAttribInfo
        {
            public GraphicsBuffer buffer;
            public int stride;
            public int offset;
            public int byteCount;

            public bool valid => buffer != null;
        }

        public static int GetVertexByteSize() => GeometryPoolConstants.GeoPoolVertexByteSize;
        public static int GetIndexByteSize() => GeometryPoolConstants.GeoPoolIndexByteSize;
        public static int GetMeshChunkTableEntryByteSize() => System.Runtime.InteropServices.Marshal.SizeOf<GeoPoolMeshChunk>();

        private int GetFormatByteCount(VertexAttributeFormat format)
        {
            switch (format)
            {
                case VertexAttributeFormat.Float32: return 4;
                case VertexAttributeFormat.Float16: return 2;
                case VertexAttributeFormat.UNorm8: return 1;
                case VertexAttributeFormat.SNorm8: return 1;
                case VertexAttributeFormat.UNorm16: return 2;
                case VertexAttributeFormat.SNorm16: return 2;
                case VertexAttributeFormat.UInt8: return 1;
                case VertexAttributeFormat.SInt8: return 1;
                case VertexAttributeFormat.UInt16: return 2;
                case VertexAttributeFormat.SInt16: return 2;
                case VertexAttributeFormat.UInt32: return 4;
                case VertexAttributeFormat.SInt32: return 4;
            }
            return 4;
        }

        private static int DivUp(int x, int y) => (x + y - 1) / y;

        private const GraphicsBuffer.Target VertexBufferTarget = GraphicsBuffer.Target.Structured;
        private const GraphicsBuffer.Target IndexBufferTarget = GraphicsBuffer.Target.Structured;

        private int m_GeoPoolEntriesCount;
        public GraphicsBuffer globalIndexBuffer { get { return m_GlobalIndexBuffer; } }
        public GraphicsBuffer globalVertexBuffer { get { return m_GlobalVertexBuffer; } }
        public int globalVertexBufferStrideBytes { get { return GetVertexByteSize(); } }
        public GraphicsBuffer globalMeshChunkTableEntryBuffer { get { return m_GlobalMeshChunkTableEntryBuffer; } }

        public int indicesCount => m_MaxIndexCounts;
        public int verticesCount => m_MaxVertCounts;
        public int meshChunkTablesEntryCount => m_MaxMeshChunkTableEntriesCount;

        private GraphicsBuffer m_GlobalIndexBuffer = null;
        private GraphicsBuffer m_GlobalVertexBuffer = null;
        private GraphicsBuffer m_GlobalMeshChunkTableEntryBuffer = null;
        private GraphicsBuffer m_DummyBuffer = null;

        private int m_MaxVertCounts;
        private int m_MaxIndexCounts;
        private int m_MaxMeshChunkTableEntriesCount;

        private BlockAllocator m_VertexAllocator;
        private BlockAllocator m_IndexAllocator;
        private BlockAllocator m_MeshChunkTableAllocator;

        private NativeParallelHashMap<uint, int> m_MeshHashToGeoSlot;
        private List<GeometrySlot> m_GeoSlots;
        private NativeList<int> m_FreeGeoSlots;

        private NativeParallelHashMap<uint, GeometryPoolHandle> m_GeoPoolEntryHashToSlot;
        private NativeList<GeoPoolEntrySlot> m_GeoPoolEntrySlots;
        private NativeList<GeometryPoolHandle> m_FreeGeoPoolEntrySlots;

        private List<GraphicsBuffer> m_InputBufferReferences;

        private ComputeShader m_CopyShader;

        private ComputeShader m_GeometryPoolKernelsCS;
        private int m_KernelMainUpdateIndexBuffer16;
        private int m_KernelMainUpdateIndexBuffer32;
        private int m_KernelMainUpdateVertexBuffer;

        private CommandBuffer m_CmdBuffer;
        private bool m_MustClearCmdBuffer;
        private int m_PendingCmds;

        public GeometryPool(in GeometryPoolDesc desc, ComputeShader geometryPoolShader, ComputeShader copyShader)
        {
            m_CopyShader = copyShader;
            LoadKernels(geometryPoolShader);

            m_CmdBuffer = new CommandBuffer();
            m_InputBufferReferences = new List<GraphicsBuffer>();
            m_MustClearCmdBuffer = false;
            m_PendingCmds = 0;

            m_GeoPoolEntriesCount = 0;

            m_MaxVertCounts = CalcVertexCount(desc.vertexPoolByteSize);
            m_MaxIndexCounts = CalcIndexCount(desc.indexPoolByteSize);
            m_MaxMeshChunkTableEntriesCount = CalcMeshChunkTablesCount(desc.meshChunkTablesByteSize);

            m_GlobalVertexBuffer = new GraphicsBuffer(VertexBufferTarget, DivUp(m_MaxVertCounts * GetVertexByteSize(), 4), 4);
            m_GlobalIndexBuffer = new GraphicsBuffer(IndexBufferTarget, m_MaxIndexCounts, 4);
            m_GlobalMeshChunkTableEntryBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_MaxMeshChunkTableEntriesCount, GetMeshChunkTableEntryByteSize());
            m_DummyBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 16, 4);

            var initialCapacity = 4096;
            m_MeshHashToGeoSlot = new NativeParallelHashMap<uint, int>(initialCapacity, Allocator.Persistent);
            m_GeoSlots = new List<GeometrySlot>();
            m_FreeGeoSlots = new NativeList<int>(Allocator.Persistent);

            m_GeoPoolEntryHashToSlot = new NativeParallelHashMap<uint, GeometryPoolHandle>(initialCapacity, Allocator.Persistent);
            m_GeoPoolEntrySlots = new NativeList<GeoPoolEntrySlot>(Allocator.Persistent);
            m_FreeGeoPoolEntrySlots = new NativeList<GeometryPoolHandle>(Allocator.Persistent);

            m_VertexAllocator = new BlockAllocator();
            m_VertexAllocator.Initialize(m_MaxVertCounts);

            m_IndexAllocator = new BlockAllocator();
            m_IndexAllocator.Initialize(m_MaxIndexCounts);

            m_MeshChunkTableAllocator = new BlockAllocator();
            m_MeshChunkTableAllocator.Initialize(m_MaxMeshChunkTableEntriesCount);
        }

        void DisposeInputBuffers()
        {
            if (m_InputBufferReferences.Count == 0)
                return;

            foreach (var b in m_InputBufferReferences)
                b.Dispose();
            m_InputBufferReferences.Clear();
        }

        public void Dispose()
        {
            m_IndexAllocator.Dispose();
            m_VertexAllocator.Dispose();
            m_MeshChunkTableAllocator.Dispose();
            m_DummyBuffer.Dispose();

            m_MeshHashToGeoSlot.Dispose();
            foreach (var geoSlot in m_GeoSlots)
            {
                if (geoSlot.valid)
                    geoSlot.meshChunks.Dispose();
            }
            m_GeoSlots = null;

            m_FreeGeoSlots.Dispose();

            m_GeoPoolEntryHashToSlot.Dispose();
            m_GeoPoolEntrySlots.Dispose();
            m_FreeGeoPoolEntrySlots.Dispose();

            m_GlobalIndexBuffer.Dispose();
            m_GlobalVertexBuffer.Release();

            m_GlobalMeshChunkTableEntryBuffer.Dispose();

            m_CmdBuffer.Release();
            DisposeInputBuffers();
        }

        private void LoadKernels(ComputeShader geometryPoolShader)
        {
            m_GeometryPoolKernelsCS = geometryPoolShader;

            m_KernelMainUpdateIndexBuffer16 = m_GeometryPoolKernelsCS.FindKernel("MainUpdateIndexBuffer16");
            m_KernelMainUpdateIndexBuffer32 = m_GeometryPoolKernelsCS.FindKernel("MainUpdateIndexBuffer32");
            m_KernelMainUpdateVertexBuffer = m_GeometryPoolKernelsCS.FindKernel("MainUpdateVertexBuffer");
        }

        private int CalcVertexCount(int bufferByteSize) => DivUp(bufferByteSize, GetVertexByteSize());
        private int CalcIndexCount(int bufferByteSize) => DivUp(bufferByteSize, GetIndexByteSize());
        private int CalcMeshChunkTablesCount(int bufferByteSize) => DivUp(bufferByteSize, GetMeshChunkTableEntryByteSize());

        private void DeallocateGeometrySlot(ref GeometrySlot slot)
        {
            if (slot.meshChunkTableAlloc.valid)
            {
                m_MeshChunkTableAllocator.FreeAllocation(slot.meshChunkTableAlloc);
                if (slot.meshChunks.IsCreated)
                {
                    for (int i = 0; i < slot.meshChunks.Length; ++i)
                    {
                        var meshChunk = slot.meshChunks[i];
                        if (meshChunk.vertexAlloc.valid)
                            m_VertexAllocator.FreeAllocation(meshChunk.vertexAlloc);

                        if (meshChunk.indexAlloc.valid)
                            m_IndexAllocator.FreeAllocation(meshChunk.indexAlloc);
                    }
                    slot.meshChunks.Dispose();
                }
            }

            slot = GeometrySlot.Invalid;
        }

        private void DeallocateGeometrySlot(int geoSlotHandle)
        {
            var geoSlot = m_GeoSlots[geoSlotHandle];
            Assertions.Assert.IsTrue(geoSlot.valid);
            --geoSlot.refCount;
            if (geoSlot.refCount == 0)
            {
                m_MeshHashToGeoSlot.Remove(geoSlot.hash);
                DeallocateGeometrySlot(ref geoSlot);
                m_FreeGeoSlots.Add(geoSlotHandle);
            }
            m_GeoSlots[geoSlotHandle] = geoSlot;
        }

        private bool AllocateGeo(Mesh mesh, out int allocationHandle)
        {
            uint meshHash = (uint)mesh.GetHashCode();
            int vertexCount = mesh.vertexCount;

            int indexCount = 0;
            for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; ++submeshIndex)
            {
                indexCount += (int)mesh.GetIndexCount(submeshIndex);
            }

            if (m_MeshHashToGeoSlot.TryGetValue(meshHash, out allocationHandle))
            {
                var geoSlot = m_GeoSlots[allocationHandle];
                Assertions.Assert.IsTrue(geoSlot.hash == meshHash);
                Assertions.Assert.IsTrue(geoSlot.meshChunkTableAlloc.block.count == mesh.subMeshCount);

                ++geoSlot.refCount;
                m_GeoSlots[allocationHandle] = geoSlot;
                return true;
            }

            allocationHandle = InvalidHandle;
            var newSlot = GeometrySlot.Invalid;
            newSlot.refCount = 1;
            newSlot.hash = meshHash;

            bool allocationSuccess = true;

            if (mesh.subMeshCount > 0)
            {
                newSlot.meshChunkTableAlloc = m_MeshChunkTableAllocator.Allocate(mesh.subMeshCount);
                if (!newSlot.meshChunkTableAlloc.valid)
                {
                    var oldChunkCount = m_MeshChunkTableAllocator.capacity;
                    var newChunkCount = m_MeshChunkTableAllocator.Grow(m_MeshChunkTableAllocator.capacity + mesh.subMeshCount);
                    var newChunkTableBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, newChunkCount, GetMeshChunkTableEntryByteSize());

                    GraphicsHelpers.CopyBuffer(m_CopyShader, m_GlobalMeshChunkTableEntryBuffer, 0, newChunkTableBuffer, 0, DivUp(oldChunkCount * GetMeshChunkTableEntryByteSize(), 4));

                    m_GlobalMeshChunkTableEntryBuffer.Dispose();
                    m_GlobalMeshChunkTableEntryBuffer = newChunkTableBuffer;
                    m_MaxMeshChunkTableEntriesCount = newChunkCount;
                    newSlot.meshChunkTableAlloc = m_MeshChunkTableAllocator.Allocate(mesh.subMeshCount);
                    Assert.IsTrue(newSlot.meshChunkTableAlloc.valid);
                }

                newSlot.meshChunks = new NativeArray<MeshChunk>(mesh.subMeshCount, Allocator.Persistent);
                for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; ++submeshIndex)
                {
                    SubMeshDescriptor submeshDescriptor = mesh.GetSubMesh(submeshIndex);
                    var newMeshChunk = MeshChunk.Invalid;

                    newMeshChunk.vertexAlloc = m_VertexAllocator.Allocate(submeshDescriptor.vertexCount);
                    if (!newMeshChunk.vertexAlloc.valid)
                    {
                        var oldVertexCount = m_VertexAllocator.capacity;
                        var newVertexCount = m_VertexAllocator.Grow(m_VertexAllocator.capacity + submeshDescriptor.vertexCount);
                        var newVertexBuffer = new GraphicsBuffer(VertexBufferTarget, DivUp(newVertexCount * GetVertexByteSize(), 4), 4);

                        GraphicsHelpers.CopyBuffer(m_CopyShader, m_GlobalVertexBuffer, 0, newVertexBuffer, 0, DivUp(oldVertexCount * GetVertexByteSize(), 4));

                        m_GlobalVertexBuffer.Dispose();
                        m_GlobalVertexBuffer = newVertexBuffer;
                        m_MaxVertCounts = newVertexCount;
                        newMeshChunk.vertexAlloc = m_VertexAllocator.Allocate(submeshDescriptor.vertexCount);
                        Assert.IsTrue(newMeshChunk.vertexAlloc.valid);
                    }

                    newMeshChunk.indexAlloc = m_IndexAllocator.Allocate(submeshDescriptor.indexCount);
                    if (!newMeshChunk.indexAlloc.valid)
                    {
                        var oldIndexcount = m_IndexAllocator.capacity;
                        var newIndexCount = m_IndexAllocator.Grow(m_IndexAllocator.capacity + submeshDescriptor.indexCount);
                        var newIndexBuffer = new GraphicsBuffer(IndexBufferTarget, newIndexCount, 4);

                        GraphicsHelpers.CopyBuffer(m_CopyShader, m_GlobalIndexBuffer, 0, newIndexBuffer, 0, oldIndexcount);

                        m_GlobalIndexBuffer.Dispose();
                        m_GlobalIndexBuffer = newIndexBuffer;
                        m_MaxIndexCounts = newIndexCount;
                        newMeshChunk.indexAlloc = m_IndexAllocator.Allocate(submeshDescriptor.indexCount);
                        Assert.IsTrue(newMeshChunk.indexAlloc.valid);
                    }

                    newSlot.meshChunks[submeshIndex] = newMeshChunk;
                }
            }

            if (!allocationSuccess)
            {
                DeallocateGeometrySlot(ref newSlot);
                return false;
            }

            if (m_FreeGeoSlots.IsEmpty)
            {
                allocationHandle = m_GeoSlots.Count;
                m_GeoSlots.Add(newSlot);
            }
            else
            {
                allocationHandle = m_FreeGeoSlots[m_FreeGeoSlots.Length - 1];
                m_FreeGeoSlots.RemoveAtSwapBack(m_FreeGeoSlots.Length - 1);
                Assertions.Assert.IsTrue(!m_GeoSlots[allocationHandle].valid);
                m_GeoSlots[allocationHandle] = newSlot;
            }

            m_MeshHashToGeoSlot.Add(newSlot.hash, allocationHandle);

            return true;
        }

        private void DeallocateGeoPoolEntrySlot(GeometryPoolHandle handle)
        {
            var slot = m_GeoPoolEntrySlots[handle.index];
            --slot.refCount;
            if (slot.refCount == 0)
            {
                m_GeoPoolEntryHashToSlot.Remove(slot.hash);
                DeallocateGeoPoolEntrySlot(ref slot);
                m_FreeGeoPoolEntrySlots.Add(handle);
            }
            m_GeoPoolEntrySlots[handle.index] = slot;
        }

        private void DeallocateGeoPoolEntrySlot(ref GeoPoolEntrySlot geoPoolEntrySlot)
        {
            if (geoPoolEntrySlot.geoSlotHandle != InvalidHandle)
                DeallocateGeometrySlot(geoPoolEntrySlot.geoSlotHandle);

            geoPoolEntrySlot = GeoPoolEntrySlot.Invalid;
        }

        public GeometryPoolEntryInfo GetEntryInfo(GeometryPoolHandle handle)
        {
            if (!handle.valid)
                return GeometryPoolEntryInfo.NewDefault();

            GeoPoolEntrySlot slot = m_GeoPoolEntrySlots[handle.index];
            if (!slot.valid)
                return GeometryPoolEntryInfo.NewDefault();

            if (slot.geoSlotHandle == -1)
                Debug.LogErrorFormat("Found invalid geometry slot handle with handle id {0}.", handle.index);
            return new GeometryPoolEntryInfo()
            {
                valid = slot.valid,
                refCount = slot.refCount
            };
        }
        public GeometrySlot GetEntryGeomAllocation(GeometryPoolHandle handle)
        {
            var slot = m_GeoPoolEntrySlots[handle.index];
            Assertions.Assert.IsTrue(slot.valid);

            var geoSlot = m_GeoSlots[slot.geoSlotHandle];
            Assertions.Assert.IsTrue(geoSlot.valid);

            return geoSlot;
        }

        private void UpdateGeoGpuState(Mesh mesh, List<GeometryPoolSubmeshData> submeshData, GeometryPoolHandle handle)
        {
            var entrySlot = m_GeoPoolEntrySlots[handle.index];
            var geoSlot = m_GeoSlots[entrySlot.geoSlotHandle];

            CommandBuffer cmdBuffer = AllocateCommandBuffer(); //clear any previous cmd buffers.

            //Upload mesh information.
            if (!geoSlot.hasGPUData)
            {
                //Load index buffer
                GraphicsBuffer buffer = LoadIndexBuffer(cmdBuffer, mesh);
                Assertions.Assert.IsTrue((buffer.target & GraphicsBuffer.Target.Raw) != 0);

                // Load attribute buffers
                var posAttrib = new VertexBufferAttribInfo();
                LoadVertexAttribInfo(mesh, VertexAttribute.Position, out posAttrib);

                var uv0Attrib = new VertexBufferAttribInfo();
                LoadVertexAttribInfo(mesh, VertexAttribute.TexCoord0, out uv0Attrib);

                var uv1Attrib = new VertexBufferAttribInfo();
                LoadVertexAttribInfo(mesh, VertexAttribute.TexCoord1, out uv1Attrib);

                var normalAttrib = new VertexBufferAttribInfo();
                LoadVertexAttribInfo(mesh, VertexAttribute.Normal, out normalAttrib);

                var meshChunkAllocationTable = new NativeArray<GeoPoolMeshChunk>(geoSlot.meshChunks.Length, Allocator.Temp);
                for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; ++submeshIndex)
                {
                    SubMeshDescriptor submeshDescriptor = mesh.GetSubMesh(submeshIndex);
                    MeshChunk targetMeshChunk = geoSlot.meshChunks[submeshIndex];
                    //Update mesh chunk vertex offset
                    AddVertexUpdateCommand(
                        cmdBuffer, submeshDescriptor.baseVertex + submeshDescriptor.firstVertex,
                        posAttrib, uv0Attrib, uv1Attrib, normalAttrib,
                        targetMeshChunk.vertexAlloc, m_GlobalVertexBuffer);

                    //Update mesh chunk index offset
                    AddIndexUpdateCommand(
                        cmdBuffer,
                        mesh.indexFormat, buffer, targetMeshChunk.indexAlloc, submeshDescriptor.firstVertex,
                        submeshDescriptor.indexStart, submeshDescriptor.indexCount, 0,
                        m_GlobalIndexBuffer);

                    meshChunkAllocationTable[submeshIndex] = targetMeshChunk.EncodeGPUEntry();
                }

                cmdBuffer.SetBufferData(m_GlobalMeshChunkTableEntryBuffer, meshChunkAllocationTable, 0, geoSlot.meshChunkTableAlloc.block.offset, meshChunkAllocationTable.Length);
                meshChunkAllocationTable.Dispose();

                geoSlot.hasGPUData = true;
                m_GeoSlots[entrySlot.geoSlotHandle] = geoSlot;
            }
        }

        private uint FNVHash(uint prevHash, uint dword)
        {
            //https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
            const uint fnvPrime = 0x811C9DC5;
            for (int i = 0; i < 4; ++i)
            {
                prevHash ^= ((dword >> (i * 8)) & 0xFF);
                prevHash *= fnvPrime;
            }
            return prevHash;
        }

        private uint CalculateClusterHash(uint meshHash, GeometryPoolSubmeshData[] submeshData)
        {
            uint clusterHash = meshHash;
            if (submeshData != null)
            {
                foreach (var data in submeshData)
                {
                    clusterHash = FNVHash(clusterHash, (uint)data.submeshIndex);
                    clusterHash = FNVHash(clusterHash, (uint)(data.material == null ? 0 : data.material.GetHashCode()));
                }
            }
            return clusterHash;
        }

        public bool Register(Mesh mesh, out GeometryPoolHandle outHandle)
        {
            return Register(new GeometryPoolEntryDesc()
            {
                mesh = mesh,
                submeshData = null
            }, out outHandle);
        }

        private static int FindSubmeshEntryInDesc(int submeshIndex, in GeometryPoolSubmeshData[] submeshData)
        {
            if (submeshData == null)
                return -1;

            for (int i = 0; i < submeshData.Length; ++i)
            {
                if (submeshData[i].submeshIndex == submeshIndex)
                    return i;
            }

            return -1;
        }

        public bool Register(in GeometryPoolEntryDesc entryDesc, out GeometryPoolHandle outHandle)
        {
            outHandle = GeometryPoolHandle.Invalid;
            if (entryDesc.mesh == null)
            {
                return false;
            }

            Mesh mesh = entryDesc.mesh;
            uint meshHashCode = (uint)entryDesc.mesh.GetHashCode();
            uint clusterListHashCode = CalculateClusterHash(meshHashCode, entryDesc.submeshData);
            uint geoPoolEntryHash = clusterListHashCode;

            if (m_GeoPoolEntryHashToSlot.TryGetValue(geoPoolEntryHash, out outHandle))
            {
                GeoPoolEntrySlot geoPoolEntrySlot = m_GeoPoolEntrySlots[outHandle.index];
                Assertions.Assert.IsTrue(geoPoolEntrySlot.hash == geoPoolEntryHash);

                GeometrySlot geoSlot = m_GeoSlots[geoPoolEntrySlot.geoSlotHandle];
                Assertions.Assert.IsTrue(geoSlot.hash == meshHashCode);

                ++geoPoolEntrySlot.refCount;
                m_GeoPoolEntrySlots[outHandle.index] = geoPoolEntrySlot;
                return true;
            }

            var newSlot = GeoPoolEntrySlot.Invalid;
            newSlot.refCount = 1;
            newSlot.hash = geoPoolEntryHash;

            // Validate submesh information
            var validSubmeshData = new List<GeometryPoolSubmeshData>(mesh.subMeshCount);
            if (mesh.subMeshCount > 0 && entryDesc.submeshData != null)
            {
                for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; ++submeshIndex)
                {
                    int entryIndex = FindSubmeshEntryInDesc(submeshIndex, entryDesc.submeshData);
                    if (entryIndex == -1)
                    {
                        Debug.LogErrorFormat("Could not find submesh index {0} for mesh entry descriptor of mesh {1}.", submeshIndex, mesh.name);
                        continue;
                    }
                    validSubmeshData.Add(entryDesc.submeshData[entryIndex]);
                }
            }

            if (!AllocateGeo(mesh, out newSlot.geoSlotHandle))
            {
                DeallocateGeoPoolEntrySlot(ref newSlot);
                return false;
            }


            if (m_FreeGeoPoolEntrySlots.IsEmpty)
            {
                outHandle = new GeometryPoolHandle() { index = m_GeoPoolEntrySlots.Length };
                m_GeoPoolEntrySlots.Add(newSlot);
            }
            else
            {
                outHandle = m_FreeGeoPoolEntrySlots[m_FreeGeoPoolEntrySlots.Length - 1];
                m_FreeGeoPoolEntrySlots.RemoveAtSwapBack(m_FreeGeoPoolEntrySlots.Length - 1);
                Assertions.Assert.IsTrue(!m_GeoPoolEntrySlots[outHandle.index].valid);
                m_GeoPoolEntrySlots[outHandle.index] = newSlot;
            }

            m_GeoPoolEntryHashToSlot.Add(newSlot.hash, outHandle);
            UpdateGeoGpuState(mesh, validSubmeshData, outHandle);

            ++m_GeoPoolEntriesCount;
            return true;
        }

        public void Unregister(GeometryPoolHandle handle)
        {
            var slot = m_GeoPoolEntrySlots[handle.index];
            Assertions.Assert.IsTrue(slot.valid);
            DeallocateGeoPoolEntrySlot(handle);
            --m_GeoPoolEntriesCount;
        }

        public void SendGpuCommands()
        {
            if (m_PendingCmds != 0)
            {
                Graphics.ExecuteCommandBuffer(m_CmdBuffer);
                m_MustClearCmdBuffer = true;
                m_PendingCmds = 0;
            }

            DisposeInputBuffers();
        }

        private GraphicsBuffer LoadIndexBuffer(CommandBuffer cmdBuffer, Mesh mesh)
        {
            if ((mesh.indexBufferTarget & GraphicsBuffer.Target.Raw) == 0 && (mesh.GetIndices(0) == null || mesh.GetIndices(0).Length == 0))
            {
                throw new Exception("Cant use a mesh buffer that is not raw and has no CPU index information.");
            }
            else
            {
                mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
                mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
                var idxBuffer = mesh.GetIndexBuffer();
                m_InputBufferReferences.Add(idxBuffer);
                return idxBuffer;
            }
        }

        void LoadVertexAttribInfo(Mesh mesh, VertexAttribute attribute, out VertexBufferAttribInfo output)
        {
            if (!mesh.HasVertexAttribute(attribute))
            {
                output.buffer = null;
                output.stride = output.offset = output.byteCount = 0;
                return;
            }

            int stream = mesh.GetVertexAttributeStream(attribute);

            output.stride = mesh.GetVertexBufferStride(stream);
            output.offset = mesh.GetVertexAttributeOffset(attribute);
            output.byteCount = GetFormatByteCount(mesh.GetVertexAttributeFormat(attribute)) * mesh.GetVertexAttributeDimension(attribute);

            output.buffer = mesh.GetVertexBuffer(stream);
            m_InputBufferReferences.Add(output.buffer);

            Assertions.Assert.IsTrue((output.buffer.target & GraphicsBuffer.Target.Raw) != 0);
        }

        private CommandBuffer AllocateCommandBuffer()
        {
            if (m_MustClearCmdBuffer)
            {
                m_CmdBuffer.Clear();
                m_MustClearCmdBuffer = false;
            }

            ++m_PendingCmds;
            return m_CmdBuffer;
        }

        private void AddIndexUpdateCommand(
            CommandBuffer cmdBuffer,
            IndexFormat inputFormat,
            in GraphicsBuffer inputBuffer,
            in BlockAllocator.Allocation location,
            int firstVertex,
            int inputOffset, int indexCount, int outputOffset,
            GraphicsBuffer outputIdxBuffer)
        {
            if (location.block.count == 0)
                return;

            Assertions.Assert.IsTrue(indexCount <= location.block.count);
            Assertions.Assert.IsTrue(outputOffset < location.block.count);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputIBBaseOffset, inputOffset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputIBCount, indexCount);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputFirstVertex, firstVertex);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._OutputIBOffset, location.block.offset + outputOffset);
            int kernel = inputFormat == IndexFormat.UInt16 ? m_KernelMainUpdateIndexBuffer16 : m_KernelMainUpdateIndexBuffer32;
            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._InputIndexBuffer, inputBuffer);
            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._OutputIndexBuffer, outputIdxBuffer);
            for (int uvChannel = 0; uvChannel < 2; uvChannel++)
            {
                cmdBuffer.EnableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM2"));
                cmdBuffer.DisableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM3"));
                cmdBuffer.DisableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM4"));
            }

            int totalGroupCount = DivUp(location.block.count, kThreadGroupSize);
            int dispatchCount = DivUp(totalGroupCount, kMaxThreadGroupsPerDispatch);

            for (int dispatchIndex = 0; dispatchIndex < dispatchCount; ++dispatchIndex)
            {
                int indexOffset = dispatchIndex * kMaxThreadGroupsPerDispatch * kThreadGroupSize;
                int dispatchGroupCount = Math.Min(kMaxThreadGroupsPerDispatch, totalGroupCount - dispatchIndex * kMaxThreadGroupsPerDispatch);

                cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._DispatchIndexOffset, indexOffset);
                cmdBuffer.DispatchCompute(m_GeometryPoolKernelsCS, kernel, dispatchGroupCount, 1, 1);
            }
        }

        private void AddVertexUpdateCommand(
            CommandBuffer cmdBuffer, int baseVertexOffset,
            in VertexBufferAttribInfo pos, in VertexBufferAttribInfo uv0, in VertexBufferAttribInfo uv1, in VertexBufferAttribInfo n,
            in BlockAllocator.Allocation location,
            GraphicsBuffer outputVertexBuffer)
        {
            if (location.block.count == 0)
                return;

            GeoPoolVertexAttribs attributes = 0;
            if (pos.valid)
                attributes |= GeoPoolVertexAttribs.Position;

            if (uv0.valid)
                attributes |= GeoPoolVertexAttribs.Uv0;

            if (uv1.valid)
                attributes |= GeoPoolVertexAttribs.Uv1;

            if (n.valid)
                attributes |= GeoPoolVertexAttribs.Normal;

            int vertexCount = location.block.count;

            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputVBCount, vertexCount);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputBaseVertexOffset, baseVertexOffset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._OutputVBSize, m_MaxVertCounts);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._OutputVBOffset, location.block.offset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputPosBufferStride, pos.stride);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputPosBufferOffset, pos.offset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputUv0BufferStride, uv0.stride);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputUv0BufferOffset, uv0.offset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputUv1BufferStride, uv1.stride);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputUv1BufferOffset, uv1.offset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputNormalBufferStride, n.stride);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._InputNormalBufferOffset, n.offset);
            cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._AttributesMask, (int)attributes);

            int kernel = m_KernelMainUpdateVertexBuffer;
            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._PosBuffer, pos.valid ? pos.buffer : m_DummyBuffer);
            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._Uv0Buffer, uv0.valid ? uv0.buffer : m_DummyBuffer);
            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._Uv1Buffer, uv1.valid ? uv1.buffer : m_DummyBuffer);
            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._NormalBuffer, n.valid ? n.buffer : m_DummyBuffer);

            cmdBuffer.SetComputeBufferParam(m_GeometryPoolKernelsCS, kernel, GeoPoolShaderIDs._OutputVB, outputVertexBuffer);


            for (int uvChannel = 0; uvChannel < 2; uvChannel++)
            {
                cmdBuffer.DisableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM2"));
                cmdBuffer.DisableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM3"));
                cmdBuffer.DisableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM4"));
                ref readonly VertexBufferAttribInfo uv = ref (uvChannel == 0) ? ref uv0 : ref uv1;
                switch (uv.byteCount / 4)
                {
                    case 0:
                    // if there are no UVs, we still have to enabe a UV_DIM* keyword, but because of the _AttributesMask no UVs will be processed. This way we save a shader variant.
                    case 2:
                        cmdBuffer.EnableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM2"));
                        break;
                    case 3:
                        cmdBuffer.EnableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM3"));
                        break;
                    case 4:
                        cmdBuffer.EnableKeyword(m_GeometryPoolKernelsCS, new LocalKeyword(m_GeometryPoolKernelsCS, $"UV{uvChannel}_DIM4"));
                        break;
                }
            }

            int totalGroupCount = DivUp(vertexCount, kThreadGroupSize);
            int dispatchCount = DivUp(totalGroupCount, kMaxThreadGroupsPerDispatch);

            for (int dispatchIndex = 0; dispatchIndex < dispatchCount; ++dispatchIndex)
            {
                int vertexOffset = dispatchIndex * kMaxThreadGroupsPerDispatch * kThreadGroupSize;
                int dispatchGroupCount = Math.Min(kMaxThreadGroupsPerDispatch, totalGroupCount - dispatchIndex * kMaxThreadGroupsPerDispatch);

                cmdBuffer.SetComputeIntParam(m_GeometryPoolKernelsCS, GeoPoolShaderIDs._DispatchVertexOffset, vertexOffset);
                cmdBuffer.DispatchCompute(m_GeometryPoolKernelsCS, kernel, dispatchGroupCount, 1, 1);
            }
        }
    }
}
