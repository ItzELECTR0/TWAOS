using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Texture")]
    [Category("Reflection/Property Texture")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Texture' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetTextureReflectionPropertyTexture : PropertyTypeGetTexture
    {
        [SerializeField] private ReflectionPropertyTexture m_Property = new ReflectionPropertyTexture();

        public override Texture Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}