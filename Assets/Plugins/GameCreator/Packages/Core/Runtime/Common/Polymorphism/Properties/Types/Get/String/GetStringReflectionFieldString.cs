using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field String")]
    [Category("Reflection/Field String")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'string' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetStringReflectionFieldString : PropertyTypeGetString
    {
        [SerializeField] private ReflectionFieldString m_Field = new ReflectionFieldString();

        public override string Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}