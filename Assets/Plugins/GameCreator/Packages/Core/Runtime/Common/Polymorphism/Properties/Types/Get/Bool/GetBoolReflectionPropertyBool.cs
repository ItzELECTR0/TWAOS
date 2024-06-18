using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Bool")]
    [Category("Reflection/Property Bool")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'boolean' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetBoolReflectionPropertyBool : PropertyTypeGetBool
    {
        [SerializeField] private ReflectionPropertyBool m_Property = new ReflectionPropertyBool();

        public override bool Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}