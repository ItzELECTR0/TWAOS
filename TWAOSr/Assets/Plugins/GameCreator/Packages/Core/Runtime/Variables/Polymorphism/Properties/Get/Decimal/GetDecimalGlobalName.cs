using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the decimal value of a Global Name Variable")]

    [Serializable]
    public class GetDecimalGlobalName : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueNumber.TYPE_ID);

        public override double Get(Args args) => this.m_Variable.Get<double>(args);

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalGlobalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}