using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Vector2")]
    [Category("Reflection/Property Vector2")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Vector2' value of a property of a component")]

    [Serializable]
    public class SetVector3ReflectionPropertyVector2 : PropertyTypeSetVector3
    {
        [SerializeField] private ReflectionPropertyVector2 m_Property = new ReflectionPropertyVector2();

        public override void Set(Vector3 value, Args args) => this.m_Property.Value = value;
        public override Vector3 Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}