using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Sprite")]
    [Category("Reflection/Field Sprite")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Sprite' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetSpriteReflectionFieldSprite : PropertyTypeGetSprite
    {
        [SerializeField] private ReflectionFieldSprite m_Field = new ReflectionFieldSprite();

        public override Sprite Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}