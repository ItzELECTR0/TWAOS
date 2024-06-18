using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Sprite")]
    [Category("Reflection/Property Sprite")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Sprite' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetSpriteReflectionPropertySprite : PropertyTypeGetSprite
    {
        [SerializeField] private ReflectionPropertySprite m_Property = new ReflectionPropertySprite();

        public override Sprite Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}