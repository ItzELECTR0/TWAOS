using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Bool")]
    [Category("Reflection/Field Bool")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'boolean' value of a public or private field of a component")]

    [Serializable]
    public class SetBoolReflectionFieldBool : PropertyTypeSetBool
    {
        [SerializeField] private ReflectionFieldBool m_Field = new ReflectionFieldBool();

        public override void Set(bool value, Args args) => this.m_Field.Value = value;
        public override bool Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}