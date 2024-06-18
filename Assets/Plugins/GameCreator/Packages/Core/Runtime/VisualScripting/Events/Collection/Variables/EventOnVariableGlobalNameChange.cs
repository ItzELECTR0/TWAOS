using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Global Name Variable Change")]
    [Category("Variables/On Global Name Variable Change")]
    [Description("Executed when the Global Name Variable is modified")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class EventOnVariableGlobalNameChange : Event
    {
        [SerializeField]
        private DetectorGlobalNameVariable m_Variable = new DetectorGlobalNameVariable();
        
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