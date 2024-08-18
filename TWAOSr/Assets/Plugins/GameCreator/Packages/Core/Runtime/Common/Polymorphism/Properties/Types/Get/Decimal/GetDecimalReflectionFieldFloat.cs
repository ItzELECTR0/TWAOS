using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Float")]
    [Category("Reflection/Field Float")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'float' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetDecimalReflectionFieldFloat : PropertyTypeGetDecimal
    {
        [SerializeField] private ReflectionFieldFloat m_Field = new ReflectionFieldFloat();

        public override double Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}