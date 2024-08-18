using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Local to World Right")]
    [Category("Transforms/Local to World Right")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Description("The Transform's right vector in world space")]

    [Keywords("Game Object")]
    [Serializable]
    public class GetDirectionLocalRight : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        public GetDirectionLocalRight()
        { }
        
        public GetDirectionLocalRight(Transform transform)
        {
            this.m_Transform = GetGameObjectTransform.Create(transform);
        }
        
        public override Vector3 Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? gameObject.transform.right : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionLocalRight()
        );

        public override string String => $"{this.m_Transform} Right";
    }
}