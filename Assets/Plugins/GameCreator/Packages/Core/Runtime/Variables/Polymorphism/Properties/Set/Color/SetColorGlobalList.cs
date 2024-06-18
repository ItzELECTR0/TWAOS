using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Description("Sets the Color value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetColorGlobalList : PropertyTypeSetColor
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueColor.TYPE_ID);

        public override void Set(Color value, Args args) => this.m_Variable.Set(value, args);
        public override Color Get(Args args) => (Color) this.m_Variable.Get(args);

        public static PropertySetColor Create => new PropertySetColor(
            new SetColorGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}