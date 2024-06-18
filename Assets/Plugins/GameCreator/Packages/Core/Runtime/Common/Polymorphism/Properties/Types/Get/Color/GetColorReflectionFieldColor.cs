using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Color")]
    [Category("Reflection/Field Color")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Color' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetColorReflectionFieldColor : PropertyTypeGetColor
    {
        [SerializeField] private ReflectionFieldColor m_Field = new ReflectionFieldColor();

        public override Color Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}