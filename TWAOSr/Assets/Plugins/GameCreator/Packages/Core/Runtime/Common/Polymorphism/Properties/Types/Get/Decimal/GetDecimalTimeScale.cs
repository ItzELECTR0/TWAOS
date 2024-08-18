using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Time Scale")]
    [Category("Time/Time Scale")]
    
    [Image(typeof(IconTimer), ColorTheme.Type.Green)]
    [Description("The scale at which time passes")]

    [Keywords("Float", "Decimal", "Double", "Slow", "Fast", "Pause", "Freeze")]
    [Serializable]
    public class GetDecimalTimeScale : PropertyTypeGetDecimal
    {
        public override double Get(Args args) => Time.timeScale;
        public override double Get(GameObject gameObject) => Time.timeScale;
        
        public GetDecimalTimeScale() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(new GetDecimalTimeScale());

        public override string String => "Time Scale";
    }
}