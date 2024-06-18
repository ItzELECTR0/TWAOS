using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Local Name Variable Change")]
    [Category("Variables/On Local Name Variable Change")]
    [Description("Executed when the Local Name Variable is modified")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class EventOnVariableLocalNameChange : Event
    {
        [SerializeField]
        private DetectorLocalNameVariable m_Variable = new DetectorLocalNameVariable();
        
        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            this.m_Variable.StartListening(this.OnChange);
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            this.m_Variable.StopListening(this.OnChange);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange(string name)
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}