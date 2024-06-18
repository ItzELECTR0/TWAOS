using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sprite Renderer")]
    [Category("Game Objects/Sprite Renderer")]

    [Image(typeof(IconSprite), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("The Sprite value attached to the Sprite Renderer")]

    [Serializable]
    public class SetSpriteSpriteRenderer : PropertyTypeSetSprite
    {
        [SerializeField]
        private PropertyGetGameObject m_SpriteRenderer = GetGameObjectInstance.Create();

        public override void Set(Sprite value, Args args)
        {
            SpriteRenderer spriteRenderer = this.m_SpriteRenderer.Get<SpriteRenderer>(args);
            if (spriteRenderer == null) return;

            spriteRenderer.sprite = value;
        }

        public override Sprite Get(Args args)
        {
            SpriteRenderer spriteRenderer = this.m_SpriteRenderer.Get<SpriteRenderer>(args);
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }

        public override string String => $"{this.m_SpriteRenderer} Sprite";
    }
}