using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Description("Sets the numeric value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetNumberGlobalName : PropertyTypeSetNumber
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueNumber.TYPE_ID);

        public override void Set(double value, Args args) => this.m_Variable.Set(value, args);
        public override double Get(Args args) => (double) this.m_Variable.Get(args);

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberGlobalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}