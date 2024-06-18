using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Last Child Index")]
    [Category("Transforms/Last Child Index")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]
    [Description("Returns the last child's index of the referenced game object")]

    [Keywords("Float", "Decimal", "Double")]
    [Serializable]
    public class GetDecimalTransformsLastChildIndex : PropertyTypeGetDecimal
    {
        [SerializeField] 
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? Math.Max(0, gameObject.transform.childCount - 1) : 0;
        }

        public GetDecimalTransformsLastChildIndex() : base()
        { }

        public GetDecimalTransformsLastChildIndex(Transform transform) : this()
        {
            GameObject gameObject = transform != null ? transform.gameObject : null;
            this.m_Transform = GetGameObjectInstance.Create(gameObject);
        }

        public static PropertyGetDecimal Create(Transform transform = null) => new PropertyGetDecimal(
            new GetDecimalTransformsLastChildIndex(transform)
        );

        public override string String => $"{this.m_Transform} Last Child Index";
    }
}