using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Image")]
    [Category("UI/Image")]
    
    [Image(typeof(IconUIImage), ColorTheme.Type.TextLight)]
    [Description("The Sprite texture of an Image component")]

    [Serializable]
    public class GetSpriteUIImage : PropertyTypeGetSprite
    {
        [SerializeField] protected bool m_OverrideSprite = true;
        [SerializeField] protected PropertyGetGameObject m_Image = GetGameObjectInstance.Create();

        public override Sprite Get(Args args)
        {
            GameObject gameObject = this.m_Image.Get(args);
            if (gameObject == null) return null;

            Image image = gameObject.Get<Image>();
            if (image == null) return null;

            return this.m_OverrideSprite switch
            {
                true => image.overrideSprite,
                false => image.sprite
            };
        }

        public GetSpriteUIImage() : base()
        { }

        public GetSpriteUIImage(Image image) : this()
        {
            this.m_Image = GetGameObjectInstance.Create(image != null ? image.gameObject : null);
        }

        public static PropertyGetSprite Create(Image image) => new PropertyGetSprite(
            new GetSpriteUIImage(image)
        );

        public override string String => this.m_Image.ToString();
    }
}