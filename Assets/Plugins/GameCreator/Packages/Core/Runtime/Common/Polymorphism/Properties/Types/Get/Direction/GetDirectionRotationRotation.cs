using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("From Rotation")]
    [Category("Rotation/From Rotation")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Green)]
    [Description("The forward direction defined by the Quaternion rotation")]

    [Keywords("Rotation", "Euler", "Angle", "Axis")]
    [Serializable]
    public class GetDirectionRotationRotation : PropertyTypeGetDirection
    {
        [SerializeField] protected PropertyGetRotation m_Rotation = GetRotationEuler.Create();

        public GetDirectionRotationRotation()
        { }
        
        public GetDirectionRotationRotation(Vector3 angleAxis)
        {
            this.m_Rotation = GetRotationEuler.Create(angleAxis);
        }

        public override Vector3 Get(Args args)
        {
            Quaternion rotation = this.m_Rotation.Get(args);
            return rotation * Vector3.forward;
        }

        public static PropertyGetDirection Create(Vector3 angleAxis) => new PropertyGetDirection(
            new GetDirectionRotationRotation(angleAxis)
        );

        public override string String => $"{this.m_Rotation}";
        
        public override Vector3 EditorValue => this.m_Rotation.EditorValue * Vector3.forward;
    }
}