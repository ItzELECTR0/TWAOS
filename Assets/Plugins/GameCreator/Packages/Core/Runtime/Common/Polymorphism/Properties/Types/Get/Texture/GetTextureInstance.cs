using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Texture")]
    [Category("Texture")]
    
    [Image(typeof(IconTexture), ColorTheme.Type.Blue)]
    [Description("A reference to a Texture asset")]

    [Serializable] [HideLabelsInEditor]
    public class GetTextureInstance : PropertyTypeGetTexture
    {
        [SerializeField] protected Texture m_Texture;

        public override Texture Get(Args args) => this.m_Texture;
        public override Texture Get(GameObject gameObject) => this.m_Texture;

        public static PropertyGetTexture Create(Texture texture = null)
        {
            GetTextureInstance instance = new GetTextureInstance
            {
                m_Texture = texture
            };
            
            return new PropertyGetTexture(instance);
        }

        public override string String => this.m_Texture != null
            ? this.m_Texture.name
            : "(none)";

        public override Texture EditorValue => this.m_Texture;
    }
}