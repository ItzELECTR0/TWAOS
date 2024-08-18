using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Has Save at Slot")]
    [Description("Returns true if there is a saved game at the specified slot")]

    [Category("Storage/Has Save at Slot")]

    [Keywords("Game", "Load", "Continue", "Resume", "Can", "Is")]
    
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Green, typeof(OverlayDot))]
    [Serializable]
    public class ConditionHasSaveAtSlot : Condition
    {
        [SerializeField] private PropertyGetInteger m_Slot = GetDecimalInteger.Create(1);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"has Saved Game at {this.m_Slot}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            int slot = (int) this.m_Slot.Get(args);
            return SaveLoadManager.Instance.HasSaveAt(slot);
        }
    }
}
