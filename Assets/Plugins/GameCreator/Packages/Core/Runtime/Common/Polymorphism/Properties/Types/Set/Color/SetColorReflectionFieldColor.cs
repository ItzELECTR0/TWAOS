using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Color")]
    [Category("Reflection/Field Color")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Color' value of a public or private field of a component")]

    [Serializable]
    public class SetColorReflectionFieldColor : PropertyTypeSetColor
    {
        [SerializeField] private ReflectionFieldColor m_Field = new ReflectionFieldColor();

        public override void Set(Color value, Args args) => this.m_Field.Value = value;
        public override Color Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}