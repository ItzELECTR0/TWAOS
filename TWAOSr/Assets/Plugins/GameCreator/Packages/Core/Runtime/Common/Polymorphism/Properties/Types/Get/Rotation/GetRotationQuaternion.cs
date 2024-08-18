using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Quaternion")]
    [Category("Values/Quaternion")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Yellow)]
    [Description("Creates a rotation using the 4D axis of a Quaternion")]

    [Serializable]
    public class GetRotationQuaternion : PropertyTypeGetRotation
    {
        [SerializeField] private PropertyGetDecimal m_X = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Y = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Z = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_W = GetDecimalConstantZero.Create;
        
        public override Quaternion Get(Args args)
        {
            float x = (float) this.m_X.Get(args);
            float y = (float) this.m_Y.Get(args);
            float z = (float) this.m_Z.Get(args);
            float w = (float) this.m_W.Get(args);

            return new Quaternion(x, y, z, w);
        }

        public GetRotationQuaternion()
        { }

        public GetRotationQuaternion(Quaternion quaternion)
        {
            this.m_X = GetDecimalDecimal.Create(quaternion.x);
            this.m_Y = GetDecimalDecimal.Create(quaternion.y);
            this.m_Z = GetDecimalDecimal.Create(quaternion.z);
            this.m_W = GetDecimalDecimal.Create(quaternion.w);
        }
        
        public GetRotationQuaternion(Transform transform)
            : this(transform != null ? transform.rotation : Quaternion.identity)
        { }
        
        public static PropertyGetRotation Create() => new PropertyGetRotation(
            new GetRotationQuaternion()
        );
        
        public static PropertyGetRotation Create(Quaternion quaternion) => new PropertyGetRotation(
            new GetRotationQuaternion(quaternion)
        );
        
        public override string String => $"({this.m_X}, {this.m_Y}, {this.m_Z}, {this.m_W})";
    }
}