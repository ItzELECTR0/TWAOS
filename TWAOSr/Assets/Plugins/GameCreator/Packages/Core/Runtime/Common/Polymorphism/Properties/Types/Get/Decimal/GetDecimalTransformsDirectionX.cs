using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Direction X")]
    [Category("Transforms/Direction X")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Red)]
    [Description("The X component of a Vector3 that represents a direction")]

    [Keywords("Position", "Vector3", "Right", "Left")]
    
    [Serializable]
    public class GetDecimalTransformsDirectionX : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetDirection m_Direction = GetDirectionSelf.Create;

        public override double Get(Args args) => this.m_Direction.Get(args).x;
        public override double Get(GameObject gameObject) => this.m_Direction.Get(gameObject).x;

        public override string String => $"{this.m_Direction}.X";
    }
}