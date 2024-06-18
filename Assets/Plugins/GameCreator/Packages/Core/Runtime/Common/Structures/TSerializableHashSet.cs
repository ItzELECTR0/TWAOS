using System;
using System.Collections;
using System.Collections.Generic;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TSerializableHashSet<T> : ISerializationCallbackReceiver, IEnumerable<T>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private HashSet<T> m_HashSet;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private T[] m_Values = Array.Empty<T>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Count => this.m_HashSet.Count;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TSerializableHashSet()
        {
            this.m_HashSet = new HashSet<T>();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Contains(T value)
        {
            return this.m_HashSet.Contains(value);
        }

        public void Clear()
        {
            this.m_HashSet.Clear();
        }

        public bool Add(T value)
        {
            return this.m_HashSet.Add(value);
        }

        public bool Remove(T value)
        {
            return this.m_HashSet.Remove(value);
        }
        
        // ENUMERABLE: ----------------------------------------------------------------------------
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_HashSet.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.m_HashSet.GetEnumerator();
        }
        
        // SERIALIZATION CALLBACK: ----------------------------------------------------------------

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (AssemblyUtils.IsReloading) return;
            
            if (this.m_HashSet == null)
            {
                this.m_Values = Array.Empty<T>();
                return;
            }
            
            this.m_Values = new T[this.m_HashSet.Count];
            int index = 0;

            foreach (T value in this.m_HashSet)
            {
                this.m_Values[index] = value;
                index += 1;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (AssemblyUtils.IsReloading) return;
            
            this.m_HashSet = new HashSet<T>();
            foreach (T value in this.m_Values)
            {
                this.m_HashSet.Add(value);
            }
        }
    }
}