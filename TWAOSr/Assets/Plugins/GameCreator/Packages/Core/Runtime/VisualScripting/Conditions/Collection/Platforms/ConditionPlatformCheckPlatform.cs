using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Check Platform")]
    [Description("Check if the running platform matches the selected one")]

    [Category("Platforms/Check Platform")]

    [Image(typeof(IconComputer), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionPlatformCheckPlatform : Condition
    {
        [SerializeField] private RuntimePlatform m_Platform = RuntimePlatform.WindowsPlayer;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is {this.m_Platform}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Application.platform == this.m_Platform;
        }
    }
}
