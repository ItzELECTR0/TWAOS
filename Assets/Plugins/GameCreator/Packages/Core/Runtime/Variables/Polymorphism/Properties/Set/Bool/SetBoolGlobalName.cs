using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Description("Sets the boolean value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetBoolGlobalName : PropertyTypeSetBool
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueBool.TYPE_ID);

        public override void Set(bool value, Args args) => this.m_Variable.Set(value, args);
        public override bool Get(Args args) => (bool) this.m_Variable.Get(args);

        public static PropertySetBool Create => new PropertySetBool(
            new SetBoolGlobalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}