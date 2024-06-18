using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Description("Sets the Color value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetColorLocalList : PropertyTypeSetColor
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueColor.TYPE_ID);

        public override void Set(Color value, Args args) => this.m_Variable.Set(value, args);
        public override Color Get(Args args) => (Color) this.m_Variable.Get(args);

        public static PropertySetColor Create => new PropertySetColor(
            new SetColorLocalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}