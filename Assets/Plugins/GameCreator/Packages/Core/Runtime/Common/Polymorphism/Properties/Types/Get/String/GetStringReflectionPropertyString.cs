using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property String")]
    [Category("Reflection/Property String")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'string' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetStringReflectionPropertyString : PropertyTypeGetString
    {
        [SerializeField] private ReflectionPropertyString m_Property = new ReflectionPropertyString();

        public override string Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}