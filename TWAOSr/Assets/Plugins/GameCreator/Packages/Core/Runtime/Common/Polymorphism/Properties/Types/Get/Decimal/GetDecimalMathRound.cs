using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Round Decimal")]
    [Category("Math/Arithmetic/Round Decimal")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.TextNormal, typeof(OverlayDot))]
    [Description("The closest integer part of a decimal value")]

    [Keywords("Float", "Decimal", "Double")]
    
    [Serializable]
    public class GetDecimalMathRound : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number = new PropertyGetDecimal();

        public override double Get(Args args) => Math.Round(this.m_Number.Get(args));
        public override double Get(GameObject gameObject) => Math.Round(this.m_Number.Get(gameObject));

        public override string String => $"Round {this.m_Number}";
        
        public override double EditorValue => Math.Round(this.m_Number.EditorValue);
    }
}