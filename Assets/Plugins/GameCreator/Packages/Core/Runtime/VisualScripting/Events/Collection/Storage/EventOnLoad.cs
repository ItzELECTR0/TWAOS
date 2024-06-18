using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Load")]
    [Category("Storage/On Load")]
    [Description("Executed when a previously saved game is loaded")]

    [Image(typeof(IconDiskSolid), ColorTheme.Type.Blue)]
    [Keywords("Load", "Save", "Profile", "Slot", "Game", "Session")]

    [Serializable]
    public class EventOnLoad : Event
    {
        private enum Option
        {
            BeforeLoading,
            AfterLoading
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Option m_When = Option.AfterLoading;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            SaveLoadManager.Instance.EventBeforeLoad += this.OnBeforeLoad;
            SaveLoadManager.Instance.EventAfterLoad += this.OnAfterLoad;
        }
        
        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            if (ApplicationManager.IsExiting) return;
            
            SaveLoadManager.Instance.EventBeforeLoad -= this.OnBeforeLoad;
            SaveLoadManager.Instance.EventAfterLoad -= this.OnAfterLoad;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnBeforeLoad(int obj)
        {
            if (this.m_When == Option.BeforeLoading)
            {
                _ = this.m_Trigger.Execute(this.Self);   
            }
        }
        
        private void OnAfterLoad(int obj)
        {
            if (this.m_When == Option.AfterLoading)
            {
                _ = this.m_Trigger.Execute(this.Self);   
            }
        }
    }
}