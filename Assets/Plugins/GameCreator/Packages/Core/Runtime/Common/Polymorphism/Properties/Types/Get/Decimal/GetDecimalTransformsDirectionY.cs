using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Direction Y")]
    [Category("Transforms/Direction Y")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("The Y component of a Vector3 that represents a direction")]

    [Keywords("Position", "Vector3", "Up", "Down")]
    
    [Serializable]
    public class GetDecimalTransformsDirectionY : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetDirection m_Direction = GetDirectionSelf.Create;

        public override double Get(Args args) => this.m_Direction.Get(args).y;
        public override double Get(GameObject gameObject) => this.m_Direction.Get(gameObject).y;

        public override string String => $"{this.m_Direction}.Y";
    }
}