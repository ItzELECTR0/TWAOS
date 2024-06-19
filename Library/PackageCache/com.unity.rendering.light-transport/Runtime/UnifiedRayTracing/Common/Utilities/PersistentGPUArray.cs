using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal class PersistentGpuArray<Tstruct> : IDisposable
        where Tstruct : struct
    {
        BlockAllocator m_SlotAllocator;
        ComputeBuffer m_GpuBuffer;
        NativeArray<Tstruct> m_CpuList;
        bool m_gpuBufferDirty = true;
        int m_ElementCount = 0;
        public int elementCount { get { return m_ElementCount; } }

        public PersistentGpuArray(int initialSize)
        {
            m_SlotAllocator.Initialize(initialSize);
            m_GpuBuffer = new ComputeBuffer(initialSize, Marshal.SizeOf<Tstruct>());
            m_CpuList = new NativeArray<Tstruct>(initialSize, Allocator.Persistent);
            m_ElementCount = 0;
        }
        public void Dispose()
        {
            m_ElementCount = 0;
            m_SlotAllocator.Dispose();
            m_GpuBuffer.Dispose();
            m_CpuList.Dispose();
        }

        public BlockAllocator.Allocation Add(Tstruct element)
        {
            m_ElementCount++;
            var slotAllocation = m_SlotAllocator.Allocate(1);
            if (!slotAllocation.valid)
            {
                Grow();
                slotAllocation = m_SlotAllocator.Allocate(1);
                Assert.IsTrue(slotAllocation.valid);
            }
            m_CpuList[slotAllocation.block.offset] = element;
            m_gpuBufferDirty = true;
            return slotAllocation;
        }

        public void Remove(BlockAllocator.Allocation allocation)
        {
            m_ElementCount--;
            m_SlotAllocator.FreeAllocation(allocation);
        }

        public void Clear()
        {
            m_ElementCount = 0;
            var currentCapacity = m_SlotAllocator.capacity;
            m_SlotAllocator.Dispose();
            m_SlotAllocator = new BlockAllocator();
            m_SlotAllocator.Initialize(currentCapacity);
        }

        public void Set(BlockAllocator.Allocation allocation, Tstruct element)
        {
            m_CpuList[allocation.block.offset] = element;
            m_gpuBufferDirty = true;
        }

        public Tstruct Get(BlockAllocator.Allocation allocation)
        {
            return m_CpuList[allocation.block.offset];
        }

        public void ModifyForEach(Func<Tstruct, Tstruct> lambda)
        {
            for (int i = 0; i < m_CpuList.Length; ++i)
            {
                m_CpuList[i] = lambda(m_CpuList[i]);
            }

            m_gpuBufferDirty = true;
        }

        // Note: this should ideally be used with only one command buffer. If used with more than one cmd buffers, the order of their execution is important.
        public ComputeBuffer GetGpuBuffer(CommandBuffer cmd)
        {
            if (m_gpuBufferDirty)
            {
                cmd.SetBufferData(m_GpuBuffer, m_CpuList);
                m_gpuBufferDirty = false;
            }

            return m_GpuBuffer;
        }
        private void Grow()
        {
            var oldCapacity = m_SlotAllocator.capacity;
            m_SlotAllocator.Grow(m_SlotAllocator.capacity + 1);

            m_GpuBuffer.Dispose();
            m_GpuBuffer = new ComputeBuffer(m_SlotAllocator.capacity, Marshal.SizeOf<Tstruct>());

            var oldList = m_CpuList;
            m_CpuList = new NativeArray<Tstruct>(m_SlotAllocator.capacity, Allocator.Persistent);
            NativeArray<Tstruct>.Copy(oldList, m_CpuList, oldCapacity);
            oldList.Dispose();
        }
    }
}

