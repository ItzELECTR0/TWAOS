using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Description("Sets the Color value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetColorLocalName : PropertyTypeSetColor
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueColor.TYPE_ID);

        public override void Set(Color value, Args args) => this.m_Variable.Set(value, args);
        public override Color Get(Args args) => (Color) this.m_Variable.Get(args);

        public static PropertySetColor Create => new PropertySetColor(
            new SetColorLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}