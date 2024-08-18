using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Description("Sets the string value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetStringGlobalName : PropertyTypeSetString
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueString.TYPE_ID);

        public override void Set(string value, Args args) => this.m_Variable.Set(value, args);
        public override string Get(Args args) => this.m_Variable.Get(args).ToString();

        public static PropertySetString Create => new PropertySetString(
            new SetStringGlobalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}