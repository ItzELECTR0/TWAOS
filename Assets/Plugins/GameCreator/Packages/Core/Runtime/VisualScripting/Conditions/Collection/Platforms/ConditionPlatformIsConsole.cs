using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Console")]
    [Description("Returns true if the running platform is a console")]

    [Category("Platforms/Is Console")]

    [Keywords("PS4", "PS5", "Switch", "XBox", "Deck")]
    
    [Image(typeof(IconConsole), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionPlatformIsConsole : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "is Console";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Application.isConsolePlatform;
        }
    }
}
