using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TSerializableLinkList<T> : ISerializationCallbackReceiver, IEnumerable<T>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private LinkedList<T> m_LinkList;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private T[] m_Values = Array.Empty<T>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Count => this.m_LinkList.Count;
        
        public bool IsEmpty => this.m_LinkList.Count == 0;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TSerializableLinkList()
        {
            this.m_LinkList = new LinkedList<T>();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Contains(T value)
        {
            return this.m_LinkList.Contains(value);
        }

        public void Clear()
        {
            this.m_LinkList.Clear();
        }
        
        public T First()
        {
            return m_LinkList.First.Value;
        }
        
        public T Last()
        {
            return this.m_LinkList.Last.Value;
        }

        public void AddFirst(T value)
        {
            this.m_LinkList.AddFirst(value);
        }
        
        public void AddLast(T value)
        {
            this.m_LinkList.AddLast(value);
        }

        public T RemoveFirst()
        {
            T value = this.First();
            this.m_LinkList.RemoveFirst();
            
            return value;
        }

        public T RemoveLast()
        {
            T value = this.Last();
            this.m_LinkList.RemoveLast();
            
            return value;
        }

        public bool Remove(T value)
        {
            return this.m_LinkList.Remove(value);
        }

        // ENUMERABLE: ----------------------------------------------------------------------------
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_LinkList.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.m_LinkList.GetEnumerator();
        }
        
        // SERIALIZATION CALLBACK: ----------------------------------------------------------------

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (AssemblyUtils.IsReloading) return;
            
            if (this.m_LinkList == null)
            {
                this.m_Values = Array.Empty<T>();
                return;
            }
            
            this.m_Values = new T[this.m_LinkList.Count];
            int index = 0;

            foreach (T value in this.m_LinkList)
            {
                this.m_Values[index] = value;
                index += 1;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (AssemblyUtils.IsReloading) return;
            this.m_LinkList = new LinkedList<T>(this.m_Values);
        }
    }
}