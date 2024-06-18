using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Description("Sets the boolean value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetBoolGlobalList : PropertyTypeSetBool
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueBool.TYPE_ID);

        public override void Set(bool value, Args args) => this.m_Variable.Set(value, args);
        public override bool Get(Args args) => (bool) this.m_Variable.Get(args);

        public static PropertySetBool Create => new PropertySetBool(
            new SetBoolGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}