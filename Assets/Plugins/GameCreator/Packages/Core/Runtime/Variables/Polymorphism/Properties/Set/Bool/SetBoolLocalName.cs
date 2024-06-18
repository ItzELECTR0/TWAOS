using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Description("Sets the boolean value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetBoolLocalName : PropertyTypeSetBool
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueBool.TYPE_ID);

        public override void Set(bool value, Args args) => this.m_Variable.Set(value, args);
        public override bool Get(Args args) => (bool) this.m_Variable.Get(args);

        public static PropertySetBool Create => new PropertySetBool(
            new SetBoolLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}