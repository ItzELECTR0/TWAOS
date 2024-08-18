using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Vector3")]
    [Category("Reflection/Field Vector3")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Vector3' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetScaleReflectionFieldVector3 : PropertyTypeGetScale
    {
        [SerializeField] private ReflectionFieldVector3 m_Field = new ReflectionFieldVector3();

        public override Vector3 Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}