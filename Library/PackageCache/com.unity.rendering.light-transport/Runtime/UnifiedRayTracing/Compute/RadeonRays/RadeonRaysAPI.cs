using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
    internal struct MeshBuildInfo
    {
        public GraphicsBuffer vertices;
        public int verticesStartOffset; // in DWORD
        public uint vertexCount;
        public uint vertexStride; // in DWORD

        public GraphicsBuffer triangleIndices;
        public int indicesStartOffset; // in DWORD
        public uint triangleCount;
    }

    internal struct MeshBuildMemoryRequirements
    {
        public ulong buildScratchSizeInDwords;
        public ulong resultSizeInDwords;
    }

    internal struct SceneBuildMemoryRequirements
    {
        public ulong buildScratchSizeInDwords;
    }

    internal class SceneMemoryRequirements
    {
        public ulong buildScratchSizeInDwords;
        public ulong[] bottomLevelBvhSizeInDwords;
        public uint[] bottomLevelBvhOffsetInNodes;
        public ulong totalBottomLevelBvhSizeInDwords;
    }

    [System.Flags]
    internal enum BuildFlags
    {
        None = 0,
        PreferFastBuild = 1 << 0,
        MinimizeMemory = 1 << 1
    }

    internal enum RayQueryType
    {
        ClosestHit,
        AnyHit
    }

    internal enum RayQueryOutputType
    {
        FullHitData,
        InstanceID
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Transform
    {
        public float4 row0;
        public float4 row1;
        public float4 row2;


        public Transform(float4 row0, float4 row1, float4 row2)
        {
            this.row0 = row0;
            this.row1 = row1;
            this.row2 = row2;
        }

        public static Transform Identity()
        {
            return new Transform(
                new float4(1.0f, 0.0f, 0.0f, 0.0f),
                new float4(0.0f, 1.0f, 0.0f, 0.0f),
                new float4(0.0f, 0.0f, 1.0f, 0.0f));
        }

        public static Transform Translation(float3 translation)
        {
            return new Transform(
                new float4(1.0f, 0.0f, 0.0f, translation.x),
                new float4(0.0f, 1.0f, 0.0f, translation.y),
                new float4(0.0f, 0.0f, 1.0f, translation.z));
        }

        public static Transform Scale(float3 scale)
        {
            return new Transform(
                new float4(scale.x, 0.0f, 0.0f, 0.0f),
                new float4(0.0f, scale.y, 0.0f, 0.0f),
                new float4(0.0f, 0.0f, scale.z, 0.0f));
        }

        public static Transform TRS(float3 translation, float3 rotation, float3 scale)
        {
            var rot = float3x3.Euler(rotation);
            rot.c0 *= scale.x;
            rot.c1 *= scale.y;
            rot.c2 *= scale.z;

            return new Transform(
                    new float4(rot.c0.x, rot.c1.x, rot.c2.x, translation.x),
                    new float4(rot.c0.y, rot.c1.y, rot.c2.y, translation.y),
                    new float4(rot.c0.z, rot.c1.z, rot.c2.z, translation.z));
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BvhNode
    {
        public uint child0;
        public uint child1;
        public uint parent;
        public uint update;

        public uint3 aabb0_min;
        public uint3 aabb0_max;
        public uint3 aabb1_min;
        public uint3 aabb1_max;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BvhHeader
    {
        public uint totalNodeCount;
        public uint leafNodeCount;
        public uint root;

        public uint unused;
        public uint3 unused1;
        public uint3 unused2;
        public uint3 unused3;
        public uint3 unused4;
    }

    internal struct Instance
    {
        public uint meshAccelStructOffset;
        public uint instanceMask;
        public uint vertexOffset;
        public uint indexOffset;
        public bool triangleCullingEnabled;
        public bool invertTriangleCulling;
        public uint userInstanceID;
        public Transform localToWorldTransform;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct InstanceInfo
    {
        public int blasOffset;
        public int instanceMask;
        public int vertexOffset;
        public int indexOffset;
        public int triangleCullingEnabled;
        public int invertTriangleCulling;
        public uint userInstanceID;
        public int padding2;
        public Transform worldToLocalTransform;
        public Transform localToWorldTransform;
    }

    internal class RadeonRaysShaders
    {
        public ComputeShader bitHistogram;
        public ComputeShader blockReducePart;
        public ComputeShader blockScan;
        public ComputeShader buildHlbvh;
        public ComputeShader reorderTriangleIndices;
        public ComputeShader restructureBvh;
        public ComputeShader scatter;
        public ComputeShader topLevelIntersector;
        public ComputeShader intersector;

#if UNITY_EDITOR
        public static RadeonRaysShaders LoadFromPath(string kernelFolderPath)
        {
            var res = new RadeonRaysShaders();

            res.bitHistogram =           UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "bit_histogram.compute"));
            res.blockReducePart =        UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "block_reduce_part.compute"));
            res.blockScan =              UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "block_scan.compute"));
            res.buildHlbvh =             UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "build_hlbvh.compute"));
            res.reorderTriangleIndices = UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "reorder_triangle_indices.compute"));
            res.restructureBvh =         UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "restructure_bvh.compute"));
            res.scatter =                UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "scatter.compute"));
            res.topLevelIntersector =    UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "top_level_intersector.compute"));
            res.intersector =            UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(Path.Combine(kernelFolderPath, "intersector.compute"));

            return res;
        }
#endif
    }

    internal class RadeonRaysAPI : IDisposable
    {
        private HlbvhBuilder buildBvh;
        private HlbvhTopLevelBuilder buildTopLevelBvh;
        private RestructureBvh restructureBvh;
        private TraceGeometry traceGeometry;
        private TraceScene traceScene;

        public const GraphicsBuffer.Target BufferTarget = GraphicsBuffer.Target.Structured;

        public RadeonRaysAPI(RadeonRaysShaders shaders)
        {
            buildBvh = new HlbvhBuilder(shaders);
            buildTopLevelBvh = new HlbvhTopLevelBuilder(shaders);
            restructureBvh = new RestructureBvh(shaders);
            traceGeometry = new TraceGeometry(shaders);
            traceScene = new TraceScene(shaders);
        }
        public void Dispose()
        {
            restructureBvh.Dispose();
        }

        static public int BvhNodeSizeInDwords() { return Marshal.SizeOf<BvhNode>() / 4; }
        static public int BvhNodeSizeInBytes() { return Marshal.SizeOf<BvhNode>(); }

        public void BuildMeshAccelStruct(
            CommandBuffer cmd,
            MeshBuildInfo buildInfo, BuildFlags buildFlags,
            GraphicsBuffer scratchBuffer,
            GraphicsBuffer accelStructBuffer, uint accelStructOffsetInNodes, uint accelStructSizeInNodes,
            Action<AsyncGPUReadbackRequest> callback = null)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                buildFlags |= BuildFlags.PreferFastBuild;

            buildBvh.Execute(cmd,
                buildInfo.vertices, buildInfo.verticesStartOffset, buildInfo.vertexStride,
                buildInfo.triangleIndices, buildInfo.indicesStartOffset, buildInfo.triangleCount,
                scratchBuffer,
                accelStructBuffer, accelStructOffsetInNodes, accelStructSizeInNodes,
                (buildFlags & BuildFlags.MinimizeMemory) != 0 ? 2u : 0);

            if (callback != null && (buildFlags & BuildFlags.MinimizeMemory) != 0)
                cmd.RequestAsyncReadback(accelStructBuffer, 4*sizeof(uint), (int)accelStructOffsetInNodes*Marshal.SizeOf<BvhNode>(), callback);

            if ((buildFlags & BuildFlags.PreferFastBuild) == 0)
            {
                restructureBvh.Execute(cmd,
                    buildInfo.vertices, buildInfo.verticesStartOffset, buildInfo.vertexStride,
                    buildInfo.triangleIndices, buildInfo.indicesStartOffset, buildInfo.triangleCount,
                    scratchBuffer, accelStructBuffer, accelStructOffsetInNodes);
            }
        }

        public MeshBuildMemoryRequirements GetMeshBuildMemoryRequirements(MeshBuildInfo buildInfo, BuildFlags buildFlags)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                buildFlags |= BuildFlags.PreferFastBuild;

            var result = new MeshBuildMemoryRequirements();
            if ((buildFlags & BuildFlags.MinimizeMemory) != 0)
                result.resultSizeInDwords = buildBvh.GetResultDataSizeInDwordsPrediction(buildInfo.triangleCount);
            else
                result.resultSizeInDwords = buildBvh.GetResultDataSizeInDwords(buildInfo.triangleCount);

            result.buildScratchSizeInDwords = buildBvh.GetScratchDataSizeInDwords(buildInfo.triangleCount);

            ulong restructureScratchSize = ((buildFlags & BuildFlags.PreferFastBuild) == 0) ? restructureBvh.GetScratchDataSizeInDwords(buildInfo.triangleCount) : 0;
            result.buildScratchSizeInDwords = math.max(result.buildScratchSizeInDwords, restructureScratchSize);

            return result;
        }

        public TopLevelAccelStruct BuildSceneAccelStruct(
                CommandBuffer cmd,
                GraphicsBuffer meshAccelStructsBuffer,
                Instance[] instances, BuildFlags buildFlags,
                GraphicsBuffer scratchBuffer)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                buildFlags |= BuildFlags.PreferFastBuild;

            var accelStruct = new TopLevelAccelStruct();

            if (instances.Length == 0)
            {
                buildTopLevelBvh.CreateEmpty(ref accelStruct);
                return accelStruct;
            }

            buildTopLevelBvh.AllocateResultBuffers((uint)instances.Length, ref accelStruct);

            var instancesInfos = new InstanceInfo[instances.Length];
            for (uint i = 0; i < instances.Length; ++i)
            {
                instancesInfos[i] = new InstanceInfo
                {
                    blasOffset = (int)instances[i].meshAccelStructOffset,
                    instanceMask = (int)instances[i].instanceMask,
                    vertexOffset = (int)instances[i].vertexOffset,
                    indexOffset = (int)instances[i].indexOffset,
                    localToWorldTransform = instances[i].localToWorldTransform,
                    triangleCullingEnabled = instances[i].triangleCullingEnabled ? 1 : 0,
                    invertTriangleCulling = instances[i].invertTriangleCulling ? 1 : 0,
                    userInstanceID = instances[i].userInstanceID
                    // worldToLocal computed in the shader
                };
            }
            accelStruct.instanceInfos.SetData(instancesInfos);
            accelStruct.bottomLevelBvhs = meshAccelStructsBuffer;
            accelStruct.instanceCount = (uint)instances.Length;

            buildTopLevelBvh.Execute(cmd, scratchBuffer, ref accelStruct);

            return accelStruct;
        }

        public SceneBuildMemoryRequirements GetSceneBuildMemoryRequirements(uint instanceCount)
        {
            var result = new SceneBuildMemoryRequirements();
            result.buildScratchSizeInDwords = buildTopLevelBvh.GetScratchDataSizeInDwords(instanceCount);

            return result;
        }

        public SceneMemoryRequirements GetSceneMemoryRequirements(MeshBuildInfo[] buildInfos, BuildFlags buildFlags)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                buildFlags |= BuildFlags.PreferFastBuild;

            var requirements = new SceneMemoryRequirements();
            requirements.bottomLevelBvhSizeInDwords = new ulong[buildInfos.Length];
            requirements.bottomLevelBvhOffsetInNodes = new uint[buildInfos.Length];
            requirements.buildScratchSizeInDwords = 0;
            requirements.totalBottomLevelBvhSizeInDwords = 0;

            int i = 0;
            uint offset = 0;
            foreach (var buildInfo in buildInfos)
            {
                var meshRequirements = GetMeshBuildMemoryRequirements(buildInfo, buildFlags);

                requirements.buildScratchSizeInDwords = math.max(requirements.buildScratchSizeInDwords, meshRequirements.buildScratchSizeInDwords);
                requirements.totalBottomLevelBvhSizeInDwords += meshRequirements.resultSizeInDwords;
                requirements.bottomLevelBvhSizeInDwords[i] = meshRequirements.resultSizeInDwords;
                requirements.bottomLevelBvhOffsetInNodes[i] = offset;

                offset += (uint)(meshRequirements.resultSizeInDwords / (ulong)RadeonRaysAPI.BvhNodeSizeInDwords());
                i++;
            }

            ulong topLevelScratchSize = buildTopLevelBvh.GetScratchDataSizeInDwords((uint)buildInfos.Length);
            requirements.buildScratchSizeInDwords = math.max(requirements.buildScratchSizeInDwords, topLevelScratchSize);

            return requirements;
        }


        static public ulong GetTraceMemoryRequirements(uint rayCount)
        {
            return math.max(TraceGeometry.GetScratchDataSizeInDwords(rayCount), TraceScene.GetScratchDataSizeInDwords(rayCount));
        }

        public void Intersect(
            CommandBuffer cmd,
            GraphicsBuffer raysBuffer, uint rayCount,
            RayQueryType queryType, RayQueryOutputType queryOutputType,
            GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer,
            GraphicsBuffer accelStructBuffer, uint accelStructOffset, GraphicsBuffer scratchBuffer,
            GraphicsBuffer hitsBuffer)
        {
            traceGeometry.Execute(cmd, raysBuffer, rayCount, null, queryType, queryOutputType,
                vertexBuffer, indexBuffer, accelStructBuffer, accelStructOffset, scratchBuffer, hitsBuffer);
        }

        public void IntersectIndirect(
            CommandBuffer cmd,
            GraphicsBuffer raysBuffer, GraphicsBuffer indirectRayCount,
            RayQueryType queryType, RayQueryOutputType queryOutputType,
            GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer,
            GraphicsBuffer accelStructBuffer, uint accelStructOffset, GraphicsBuffer scratchBuffer,
            GraphicsBuffer hitsBuffer)
        {
            traceGeometry.Execute(cmd, raysBuffer, 0, indirectRayCount, queryType, queryOutputType,
                vertexBuffer, indexBuffer, accelStructBuffer, accelStructOffset, scratchBuffer, hitsBuffer);
        }

        public void Intersect(
            CommandBuffer cmd,
            GraphicsBuffer raysBuffer, uint rayCount, GraphicsBuffer indirectRayCount,
            RayQueryType queryType, RayQueryOutputType queryOutputType,
            GraphicsBuffer vertexBuffer, int vertexStride, GraphicsBuffer indexBuffer,
            TopLevelAccelStruct accelStruct, GraphicsBuffer scratchBuffer,
            GraphicsBuffer hitsBuffer)
        {
            traceScene.Execute(
                cmd,
                raysBuffer, rayCount, indirectRayCount, queryType, queryOutputType,
                vertexBuffer, vertexStride, indexBuffer,
                accelStruct, scratchBuffer, hitsBuffer);
        }
    }
}
