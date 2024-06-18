using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Cross Product")]
    [Category("Math/Cross Product")]
    
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    [Description("Calculates the cross product between two directions")]

    [Serializable]
    public class GetDirectionMathCrossProduct : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetDirection m_Direction1 = GetDirectionSelf.Create;
        [SerializeField] private PropertyGetDirection m_Direction2 = GetDirectionTarget.Create;

        public override Vector3 Get(Args args)
        {
            Vector3 direction1 = this.m_Direction1.Get(args);
            Vector3 direction2 = this.m_Direction2.Get(args);
            
            return Vector3.Cross(direction1, direction2);
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathCrossProduct()
        );

        public override string String => $"({this.m_Direction1} * {this.m_Direction2})";
    }
}