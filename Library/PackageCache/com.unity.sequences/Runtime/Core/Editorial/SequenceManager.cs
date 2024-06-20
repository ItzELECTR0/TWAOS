using System;
using System.Collections.Generic;

namespace UnityEngine.Sequences
{
    class StackInt : Stack<int> {}

    [Serializable]
    internal class SequenceManager  // TODO (FTV-581): Make it enumerable
    {
        [SerializeReference] List<Sequence> m_Data;
        [SerializeReference] StackInt m_AvailableIndices;  // TODO (FTV-581): Remove and manage indices when needed.

        public int count
        {
            get
            {
                if (m_Data == null)
                    return 0;

                int count = m_Data.Count;

                if (m_AvailableIndices != null)
                    count -= m_AvailableIndices.Count;

                return count;
            }
        }

        public IEnumerable<Sequence> sequences
        {
            get
            {
                if (m_Data == null) yield break;

                foreach (var data in m_Data)
                {
                    if (data != null)
                        yield return data;
                }
            }
        }

        public Sequence GetAt(int index)
        {
            return m_Data[index];
        }

        public int Add(Sequence value)
        {
            if (m_Data == null)
                m_Data = new List<Sequence>();

            // Ensure that a newly added masterSequence clip has `this` as its manager.
            if (value.manager != this)
                value.manager = this;

            int index = GetIndex(value);
            if (index >= 0)  // Provided value is already in the manager.
                return index;

            if (m_AvailableIndices == null || m_AvailableIndices.Count == 0)
            {
                m_Data.Add(value);
                return m_Data.Count - 1;
            }

            int insertedIndex = m_AvailableIndices.Pop();
            m_Data[insertedIndex] = value;
            return insertedIndex;
        }

        void Remove(int index)
        {
            if (m_AvailableIndices == null)
                m_AvailableIndices = new StackInt();

            m_AvailableIndices.Push(index);
            m_Data[index] = null;
        }

        public void Remove(Sequence value)
        {
            int index = GetIndex(value);
            Remove(index);
        }

        public int GetIndex(Sequence value)
        {
            if (m_Data == null)
                return -1;

            for (int i = 0; i < m_Data.Count; i++)
            {
                if (m_Data[i] != null && m_Data[i].Equals(value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
