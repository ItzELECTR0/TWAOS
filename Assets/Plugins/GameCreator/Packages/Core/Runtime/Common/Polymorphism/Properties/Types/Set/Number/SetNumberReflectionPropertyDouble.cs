using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Double")]
    [Category("Reflection/Property Double")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'double' value of a property of a component")]

    [Serializable]
    public class SetNumberReflectionPropertyDouble : PropertyTypeSetNumber
    {
        [SerializeField] private ReflectionPropertyDouble m_Property = new ReflectionPropertyDouble();

        public override void Set(double value, Args args) => this.m_Property.Value = value;
        public override double Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}