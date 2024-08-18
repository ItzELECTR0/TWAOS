using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Color")]
    [Category("Reflection/Property Color")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Color' value of a property of a component")]

    [Serializable]
    public class SetColorReflectionPropertyColor : PropertyTypeSetColor
    {
        [SerializeField] private ReflectionPropertyColor m_Property = new ReflectionPropertyColor();

        public override void Set(Color value, Args args) => this.m_Property.Value = value;
        public override Color Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}