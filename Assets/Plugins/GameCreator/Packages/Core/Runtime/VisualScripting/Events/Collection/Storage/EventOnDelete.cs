using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Delete")]
    [Category("Storage/On Delete")]
    [Description("Executed when a previously saved game deleted")]

    [Keywords("Load", "Save", "Delete", "Profile", "Slot", "Game", "Session")]
    [Image(typeof(IconDiskOutline), ColorTheme.Type.Red, typeof(OverlayCross))]

    [Serializable]
    public class EventOnDelete : Event
    {
        private enum Option
        {
            BeforeDeleting,
            AfterDeleting
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Option m_When = Option.AfterDeleting;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            SaveLoadManager.Instance.EventBeforeDelete += this.OnBeforeDelete;
            SaveLoadManager.Instance.EventAfterDelete += this.OnAfterDelete;
        }
        
        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            if (ApplicationManager.IsExiting) return;
            
            SaveLoadManager.Instance.EventBeforeDelete -= this.OnBeforeDelete;
            SaveLoadManager.Instance.EventAfterDelete -= this.OnAfterDelete;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnBeforeDelete(int obj)
        {
            if (this.m_When == Option.BeforeDeleting)
            {
                _ = this.m_Trigger.Execute(this.Self);   
            }
        }
        
        private void OnAfterDelete(int obj)
        {
            if (this.m_When == Option.AfterDeleting)
            {
                _ = this.m_Trigger.Execute(this.Self);   
            }
        }
    }
}