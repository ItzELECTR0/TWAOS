using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Count of Global List Variable")]
    [Category("Variables/Count of Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the amount of elements of a Global List Variable")]

    [Serializable]
    public class GetDecimalGlobalListLength : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected GlobalListVariables m_Variable;

        public override double Get(Args args) => this.m_Variable != null 
            ? this.m_Variable.Count
            : 0;
        
        public override double Get(GameObject gameObject) => this.m_Variable != null 
            ? this.m_Variable.Count
            : 0;

        public override string String => this.m_Variable != null
            ? this.m_Variable.name + " Length"
            : "(none)";
    }
}