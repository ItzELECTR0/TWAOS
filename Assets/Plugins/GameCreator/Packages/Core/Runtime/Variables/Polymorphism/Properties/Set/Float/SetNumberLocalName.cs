using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Description("Sets the numeric value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetNumberLocalName : PropertyTypeSetNumber
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueNumber.TYPE_ID);

        public override void Set(double value, Args args) => this.m_Variable.Set(value, args);
        public override double Get(Args args) => (double) this.m_Variable.Get(args);

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}