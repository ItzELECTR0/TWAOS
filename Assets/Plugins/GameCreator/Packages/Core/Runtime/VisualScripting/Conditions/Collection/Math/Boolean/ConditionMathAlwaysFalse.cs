using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Always False")]
    [Description("Always returns false")]

    [Category("Math/Boolean/Always False")]
    
    [Keywords("Boolean", "No", "Contradiction")]
    
    [Image(typeof(IconToggleOff), ColorTheme.Type.Red)]
    [Serializable]
    public class ConditionMathAlwaysFalse : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "False";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override bool Run(Args args) => false;
    }
}
