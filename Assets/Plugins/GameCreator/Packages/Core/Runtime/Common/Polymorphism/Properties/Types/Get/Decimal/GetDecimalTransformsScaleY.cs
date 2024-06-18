using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scale Y")]
    [Category("Transforms/Scale Y")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("The Y component of a Vector3 that represents a scale")]

    [Keywords("Position", "Vector3", "Up", "Down")]
    
    [Serializable]
    public class GetDecimalTransformsScaleY : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetScale m_Scale = GetScaleSelf.Create;

        public override double Get(Args args) => this.m_Scale.Get(args).y;
        public override double Get(GameObject gameObject) => this.m_Scale.Get(gameObject).y;

        public override string String => $"{this.m_Scale}.Y";
    }
}