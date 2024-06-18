using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Mobile")]
    [Description("Returns true if the running platform is a smartphone or tablet")]

    [Category("Platforms/Is Mobile")]

    [Keywords("Smartphone", "Tablet", "iOS", "Android")]
    
    [Image(typeof(IconMobile), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionPlatformIsMobile : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "is Mobile";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Application.isMobilePlatform;
        }
    }
}
