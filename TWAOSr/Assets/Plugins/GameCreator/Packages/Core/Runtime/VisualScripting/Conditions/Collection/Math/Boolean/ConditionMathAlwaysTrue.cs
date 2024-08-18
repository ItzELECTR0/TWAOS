using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Always True")]
    [Description("Always returns true")]

    [Category("Math/Boolean/Always True")]
    
    [Keywords("Boolean", "Yes", "Tautology")]
    
    [Image(typeof(IconToggleOn), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionMathAlwaysTrue : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "True";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override bool Run(Args args) => true;
    }
}
