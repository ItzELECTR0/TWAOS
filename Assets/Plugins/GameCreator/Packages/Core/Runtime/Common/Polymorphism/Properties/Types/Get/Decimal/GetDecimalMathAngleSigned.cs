using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Directions Angle Signed")]
    [Category("Math/Geometry/Directions Angle Signed")]
    
    [Image(typeof(IconAlpha), ColorTheme.Type.TextNormal, typeof(OverlayDot))]
    [Description("The signed angle between two directions")]

    [Keywords("Degrees", "Radians")]
    
    [Serializable]
    public class GetDecimalMathAngleSigned : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDirection m_Direction1 = GetDirectionSelf.Create;
        [SerializeField] protected PropertyGetDirection m_Direction2 = GetDirectionTarget.Create;

        [SerializeField] private PropertyGetDirection m_Axis = GetDirectionConstantUp.Create;

        public override double Get(Args args)
        {
            Vector3 direction1 = this.m_Direction1.Get(args);
            Vector3 direction2 = this.m_Direction2.Get(args);
            Vector3 axis = this.m_Axis.Get(args);
            
            return Vector3.SignedAngle(direction1, direction2, axis);
        }

        public override string String => $"Angle [{this.m_Direction1}, {this.m_Direction2}]";

        public override double EditorValue
        {
            get
            {
                Vector3 direction1 = this.m_Direction1.EditorValue;
                Vector3 direction2 = this.m_Direction2.EditorValue;
                Vector3 axis = this.m_Axis.EditorValue;

                if (direction1 == Vector3.zero) return default;
                if (direction2 == Vector3.zero) return default;
                if (axis == Vector3.zero) return default;
                
                return Vector3.SignedAngle(
                    direction1,
                    direction2,
                    axis
                );
            }
        }
    }
}