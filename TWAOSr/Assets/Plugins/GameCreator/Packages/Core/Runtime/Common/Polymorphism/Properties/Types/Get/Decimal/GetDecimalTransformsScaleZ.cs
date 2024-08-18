using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scale Z")]
    [Category("Transforms/Scale Z")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Blue)]
    [Description("The Z component of a Vector3 that represents a scale")]

    [Keywords("Position", "Vector3", "Forward", "Backward")]
    
    [Serializable]
    public class GetDecimalTransformsScaleZ : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetScale m_Scale = GetScaleSelf.Create;

        public override double Get(Args args) => this.m_Scale.Get(args).z;
        public override double Get(GameObject gameObject) => this.m_Scale.Get(gameObject).z;

        public override string String => $"{this.m_Scale}.Z";
    }
}