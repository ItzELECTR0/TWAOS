using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Vector2")]
    [Category("Reflection/Property Vector2")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Vector2' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetDirectionReflectionPropertyVector2 : PropertyTypeGetDirection
    {
        [SerializeField] private ReflectionPropertyVector2 m_Property = new ReflectionPropertyVector2();

        public override Vector3 Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}