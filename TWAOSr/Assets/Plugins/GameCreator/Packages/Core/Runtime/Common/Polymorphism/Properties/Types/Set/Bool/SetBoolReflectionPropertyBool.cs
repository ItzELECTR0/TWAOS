using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Bool")]
    [Category("Reflection/Property Bool")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'boolean' value of a property of a component")]

    [Serializable]
    public class SetBoolReflectionPropertyBool : PropertyTypeSetBool
    {
        [SerializeField] private ReflectionPropertyBool m_Property = new ReflectionPropertyBool();

        public override void Set(bool value, Args args) => this.m_Property.Value = value;
        public override bool Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}