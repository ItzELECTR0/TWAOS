using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Transform Point")]
    [Category("Transforms/Transform Point")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Description("Transforms the local space point to world space and returns the value")]
    
    [Serializable]
    public class GetPositionTransformPoint : PropertyTypeGetPosition
    {
        [SerializeField] protected PropertyGetGameObject m_From = GetGameObjectPlayer.Create();
        [SerializeField] protected PropertyGetPosition m_Point = GetPositionVector3.Create();
        
        public override Vector3 Get(Args args)
        {
            GameObject from = this.m_From.Get(args);
            Vector3 point = this.m_Point.Get(args);
            
            return from != null ? from.transform.TransformPoint(point) : point;
        }

        public GetPositionTransformPoint()
        { }
        
        public GetPositionTransformPoint(Vector3 point)
        {
            this.m_Point = GetPositionVector3.Create(point);
        }
        
        public static PropertyGetPosition Create(Vector3 point) => new PropertyGetPosition(
            new GetPositionTransformPoint(point)
        );

        public override string String => $"{this.m_From} {this.m_Point}";
    }
}