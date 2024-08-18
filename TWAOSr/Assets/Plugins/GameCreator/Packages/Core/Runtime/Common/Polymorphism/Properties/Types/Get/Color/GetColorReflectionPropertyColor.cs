using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Color")]
    [Category("Reflection/Property Color")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Color' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetColorReflectionPropertyColor : PropertyTypeGetColor
    {
        [SerializeField] private ReflectionPropertyColor m_Property = new ReflectionPropertyColor();

        public override Color Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}