using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameCreator.Runtime.Common
{
    public class ScheduleManager : Singleton<ScheduleManager>
    {
        private class Interval
        {
            // PROPERTIES: ------------------------------------------------------------------------
            
            private Action Action { get; }
            private TimeMode TimeMode { get; }
            
            private float Duration { get; }
            private float LastTime { get; set; }

            public bool CanRun => this.TimeMode.Time >= this.LastTime + this.Duration;
            
            // CONSTRUCTOR: -----------------------------------------------------------------------
            
            public Interval(Action action, float duration, TimeMode.UpdateMode mode)
            {
                this.Action = action;
                this.TimeMode = new TimeMode(mode);

                this.Duration = duration;
                this.Run();
            }
            
            // PUBLIC METHODS: --------------------------------------------------------------------

            public void Run()
            {
                this.Action?.Invoke();
                this.LastTime = this.TimeMode.Time;
            }
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        private static int IntervalCounter;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Dictionary<int, Interval> m_Intervals;
        
        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            this.m_Intervals = new Dictionary<int, Interval>();
        }
        
        // CYCLE METHODS: -------------------------------------------------------------------------

        private void Update()
        {
            foreach (KeyValuePair<int, Interval> entry in this.m_Intervals)
            {
                if (entry.Value.CanRun) entry.Value.Run();
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        /// <summary>
        /// Runs the action callback after time seconds have passed.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time"></param>
        /// <param name="mode"></param>
        public async Task RunIn(Action action, float time, TimeMode.UpdateMode mode)
        {
            TimeMode timeMode = new TimeMode(mode);
            
            float startTime = timeMode.Time;
            while (timeMode.Time < startTime + time)
            {
                if (ApplicationManager.IsExiting) return;
                await Task.Yield();
            }
            
            action?.Invoke();
        }

        /// <summary>
        /// Starts executing the callback action every few seconds defined by an interval. Returns
        /// the interval ID which can be used to cancel the execution.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval"></param>
        /// <param name="mode"></param>
        /// <returns>Id of the interval</returns>
        public int RunInterval(Action action, float interval, TimeMode.UpdateMode mode)
        {
            int intervalId = ++IntervalCounter;
            this.m_Intervals[intervalId] = new Interval(action, interval, mode);
            
            return intervalId;
        }

        /// <summary>
        /// Stops an interval callback from running again.
        /// </summary>
        /// <param name="intervalId"></param>
        public void StopInterval(int intervalId)
        {
            this.m_Intervals.Remove(intervalId);
        }
    }
}