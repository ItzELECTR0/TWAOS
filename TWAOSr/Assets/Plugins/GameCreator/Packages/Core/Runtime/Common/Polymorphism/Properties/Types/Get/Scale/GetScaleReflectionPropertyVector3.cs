using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Vector3")]
    [Category("Reflection/Property Vector3")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Vector3' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetScaleReflectionPropertyVector3 : PropertyTypeGetScale
    {
        [SerializeField] private ReflectionPropertyVector3 m_Property = new ReflectionPropertyVector3();

        public override Vector3 Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}