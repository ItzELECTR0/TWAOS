using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TypeReference<T>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private string m_TypeName;

        // PROPERTIES: ---------------------------------------------------------------------------- 

        public Type Base => typeof(T);
        
        public Type Type => Type.GetType(this.m_TypeName, false);
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.Type != null 
                ? TextUtils.Humanize(this.Type.Name) 
                : "(none)";
        }
    }
}