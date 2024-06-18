using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Float")]
    [Category("Reflection/Field Float")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'float' value of a public or private field of a component")]

    [Serializable]
    public class SetNumberReflectionFieldFloat : PropertyTypeSetNumber
    {
        [SerializeField] private ReflectionFieldFloat m_Field = new ReflectionFieldFloat();

        public override void Set(double value, Args args) => this.m_Field.Value = (float) value;
        public override double Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}