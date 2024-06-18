using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("From Positions")]
    [Category("Math/From Positions")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("Creates a direction from a point towards another")]

    [Serializable]
    public class GetDirectionMathFromTo : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetPosition m_From = GetPositionSelf.Create();
        [SerializeField] private PropertyGetPosition m_To = GetPositionTarget.Create();

        public override Vector3 Get(Args args)
        {
            Vector3 from = this.m_From.Get(args);
            Vector3 to = this.m_To.Get(args);

            return to - from;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathFromTo()
        );

        public override string String => $"({this.m_From} -> {this.m_To})";
    }
}