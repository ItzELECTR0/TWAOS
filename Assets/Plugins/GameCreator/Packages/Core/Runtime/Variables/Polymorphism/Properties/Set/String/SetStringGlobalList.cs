using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Description("Sets the String value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetStringGlobalList : PropertyTypeSetString
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueString.TYPE_ID);

        public override void Set(string value, Args args) => this.m_Variable.Set(value, args);
        public override string Get(Args args) => this.m_Variable.Get(args).ToString();

        public static PropertySetString Create => new PropertySetString(
            new SetStringGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}