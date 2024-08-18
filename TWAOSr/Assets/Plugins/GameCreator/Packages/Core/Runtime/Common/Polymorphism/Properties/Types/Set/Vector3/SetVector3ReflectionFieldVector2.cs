using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Vector2")]
    [Category("Reflection/Field Vector2")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Vector2' value of a public or private field of a component")]

    [Serializable]
    public class SetVector3ReflectionFieldVector2 : PropertyTypeSetVector3
    {
        [SerializeField] private ReflectionFieldVector2 m_Field = new ReflectionFieldVector2();

        public override void Set(Vector3 value, Args args) => this.m_Field.Value = value;
        public override Vector3 Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}