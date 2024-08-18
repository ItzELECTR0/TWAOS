using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the decimal value of a Global List Variable")]

    [Serializable]
    public class GetDecimalGlobalList : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueNumber.TYPE_ID);

        public override double Get(Args args) => this.m_Variable.Get<double>(args);

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}