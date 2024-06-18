using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property GameObject")]
    [Category("Reflection/Property GameObject")]

    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'GameObject' value of a property of a component")]

    [Serializable]
    public class SetGameObjectReflectionPropertyGameObject : PropertyTypeSetGameObject
    {
        [SerializeField] private ReflectionPropertyGameObject m_Property = new ReflectionPropertyGameObject();

        public override void Set(GameObject value, Args args) => this.m_Property.Value = value;
        public override GameObject Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}