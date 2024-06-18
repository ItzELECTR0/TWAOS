using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TRun<TValue>
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] protected GameObject m_Template;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract TValue Value { get; }
        protected abstract GameObject Template { get; }
    }
}