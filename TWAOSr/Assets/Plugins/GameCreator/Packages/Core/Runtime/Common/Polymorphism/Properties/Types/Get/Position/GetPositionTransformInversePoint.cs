using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Inverse Transform Point")]
    [Category("Transforms/Inverse Transform Point")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    [Description("Transforms the world space point to local space and returns the value")]
    
    [Serializable]
    public class GetPositionTransformInversePoint : PropertyTypeGetPosition
    {
        [SerializeField] protected PropertyGetPosition m_Point = GetPositionVector3.Create();
        [SerializeField] protected PropertyGetGameObject m_To = GetGameObjectPlayer.Create();
        
        public override Vector3 Get(Args args)
        {
            GameObject to = this.m_To.Get(args);
            Vector3 point = this.m_Point.Get(args);
            
            return to != null ? to.transform.InverseTransformPoint(point) : point;
        }

        public GetPositionTransformInversePoint()
        { }
        
        public GetPositionTransformInversePoint(Vector3 point)
        {
            this.m_Point = GetPositionVector3.Create(point);
        }
        
        public static PropertyGetPosition Create(Vector3 point) => new PropertyGetPosition(
            new GetPositionTransformInversePoint(point)
        );

        public override string String => $"{this.m_To} {this.m_Point}";
    }
}