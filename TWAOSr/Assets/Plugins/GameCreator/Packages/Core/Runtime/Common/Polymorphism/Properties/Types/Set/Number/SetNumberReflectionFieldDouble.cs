using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Double")]
    [Category("Reflection/Field Double")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'double' value of a public or private field of a component")]

    [Serializable]
    public class SetNumberReflectionFieldDouble : PropertyTypeSetNumber
    {
        [SerializeField] private ReflectionFieldDouble m_Field = new ReflectionFieldDouble();

        public override void Set(double value, Args args) => this.m_Field.Value = value;
        public override double Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}