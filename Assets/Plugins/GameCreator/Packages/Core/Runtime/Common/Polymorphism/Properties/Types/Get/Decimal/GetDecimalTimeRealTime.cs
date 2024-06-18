using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Real Time")]
    [Category("Time/Real Time")]
    
    [Image(typeof(IconTimer), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    [Description("The unscaled amount of seconds since the application started running")]

    [Keywords("Float", "Decimal", "Double", "Elapsed", "Unscaled")]
    [Serializable]
    public class GetDecimalTimeRealTime : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Time.unscaledTime;
        public override double Get(GameObject gameObject) => Time.unscaledTime;
        
        public GetDecimalTimeRealTime() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalTimeRealTime());

        public override string String => "Real Time";
    }
}