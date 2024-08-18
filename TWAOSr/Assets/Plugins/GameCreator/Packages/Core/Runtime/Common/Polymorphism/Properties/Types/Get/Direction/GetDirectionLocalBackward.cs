using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Local to World Backward")]
    [Category("Transforms/Local to World Backward")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowDown))]
    [Description("The Transform's backward vector in world space")]

    [Keywords("Game Object")]
    [Serializable]
    public class GetDirectionLocalBackward : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        public GetDirectionLocalBackward()
        { }
        
        public GetDirectionLocalBackward(Transform transform)
        {
            this.m_Transform = GetGameObjectTransform.Create(transform);
        }
        
        public override Vector3 Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? -gameObject.transform.forward : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionLocalBackward()
        );

        public override string String => $"{this.m_Transform} Backward";
    }
}