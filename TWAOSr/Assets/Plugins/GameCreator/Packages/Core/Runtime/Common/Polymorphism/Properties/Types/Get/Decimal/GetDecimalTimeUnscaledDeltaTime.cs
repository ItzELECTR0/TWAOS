using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Real Delta Time")]
    [Category("Time/Real Delta Time")]
    
    [Image(typeof(IconTimer), ColorTheme.Type.Blue, typeof(OverlayDot))]
    [Description("The amount of seconds elapsed since the completion of the last frame without the time scale")]

    [Keywords("Float", "Decimal", "Double", "Frame", "Increment")]
    [Serializable]
    public class GetDecimalTimeUnscaledDeltaTime : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Time.unscaledDeltaTime;
        public override double Get(GameObject gameObject) => Time.unscaledDeltaTime;
        
        public GetDecimalTimeUnscaledDeltaTime() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalTimeUnscaledDeltaTime());

        public override string String => "Real Delta Time";
    }
}