using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Description("Sets the boolean value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetBoolLocalList : PropertyTypeSetBool
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueBool.TYPE_ID);

        public override void Set(bool value, Args args) => this.m_Variable.Set(value, args);
        public override bool Get(Args args) => (bool) this.m_Variable.Get(args);

        public static PropertySetBool Create => new PropertySetBool(
            new SetBoolLocalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}