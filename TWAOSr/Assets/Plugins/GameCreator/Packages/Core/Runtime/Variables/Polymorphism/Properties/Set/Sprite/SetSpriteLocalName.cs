using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Description("Sets the Sprite value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetSpriteLocalName : PropertyTypeSetSprite
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueSprite.TYPE_ID);

        public override void Set(Sprite value, Args args) => this.m_Variable.Set(value, args);
        public override Sprite Get(Args args) => this.m_Variable.Get(args) as Sprite;

        public static PropertySetSprite Create => new PropertySetSprite(
            new SetSpriteLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}