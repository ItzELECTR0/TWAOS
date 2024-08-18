using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Project on Plane")]
    [Category("Math/Project on Plane")]
    
    [Image(typeof(IconProjection), ColorTheme.Type.Green)]
    [Description("Projects a direction onto the normal of a Plane")]

    [Serializable]
    public class GetDirectionMathProjectPlane : PropertyTypeGetDirection
    {
        [SerializeField]
        private PropertyGetDirection m_Direction = new PropertyGetDirection();
        
        [SerializeField]
        private PropertyGetDirection m_PlaneNormal = new PropertyGetDirection();

        public override Vector3 Get(Args args)
        {
            return Vector3.ProjectOnPlane(
                this.m_Direction.Get(args),
                this.m_PlaneNormal.Get(args)
            );
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathProjectPlane()
        );
        
        public override string String => $"({this.m_Direction} project {this.m_PlaneNormal})";
    }
}