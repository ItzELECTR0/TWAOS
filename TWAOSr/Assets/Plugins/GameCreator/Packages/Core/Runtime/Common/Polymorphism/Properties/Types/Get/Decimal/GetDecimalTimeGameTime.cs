using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Time")]
    [Category("Time/Game Time")]
    
    [Image(typeof(IconTimer), ColorTheme.Type.Yellow)]
    [Description("The internal amount of seconds since the application started running")]

    [Keywords("Float", "Decimal", "Double", "Elapsed")]
    [Serializable]
    public class GetDecimalTimeGameTime : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Time.time;
        public override double Get(GameObject gameObject) => Time.time;
        
        public GetDecimalTimeGameTime() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalTimeGameTime());

        public override string String => "Game Time";
    }
}