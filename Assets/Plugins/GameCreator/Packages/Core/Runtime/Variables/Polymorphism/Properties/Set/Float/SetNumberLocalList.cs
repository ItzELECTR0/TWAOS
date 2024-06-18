using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Description("Sets the numeric value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetNumberLocalList : PropertyTypeSetNumber
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueNumber.TYPE_ID);

        public override void Set(double value, Args args) => this.m_Variable.Set(value, args);
        public override double Get(Args args) => (double) this.m_Variable.Get(args);

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberLocalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}