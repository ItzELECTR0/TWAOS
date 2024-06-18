using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Batch mode")]
    [Description("Returns true if the running platform is in batch mode (no interface)")]

    [Category("Platforms/Is Batch mode")]

    [Keywords("Server")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionPlatformIsBatch : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "is Batch mode";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Application.isBatchMode;
        }
    }
}
