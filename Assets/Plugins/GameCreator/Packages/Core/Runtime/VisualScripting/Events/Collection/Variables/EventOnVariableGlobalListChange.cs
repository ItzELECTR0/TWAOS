using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Global List Variable Change")]
    [Category("Variables/On Global List Variable Change")]
    [Description("Executed when the Global List Variable is modified")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class EventOnVariableGlobalListChange : Event
    {
        [SerializeField]
        private DetectorGlobalListVariable m_Variable = new DetectorGlobalListVariable();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Args m_Args;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            this.m_Args = new Args(trigger);
        }

        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            this.m_Variable.StartListening(this.OnChange, this.m_Args);
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            this.m_Variable.StopListening(this.OnChange, this.m_Args);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}