using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Texture")]
    [Category("Reflection/Field Texture")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Texture' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetTextureReflectionFieldTexture : PropertyTypeGetTexture
    {
        [SerializeField] private ReflectionFieldTexture m_Field = new ReflectionFieldTexture();

        public override Texture Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}