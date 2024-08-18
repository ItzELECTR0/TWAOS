using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scale X")]
    [Category("Transforms/Scale X")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Red)]
    [Description("The X component of a Vector3 that represents a scale")]

    [Keywords("Position", "Vector3", "Right", "Left")]
    
    [Serializable]
    public class GetDecimalTransformsScaleX : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetScale m_Scale = GetScaleSelf.Create;

        public override double Get(Args args) => this.m_Scale.Get(args).x;
        public override double Get(GameObject gameObject) => this.m_Scale.Get(gameObject).x;

        public override string String => $"{this.m_Scale}.X";
    }
}