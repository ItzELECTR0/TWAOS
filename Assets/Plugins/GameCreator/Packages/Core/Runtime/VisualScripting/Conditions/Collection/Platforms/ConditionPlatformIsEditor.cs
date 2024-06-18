using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Editor")]
    [Description("Returns true if the running platform is the Unity Editor")]

    [Category("Platforms/Is Editor")]

    [Keywords("Unity")]
    
    [Image(typeof(IconUnity), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionPlatformIsEditor : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "is Editor";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Application.isEditor;
        }
    }
}
