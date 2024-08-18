using System;
using System.Reflection;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TReflectionMember<T> : IReflectionMember
    {
        protected const BindingFlags BINDINGS = BindingFlags.Public    |
                                                BindingFlags.NonPublic |
                                                BindingFlags.Instance;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected Component m_Component;
        [SerializeField] protected string m_Member;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Type Type => typeof(T);

        public abstract T Value { get; set; }

        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Component != null
                ? $"{this.m_Component.gameObject.name}[{this.m_Member}]"
                : "(none)";
        }
    }
}