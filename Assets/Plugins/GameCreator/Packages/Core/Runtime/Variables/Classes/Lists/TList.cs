using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TList<T> : TPolymorphicList<T> where T : TVariable
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeReference] private List<T> m_Source = new List<T>(); 
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Source.Count;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected TList()
        { }

        protected TList(params T[] variables) : this()
        {
            this.m_Source = new List<T>(variables);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Get(int index)
        {
            return this.m_Source[index];
        }

        public void Set(int index, T value)
        {
            this.m_Source[index] = value;
        }

        public void Add(T value)
        {
            this.m_Source.Add(value);
        }

        public void Remove(int index)
        {
            this.m_Source.RemoveAt(index);
        }
    }
}