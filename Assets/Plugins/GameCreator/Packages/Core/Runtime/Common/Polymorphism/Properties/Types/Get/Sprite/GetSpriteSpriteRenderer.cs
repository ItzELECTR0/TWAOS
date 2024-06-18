using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sprite Renderer")]
    [Category("Game Objects/Sprite Renderer")]
    
    [Image(typeof(IconSprite), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("A reference to a Sprite Renderer's Sprite value")]

    [Keywords("Sprite", "2D")]
    
    [Serializable]
    public class GetSpriteSpriteRenderer : PropertyTypeGetSprite
    {
        [SerializeField]
        private PropertyGetGameObject m_SpriteRenderer = GetGameObjectInstance.Create();

        public override Sprite Get(Args args)
        {
            SpriteRenderer spriteRenderer = this.m_SpriteRenderer.Get<SpriteRenderer>(args);
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }

        public override Sprite Get(GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = this.m_SpriteRenderer.Get<SpriteRenderer>(gameObject);
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }

        public static PropertyGetSprite Create() => new PropertyGetSprite(
            new GetSpriteSpriteRenderer()
        );

        public override string String => this.m_SpriteRenderer.ToString();

        public override Sprite EditorValue
        {
            get
            {
                GameObject gameObject = this.m_SpriteRenderer.EditorValue;
                if (gameObject == null) return null;

                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                return spriteRenderer != null ? spriteRenderer.sprite : null;
            }
        }
    }
}