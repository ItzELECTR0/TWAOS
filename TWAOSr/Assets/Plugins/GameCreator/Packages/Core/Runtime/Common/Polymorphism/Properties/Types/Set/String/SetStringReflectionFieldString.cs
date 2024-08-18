using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field String")]
    [Category("Reflection/Field String")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'string' value of a public or private field of a component")]

    [Serializable]
    public class SetStringReflectionFieldString : PropertyTypeSetString
    {
        [SerializeField] private ReflectionFieldString m_Field = new ReflectionFieldString();

        public override void Set(string value, Args args) => this.m_Field.Value = value;
        public override string Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}