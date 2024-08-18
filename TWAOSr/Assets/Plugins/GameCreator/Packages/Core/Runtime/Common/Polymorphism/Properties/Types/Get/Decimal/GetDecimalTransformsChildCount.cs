using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Child Count")]
    [Category("Transforms/Child Count")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]
    [Description("The number of child game objects hanging from the referenced game object")]

    [Keywords("Float", "Decimal", "Double")]
    [Serializable]
    public class GetDecimalTransformsChildCount : PropertyTypeGetDecimal
    {
        [SerializeField] 
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? gameObject.transform.childCount : 0;
        }

        public GetDecimalTransformsChildCount() : base()
        { }

        public GetDecimalTransformsChildCount(Transform transform) : this()
        {
            GameObject gameObject = transform != null ? transform.gameObject : null;
            this.m_Transform = GetGameObjectInstance.Create(gameObject);
        }

        public static PropertyGetDecimal Create(Transform transform = null) => new PropertyGetDecimal(
            new GetDecimalTransformsChildCount(transform)
        );

        public override string String => $"{this.m_Transform} Child Count";
    }
}