using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Interval")]
    [Category("Lifecycle/On Interval")]
    [Description("Executes after an amount of seconds have passed between each call")]

    [Parameter("Time Mode", "The time scale in which the interval is calculated")]
    [Parameter("Interval", "Amount of seconds between each iteration")]
    
    [Image(typeof(IconLoop), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Keywords("Loop", "Tick", "Continuous", "FPS")]

    [Serializable]
    public class EventOnInterval : Event
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private TimeMode m_TimeMode = new TimeMode(TimeMode.UpdateMode.GameTime);
        [SerializeField] private PropertyGetDecimal m_Interval = new PropertyGetDecimal(1f);

        // MEMBERS: -------------------------------------------------------------------------------
        
        private double m_NextInterval = double.MinValue;

        // METHODS: -------------------------------------------------------------------------------
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            this.m_NextInterval = this.m_TimeMode.Time + this.m_Interval.Get(trigger.gameObject);
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            this.m_NextInterval = double.MinValue;
        }

        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            
            if (this.m_TimeMode.Time < this.m_NextInterval) return;
            
            this.m_NextInterval = this.m_TimeMode.Time + this.m_Interval.Get(trigger.gameObject);
            _ = trigger.Execute(this.Self);
        }
    }
}