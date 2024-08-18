using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector Components")]
    [Category("Values/Vector Components")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow)]
    [Description("A vector with each component as a dynamic property")]

    [Serializable]
    public class GetPositionValueDecimals : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetDecimal m_X = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Y = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Z = GetDecimalConstantZero.Create;
        
        public override Vector3 Get(Args args)
        {
            return new Vector3(
                (float) this.m_X.Get(args),
                (float) this.m_Y.Get(args),
                (float) this.m_Z.Get(args)
            );
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionValueDecimals()
        );

        public override string String => $"({this.m_X}, {this.m_Y}, {this.m_Z})";
        
        public override Vector3 EditorValue => new Vector3(
            (float) this.m_X.EditorValue,
            (float) this.m_Y.EditorValue,
            (float) this.m_Z.EditorValue
        );
    }
}