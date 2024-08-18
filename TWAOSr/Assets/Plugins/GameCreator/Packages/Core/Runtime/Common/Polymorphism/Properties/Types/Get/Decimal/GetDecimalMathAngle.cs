using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Directions Angle Absolute")]
    [Category("Math/Geometry/Directions Angle Absolute")]
    
    [Image(typeof(IconAlpha), ColorTheme.Type.TextNormal)]
    [Description("The absolute angle (without sign) between two directions")]

    [Keywords("Degrees", "Radians")]
    
    [Serializable]
    public class GetDecimalMathAngle : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDirection m_Direction1 = GetDirectionSelf.Create;
        [SerializeField] protected PropertyGetDirection m_Direction2 = GetDirectionTarget.Create;

        public override double Get(Args args)
        {
            Vector3 direction1 = this.m_Direction1.Get(args);
            Vector3 direction2 = this.m_Direction2.Get(args);
            return Vector3.Angle(direction1, direction2);
        }

        public override string String => $"Angle [{this.m_Direction1}, {this.m_Direction2}]";

        public override double EditorValue
        {
            get
            {
                Vector3 direction1 = this.m_Direction1.EditorValue;
                Vector3 direction2 = this.m_Direction2.EditorValue;

                if (direction1 == Vector3.zero) return default;
                if (direction2 == Vector3.zero) return default;
                
                return Vector3.Angle(
                    direction1,
                    direction2
                );
            }
        }
    }
}