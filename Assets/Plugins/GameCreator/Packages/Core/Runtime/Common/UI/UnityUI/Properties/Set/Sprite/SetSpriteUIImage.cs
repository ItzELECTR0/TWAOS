using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Image")]
    [Category("UI/Image")]
    
    [Description("Sets the Image's sprite value")]
    [Image(typeof(IconUIImage), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetSpriteUIImage : PropertyTypeSetSprite
    {
        [SerializeField] protected bool m_OverrideSprite = true;
        [SerializeField] private PropertyGetGameObject m_Image = GetGameObjectInstance.Create();

        public override void Set(Sprite value, Args args)
        {
            GameObject gameObject = this.m_Image.Get(args);
            if (gameObject == null) return;

            Image image = gameObject.Get<Image>();
            if (image == null) return;

            switch (this.m_OverrideSprite)
            {
                case true: image.overrideSprite = value; break;
                case false: image.sprite = value; break;
            }
        }

        public override Sprite Get(Args args)
        {
            GameObject gameObject = this.m_Image.Get(args);
            if (gameObject == null) return default;

            Image image = gameObject.Get<Image>();
            if (image == null) return null;
            
            return this.m_OverrideSprite switch
            {
                true => image.overrideSprite,
                false => image.sprite
            };
        }

        public static PropertySetSprite Create => new PropertySetSprite(
            new SetSpriteUIImage()
        );
        
        public override string String => this.m_Image.ToString();
    }
}