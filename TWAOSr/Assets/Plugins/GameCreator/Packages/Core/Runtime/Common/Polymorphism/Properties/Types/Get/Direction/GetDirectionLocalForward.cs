using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Local to World Forward")]
    [Category("Transforms/Local to World Forward")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowUp))]
    [Description("The Transform's forward vector in world space")]

    [Keywords("Game Object")]
    [Serializable]
    public class GetDirectionLocalForward : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        public GetDirectionLocalForward()
        { }
        
        public GetDirectionLocalForward(Transform transform)
        {
            this.m_Transform = GetGameObjectTransform.Create(transform);
        }
        
        public override Vector3 Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? gameObject.transform.forward : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionLocalForward()
        );

        public override string String => $"{this.m_Transform} Forward";
    }
}