using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the decimal value of a Local Name Variable")]

    [Serializable]
    public class GetDecimalLocalName : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueNumber.TYPE_ID);

        public override double Get(Args args) => this.m_Variable.Get<double>(args);

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}