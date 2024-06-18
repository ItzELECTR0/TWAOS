using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Integer")]
    [Category("Reflection/Field Integer")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'integer' value of a public or private field of a component")]

    [Serializable]
    public class SetNumberReflectionFieldInteger : PropertyTypeSetNumber
    {
        [SerializeField] private ReflectionFieldInteger m_Field = new ReflectionFieldInteger();

        public override void Set(double value, Args args) => this.m_Field.Value = (int) value;
        public override double Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}