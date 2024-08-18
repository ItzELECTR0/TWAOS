using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Change Image")]
    [Category("UI/Change Image")]
    
    [Image(typeof(IconUIImage), ColorTheme.Type.TextLight)]
    [Description("Changes the Sprite of an Image component")]

    [Parameter("Override Sprite", "If the Sprite replaced is the original or the overriden")]
    [Parameter("Image", "The Image component that changes its sprite value")]
    [Parameter("Sprite", "The new Sprite reference")]

    [Serializable]
    public class InstructionUIChangeImage : Instruction
    {
        [SerializeField] private bool m_OverrideSprite = true;
        [SerializeField] private PropertyGetGameObject m_Image = GetGameObjectInstance.Create();
        [SerializeField] private PropertyGetSprite m_Sprite = GetSpriteInstance.Create();

        public override string Title => $"Image {this.m_Image} = {this.m_Sprite}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Image.Get(args);
            if (gameObject == null) return DefaultResult;

            Image image = gameObject.Get<Image>();
            if (image == null) return DefaultResult;

            switch (this.m_OverrideSprite)
            {
                case true: image.overrideSprite = this.m_Sprite.Get(args); break;
                case false: image.sprite = this.m_Sprite.Get(args); break;
            }
            
            return DefaultResult;
        }
    }
}