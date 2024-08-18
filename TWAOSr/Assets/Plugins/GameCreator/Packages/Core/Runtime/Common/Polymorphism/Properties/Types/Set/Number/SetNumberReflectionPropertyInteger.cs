using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Integer")]
    [Category("Reflection/Property Integer")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'integer' value of a property of a component")]

    [Serializable]
    public class SetNumberReflectionPropertyInteger : PropertyTypeSetNumber
    {
        [SerializeField] private ReflectionPropertyInteger m_Property = new ReflectionPropertyInteger();

        public override void Set(double value, Args args) => this.m_Property.Value = (int) value;
        public override double Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}