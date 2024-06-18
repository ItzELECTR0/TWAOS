using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("From To Direction")]
    [Category("Math/From To Direction")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow)]
    [Description("Rotation from a direction towards another one")]
    
    [Serializable]
    public class GetRotationFromToDirection : PropertyTypeGetRotation
    {
        [SerializeField]
        protected PropertyGetDirection m_From = GetDirectionConstantForward.Create;
        
        [SerializeField]
        protected PropertyGetDirection m_To = GetDirectionConstantRight.Create;
        
        public GetRotationFromToDirection()
        { }
        
        public GetRotationFromToDirection(Vector3 from, Vector3 to) : this()
        {
            this.m_From = GetDirectionVector.Create(from);
            this.m_To = GetDirectionVector.Create(to);
        }

        public override Quaternion Get(Args args)
        {
            Vector3 from = this.m_From.Get(args);
            Vector3 to = this.m_From.Get(args);
            
            return Quaternion.FromToRotation(from, to);
        }

        public static PropertyGetRotation Create(Vector3 from, Vector3 to) => new PropertyGetRotation(
            new GetRotationFromToDirection(from, to)
        );

        public override string String => $"[{this.m_From} -> {this.m_To}]";
    }
}