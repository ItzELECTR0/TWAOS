using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Quaternion")]
    [Category("Reflection/Field Quaternion")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Quaternion' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetRotationReflectionFieldQuaternion : PropertyTypeGetRotation
    {
        [SerializeField] private ReflectionFieldQuaternion m_Field = new ReflectionFieldQuaternion();

        public override Quaternion Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}