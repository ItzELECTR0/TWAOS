using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Reflect from Plane")]
    [Category("Math/Reflect from Plane")]
    
    [Image(typeof(IconReflection), ColorTheme.Type.Green)]
    [Description("Reflects a direction going straight into the normal of a Plane")]

    [Serializable]
    public class GetDirectionMathReflectPlane : PropertyTypeGetDirection
    {
        [SerializeField]
        private PropertyGetDirection m_Direction = new PropertyGetDirection();
        
        [SerializeField]
        private PropertyGetDirection m_PlaneNormal = new PropertyGetDirection();

        public override Vector3 Get(Args args)
        {
            return Vector3.Reflect(
                this.m_Direction.Get(args),
                this.m_PlaneNormal.Get(args)
            );
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathReflectPlane()
        );
        
        public override string String => $"({this.m_Direction} reflect {this.m_PlaneNormal})";
    }
}