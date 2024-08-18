using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Float")]
    [Category("Reflection/Property Float")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'float' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetDecimalReflectionPropertyFloat : PropertyTypeGetDecimal
    {
        [SerializeField] private ReflectionPropertyFloat m_Property = new ReflectionPropertyFloat();

        public override double Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}