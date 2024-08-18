using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Multiply Rotations")]
    [Category("Math/Multiply Rotations")]
    
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Blue)]
    [Description("Multiplies two Quaternions, which results in the sum of both rotations")]

    [Serializable]
    public class GetRotationMathMultiply : PropertyTypeGetRotation
    {
        [SerializeField] protected PropertyGetRotation m_Rotation1 = GetRotationIdentity.Create;
        [SerializeField] protected PropertyGetRotation m_Rotation2 = GetRotationEuler.Create();

        public override Quaternion Get(Args args)
        {
            Quaternion rotation1 = this.m_Rotation1.Get(args);
            Quaternion rotation2 = this.m_Rotation2.Get(args);
            
            return rotation1 * rotation2;
        }

        public GetRotationMathMultiply()
        { }

        public GetRotationMathMultiply(PropertyGetRotation a, PropertyGetRotation b)
        {
            this.m_Rotation1 = a;
            this.m_Rotation2 = b;
        }
        
        public static PropertyGetRotation Create()
        {
            return new PropertyGetRotation(new GetRotationMathMultiply());
        }

        public override string String => $"{this.m_Rotation1} * {this.m_Rotation2}";
    }
}