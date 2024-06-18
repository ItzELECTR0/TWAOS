using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Description("Sets the string value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetStringLocalName : PropertyTypeSetString
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueString.TYPE_ID);

        public override void Set(string value, Args args) => this.m_Variable.Set(value, args);
        public override string Get(Args args) => this.m_Variable.Get(args).ToString();

        public static PropertySetString Create => new PropertySetString(
            new SetStringLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}