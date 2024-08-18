using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field GameObject")]
    [Category("Reflection/Field GameObject")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'GameObject' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetGameObjectReflectionFieldObject : PropertyTypeGetGameObject
    {
        [SerializeField] private ReflectionFieldGameObject m_Field = new ReflectionFieldGameObject();

        public override GameObject Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}