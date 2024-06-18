using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Direction Z")]
    [Category("Transforms/Direction Z")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Blue)]
    [Description("The Z component of a Vector3 that represents a direction")]

    [Keywords("Position", "Vector3", "Forward", "Backward")]
    
    [Serializable]
    public class GetDecimalTransformsDirectionZ : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetDirection m_Direction = GetDirectionSelf.Create;

        public override double Get(Args args) => this.m_Direction.Get(args).z;
        public override double Get(GameObject gameObject) => this.m_Direction.Get(gameObject).z;

        public override string String => $"{this.m_Direction}.Z";
    }
}