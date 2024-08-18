using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Description("Sets the Color value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetColorGlobalName : PropertyTypeSetColor
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueColor.TYPE_ID);

        public override void Set(Color value, Args args) => this.m_Variable.Set(value, args);
        public override Color Get(Args args) => (Color) this.m_Variable.Get(args);

        public static PropertySetColor Create => new PropertySetColor(
            new SetColorGlobalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}