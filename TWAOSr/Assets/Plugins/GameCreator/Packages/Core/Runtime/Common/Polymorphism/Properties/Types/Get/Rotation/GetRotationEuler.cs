using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Euler Angles")]
    [Category("Values/Euler Angles")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Yellow)]
    [Description("Rotation from the euler angle of each individual axis in world space")]

    [Serializable]
    public class GetRotationEuler : PropertyTypeGetRotation
    {
        [SerializeField] private PropertyGetDecimal m_X = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Y = GetDecimalConstantZero.Create;
        [SerializeField] private PropertyGetDecimal m_Z = GetDecimalConstantZero.Create;
        
        public override Quaternion Get(Args args)
        {
            float x = (float) this.m_X.Get(args);
            float y = (float) this.m_Y.Get(args);
            float z = (float) this.m_Z.Get(args);
            
            return Quaternion.Euler(new Vector3(x, y, z));
        }

        public GetRotationEuler()
        { }
        
        public GetRotationEuler(Vector3 angles)
        {
            this.m_X = GetDecimalDecimal.Create(angles.x);
            this.m_Y = GetDecimalDecimal.Create(angles.y);
            this.m_Z = GetDecimalDecimal.Create(angles.z);
        }
        
        public GetRotationEuler(float x, float y, float z)
        {
            this.m_X = GetDecimalDecimal.Create(x);
            this.m_Y = GetDecimalDecimal.Create(y);
            this.m_Z = GetDecimalDecimal.Create(z);
        }
        
        public GetRotationEuler(Transform transform)
            : this(transform != null ? transform.rotation.eulerAngles: Vector3.zero)
        { }
        
        public static PropertyGetRotation Create() => new PropertyGetRotation(
            new GetRotationEuler()
        );
        
        public static PropertyGetRotation Create(Vector3 euler) => new PropertyGetRotation(
            new GetRotationEuler(euler)
        );

        public override string String => $"Euler ({this.m_X}, {this.m_Y}, {this.m_Z})";
    }
}