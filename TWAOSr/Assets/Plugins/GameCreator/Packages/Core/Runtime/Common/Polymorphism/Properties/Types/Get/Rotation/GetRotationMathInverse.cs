using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Inverse Rotation")]
    [Category("Math/Inverse Rotation")]
    
    [Image(typeof(IconContrast), ColorTheme.Type.Blue)]
    [Description("Inverses the Quaternion rotation")]

    [Serializable]
    public class GetRotationMathInverse : PropertyTypeGetRotation
    {
        [SerializeField] protected PropertyGetRotation m_Rotation = GetRotationIdentity.Create;

        public override Quaternion Get(Args args)
        {
            Quaternion rotation = this.m_Rotation.Get(args);
            return Quaternion.Inverse(rotation);
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationMathInverse()
        );

        public override string String => $"Inverse {this.m_Rotation}";
    }
}