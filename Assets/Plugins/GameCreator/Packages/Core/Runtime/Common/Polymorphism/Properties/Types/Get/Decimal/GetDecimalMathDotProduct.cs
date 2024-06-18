using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Dot Product")]
    [Category("Math/Geometry/Dot Product")]
    
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.TextNormal)]
    [Description("The dot product between two directions")]

    [Keywords("Orthogonal", "Perpendicular", "Multiply")]
    
    [Serializable]
    public class GetDecimalMathDotProduct : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDirection m_Direction1 = GetDirectionSelf.Create;
        [SerializeField] protected PropertyGetDirection m_Direction2 = GetDirectionTarget.Create;

        public override double Get(Args args)
        {
            Vector3 direction1 = this.m_Direction1.Get(args);
            Vector3 direction2 = this.m_Direction2.Get(args);
            return Vector3.Dot(direction1, direction2);
        }
        
        public override string String => $"Dot [{this.m_Direction1}, {this.m_Direction2}]";

        public override double EditorValue
        {
            get
            {
                Vector3 direction1 = this.m_Direction1.EditorValue;
                Vector3 direction2 = this.m_Direction2.EditorValue;

                if (direction1 == Vector3.zero) return default;
                if (direction2 == Vector3.zero) return default;
                
                return Vector3.Dot(
                    direction1,
                    direction2
                );
            }
        }
    }
}