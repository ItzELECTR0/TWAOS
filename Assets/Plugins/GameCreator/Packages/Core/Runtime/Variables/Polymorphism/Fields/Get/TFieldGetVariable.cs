using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TFieldGetVariable
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected IdString m_TypeID = ValueNull.TYPE_ID;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public T Get<T>(Args args)
        {
            object value = this.Get(args);
            return value switch
            {
                T valueTyped => valueTyped,
                _ => Convert.ChangeType(value, typeof(T)) is T valueConverted
                    ? valueConverted
                    : default
            };
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract object Get(Args args);
        public abstract override string ToString();
    }
}