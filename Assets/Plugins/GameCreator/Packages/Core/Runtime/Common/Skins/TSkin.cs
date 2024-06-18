using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TSkin<T> : Skin where T : class
    {
        [SerializeReference] protected T m_Value;

        // PROPERTIES: ----------------------------------------------------------------------------

        public T Value => this.m_Value;
        public bool HasValue => this.m_Value != null;
    }
}