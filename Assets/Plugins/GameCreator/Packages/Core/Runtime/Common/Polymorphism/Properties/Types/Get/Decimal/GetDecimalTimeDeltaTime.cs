using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Delta Time")]
    [Category("Time/Delta Time")]
    
    [Image(typeof(IconTimer), ColorTheme.Type.Blue)]
    [Description("The amount of seconds elapsed since the completion of the last frame")]

    [Keywords("Float", "Decimal", "Double", "Frame", "Increment")]
    [Serializable]
    public class GetDecimalTimeDeltaTime : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Time.deltaTime;
        public override double Get(GameObject gameObject) => Time.deltaTime;
        
        public GetDecimalTimeDeltaTime() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalTimeDeltaTime());

        public override string String => "Delta Time";
    }
}