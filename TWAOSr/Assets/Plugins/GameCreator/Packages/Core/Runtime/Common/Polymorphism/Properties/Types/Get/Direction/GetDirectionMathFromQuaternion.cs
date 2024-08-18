using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("From Rotation")]
    [Category("Math/From Rotation")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Green)]
    [Description("Creates a direction a Quaternion rotation")]

    [Serializable]
    public class GetDirectionMathFromQuaternion : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetRotation m_Rotation;

        public override Vector3 Get(Args args) => this.m_Rotation.Get(args) * Vector3.forward;
        public override Vector3 Get(GameObject args) => this.m_Rotation.Get(args) * Vector3.forward;

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathFromQuaternion()
        );

        public override string String => this.m_Rotation.ToString();
    }
}