using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Float")]
    [Category("Reflection/Property Float")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'float' value of a property of a component")]

    [Serializable]
    public class SetNumberReflectionPropertyFloat : PropertyTypeSetNumber
    {
        [SerializeField] private ReflectionPropertyFloat m_Property = new ReflectionPropertyFloat();

        public override void Set(double value, Args args) => this.m_Property.Value = (float) value;
        public override double Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}