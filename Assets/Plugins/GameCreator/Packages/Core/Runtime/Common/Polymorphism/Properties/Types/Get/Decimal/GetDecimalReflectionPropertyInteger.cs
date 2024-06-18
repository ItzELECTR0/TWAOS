using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Integer")]
    [Category("Reflection/Property Integer")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'integer' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetDecimalReflectionPropertyInteger : PropertyTypeGetDecimal
    {
        [SerializeField] private ReflectionPropertyInteger m_Property = new ReflectionPropertyInteger();

        public override double Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}