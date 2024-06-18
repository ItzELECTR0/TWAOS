using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scale Number")]
    [Category("Math/Scale Number")]
    
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Yellow)]
    [Description("Scale a Scale property uniformly by a certain value")]

    [Serializable]
    public class GetScaleMathScale : PropertyTypeGetScale
    {
        [SerializeField] private PropertyGetScale m_Scale = GetScalePlayer.Create;
        [SerializeField] private PropertyGetDecimal m_Amount = GetDecimalConstantOne.Create;

        public override Vector3 Get(Args args) => this.m_Scale.Get(args) * (float) this.m_Amount.Get(args);
        
        public static PropertyGetScale Create => new PropertyGetScale(
            new GetScaleMathScale()
        );
        
        public override string String => $"({this.m_Scale} * {this.m_Amount})";
    }
}