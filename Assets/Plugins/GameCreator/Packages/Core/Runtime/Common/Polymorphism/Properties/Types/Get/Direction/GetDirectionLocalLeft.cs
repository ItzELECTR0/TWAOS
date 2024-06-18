using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Local to World Left")]
    [Category("Transforms/Local to World Left")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    [Description("The Transform's left vector in world space")]

    [Keywords("Game Object")]
    [Serializable]
    public class GetDirectionLocalLeft : PropertyTypeGetDirection
    {
        [SerializeField] 
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        public GetDirectionLocalLeft()
        { }
        
        public GetDirectionLocalLeft(Transform transform)
        {
            this.m_Transform = GetGameObjectTransform.Create(transform);
        }
        
        public override Vector3 Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? -gameObject.transform.right : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionLocalLeft()
        );

        public override string String => $"{this.m_Transform} Left";
    }
}