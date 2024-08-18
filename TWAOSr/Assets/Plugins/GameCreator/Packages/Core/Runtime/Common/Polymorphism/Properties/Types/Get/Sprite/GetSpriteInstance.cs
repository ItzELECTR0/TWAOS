using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sprite")]
    [Category("Sprite")]
    
    [Image(typeof(IconSprite), ColorTheme.Type.Purple)]
    [Description("A reference to a Sprite texture")]

    [Keywords("Sprite", "UI", "2D")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetSpriteInstance : PropertyTypeGetSprite
    {
        [SerializeField] protected Sprite m_Sprite;

        public override Sprite Get(Args args) => this.m_Sprite;
        public override Sprite Get(GameObject gameObject) => this.m_Sprite;
        
        public GetSpriteInstance() : base()
        { }

        public GetSpriteInstance(Sprite sprite) : this()
        {
            this.m_Sprite = sprite;
        }

        public static PropertyGetSprite Create(Sprite value = null) => new PropertyGetSprite(
            new GetSpriteInstance(value)
        );

        public override string String => this.m_Sprite.ToString();

        public override Sprite EditorValue => this.m_Sprite;
    }
}