using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class TimeManager : Singleton<TimeManager>
    {
        private const float PHYSICS_TIME_STEP = 0.02f;
        private const float EPSILON = 0.01f;

        // STRUCTS: -------------------------------------------------------------------------------
        
        private readonly struct TimeData
        {
            private readonly float m_To;
            private readonly float m_From;

            private readonly float m_Transition;
            private readonly float m_StartTime;
            
            private readonly float m_Duration;
            private readonly float m_Delay;

            public bool IsInDelay
            {
                get
                {
                    float elapsedTime = Time.unscaledTime - (this.m_StartTime + this.m_Delay);
                    return elapsedTime < 0f;
                }
            }

            public float Get
            {
                get
                {
                    if (this.m_Transition <= EPSILON) return this.m_To;

                    float elapsedTime = Time.unscaledTime - (this.m_StartTime + this.m_Delay);
                    float t = elapsedTime / this.m_Transition;
                    return Mathf.Lerp(this.m_From, this.m_To, t);
                }
            }

            public bool TimeRanOut
            {
                get
                {
                    if (this.m_Duration < 0f) return false;

                    float elapsedTime = Time.unscaledTime - (this.m_StartTime + this.m_Delay); 
                    return elapsedTime > this.m_Duration;
                }
            }

            public TimeData(float transition, float to, float from, float duration, float delay)
            {
                this.m_To = to;
                this.m_From = from;

                this.m_Transition = transition;
                this.m_StartTime = Time.unscaledTime;
                this.m_Duration = duration;
                this.m_Delay = delay;
            }
        }

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Dictionary<int, TimeData> m_TimeScales = new Dictionary<int, TimeData>();
        [NonSerialized] private float m_EndTime;

        [NonSerialized] private readonly List<int> m_RemoveCandidates = new List<int>();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetTimeScale(float timeScale, int layer)
        {
            this.SetTimeScale(timeScale, -1f, layer);
        }
        
        public void SetTimeScale(float timeScale, float duration, int layer)
        {
            this.SetTimeScale(timeScale, duration, 0f, layer);
        }
        
        public void SetTimeScale(float timeScale, float duration, float delay, int layer)
        {
            this.m_TimeScales[layer] = new TimeData(0f, timeScale, 1f, duration, delay);
            this.RecalculateTimeScale();
        }

        public void SetSmoothTimeScale(float timeScale, float transition, int layer)
        {
            this.SetSmoothTimeScale(timeScale, transition, -1f, layer);
        }
        
        public void SetSmoothTimeScale(float timeScale, float transition, float duration, int layer)
        {
            this.SetSmoothTimeScale(timeScale, transition, duration, 0f, layer);
        }
        
        public void SetSmoothTimeScale(float timeScale, float transition, float duration, float delay, int layer)
        {
            if (transition < EPSILON)
            {
                this.SetTimeScale(timeScale, duration, layer);
                return;
            }

            float endTime = transition + duration >= 0f ? delay : 0f;
            this.m_EndTime = Mathf.Max(this.m_EndTime, Time.unscaledTime + endTime);

            TimeData timeData = new TimeData(
                transition, 
                timeScale,
                Time.timeScale,
                duration, 
                delay
            );
            
            this.m_TimeScales[layer] = timeData;
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        private void Update()
        {
            this.RecalculateTimeScale();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RecalculateTimeScale()
        {
            this.m_RemoveCandidates.Clear();
            
            float scale = 1f;
            bool firstEntrySet = false;
            
            foreach (KeyValuePair<int, TimeData> item in this.m_TimeScales)
            {
                if (item.Value.IsInDelay) continue;

                scale = firstEntrySet ? item.Value.Get : Math.Min(scale, item.Value.Get);
                firstEntrySet = true;

                if (item.Value.TimeRanOut)
                {
                    this.m_RemoveCandidates.Add(item.Key);
                }
            }

            Time.timeScale = scale;
            Time.fixedDeltaTime = PHYSICS_TIME_STEP * scale;

            foreach (int removeCandidate in this.m_RemoveCandidates)
            {
                this.m_TimeScales.Remove(removeCandidate);
            }
        }
    }
}