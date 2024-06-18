using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Has Save")]
    [Description("Returns true if there is at least one saved game")]

    [Category("Storage/Has Save")]

    [Keywords("Game", "Load", "Continue", "Resume", "Can", "Is")]
    
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionHasSave : Condition
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => "has Saved Game";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return SaveLoadManager.Instance.HasSave();
        }
    }
}
