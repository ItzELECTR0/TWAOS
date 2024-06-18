using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Euler Vector3")]
    [Category("Constants/Euler Vector3")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Yellow)]
    [Description("Rotation from the euler angle of each individual axis in world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetRotationConstantEulerVector : PropertyTypeGetRotation
    {
        [SerializeField] private Vector3 m_Angles = Vector3.zero; 
        
        public override Quaternion Get(Args args)
        {
            return Quaternion.Euler(this.m_Angles);
        }

        public GetRotationConstantEulerVector()
        { }
        
        public GetRotationConstantEulerVector(Vector3 angles)
        {
            this.m_Angles = angles;
        }
        
        public GetRotationConstantEulerVector(float x, float y, float z)
        {
            this.m_Angles = new Vector3(x, y, z);
        }
        
        public static PropertyGetRotation Create() => new PropertyGetRotation(
            new GetRotationConstantEulerVector()
        );
        
        public static PropertyGetRotation Create(Vector3 euler) => new PropertyGetRotation(
            new GetRotationConstantEulerVector(euler)
        );

        public override string String => $"Euler {this.m_Angles}";
    }
}