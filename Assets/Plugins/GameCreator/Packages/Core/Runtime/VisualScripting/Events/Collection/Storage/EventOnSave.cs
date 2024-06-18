using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Save")]
    [Category("Storage/On Save")]
    [Description("Executed when the game is saved")]

    [Image(typeof(IconDiskSolid), ColorTheme.Type.Green)]
    [Keywords("Load", "Save", "Profile", "Slot", "Game", "Session")]
    
    [Serializable]
    public class EventOnSave : Event
    {
        private enum Option
        {
            BeforeSaving,
            AfterSaving
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Option m_When = Option.BeforeSaving;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            SaveLoadManager.Instance.EventBeforeSave += this.OnBeforeSave;
            SaveLoadManager.Instance.EventAfterSave += this.OnAfterSave;
        }
        
        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            if (ApplicationManager.IsExiting) return;
            
            SaveLoadManager.Instance.EventBeforeSave -= this.OnBeforeSave;
            SaveLoadManager.Instance.EventAfterSave -= this.OnAfterSave;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnBeforeSave(int obj)
        {
            if (this.m_When == Option.BeforeSaving)
            {
                _ = this.m_Trigger.Execute(this.Self);   
            }
        }
        
        private void OnAfterSave(int obj)
        {
            if (this.m_When == Option.AfterSaving)
            {
                _ = this.m_Trigger.Execute(this.Self);   
            }
        }
    }
}