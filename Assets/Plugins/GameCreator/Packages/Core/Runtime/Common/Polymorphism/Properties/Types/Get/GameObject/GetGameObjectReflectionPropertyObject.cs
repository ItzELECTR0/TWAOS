using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property GameObject")]
    [Category("Reflection/Property GameObject")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'GameObject' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetGameObjectReflectionPropertyObject : PropertyTypeGetGameObject
    {
        [SerializeField] private ReflectionPropertyGameObject m_Property = new ReflectionPropertyGameObject();

        public override GameObject Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}