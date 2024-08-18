using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Description("Sets the Sprite value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetSpriteGlobalList : PropertyTypeSetSprite
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueSprite.TYPE_ID);

        public override void Set(Sprite value, Args args) => this.m_Variable.Set(value, args);
        public override Sprite Get(Args args) => this.m_Variable.Get(args) as Sprite;

        public static PropertySetSprite Create => new PropertySetSprite(
            new SetSpriteGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}