using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Subtract Directions")]
    [Category("Math/Subtract Directions")]
    
    [Image(typeof(IconMinusCircle), ColorTheme.Type.Green)]
    [Description("Subtracts two directions")]

    [Serializable]
    public class GetDirectionMathSubtract : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetDirection m_Direction1 = GetDirectionSelf.Create;
        [SerializeField] private PropertyGetDirection m_Direction2 = GetDirectionTarget.Create;

        public override Vector3 Get(Args args)
        {
            Vector3 direction1 = this.m_Direction1.Get(args);
            Vector3 direction2 = this.m_Direction2.Get(args);
            
            return direction1 - direction2;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathSubtract()
        );

        public override string String => $"({this.m_Direction1} - {this.m_Direction2})";
    }
}