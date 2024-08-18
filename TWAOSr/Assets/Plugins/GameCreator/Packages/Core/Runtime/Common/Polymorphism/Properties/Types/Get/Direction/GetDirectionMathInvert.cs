using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Invert Direction")]
    [Category("Math/Invert Direction")]
    
    [Image(typeof(IconContrast), ColorTheme.Type.Green)]
    [Description("Inverses the direction orientation")]

    [Serializable]
    public class GetDirectionMathInvert : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetDirection m_Direction = GetDirectionSelf.Create;

        public override Vector3 Get(Args args)
        {
            return this.m_Direction.Get(args) * -1;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathInvert()
        );

        public override string String => $"-{this.m_Direction}";
    }
}