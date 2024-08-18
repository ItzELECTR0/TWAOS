using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Quaternion")]
    [Category("Reflection/Property Quaternion")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Quaternion' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetRotationReflectionPropertyQuaternion : PropertyTypeGetRotation
    {
        [SerializeField] private ReflectionPropertyQuaternion m_Property = new ReflectionPropertyQuaternion();

        public override Quaternion Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}