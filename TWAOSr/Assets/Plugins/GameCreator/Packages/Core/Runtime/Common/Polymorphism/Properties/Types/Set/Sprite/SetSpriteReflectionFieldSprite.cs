using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Sprite")]
    [Category("Reflection/Field Sprite")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Sprite' value of a public or private field of a component")]

    [Serializable]
    public class SetSpriteReflectionFieldSprite : PropertyTypeSetSprite
    {
        [SerializeField] private ReflectionFieldSprite m_Field = new ReflectionFieldSprite();

        public override void Set(Sprite value, Args args) => this.m_Field.Value = value;
        public override Sprite Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}