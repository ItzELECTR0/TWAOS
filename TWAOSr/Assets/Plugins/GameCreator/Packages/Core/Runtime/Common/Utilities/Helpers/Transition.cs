using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class Transition
    {
        [SerializeField] private float m_Duration = 0f;
        [SerializeField] private Easing.Type m_Easing = Easing.Type.QuadInOut;
        [SerializeField] private TimeMode.UpdateMode m_Time = TimeMode.UpdateMode.GameTime;
        [SerializeField] private bool m_WaitToComplete = true;

        // PROPERTIES: ----------------------------------------------------------------------------

        public float Duration => this.m_Duration;
        
        public Easing.Type EasingType => this.m_Easing;

        public TimeMode.UpdateMode Time => this.m_Time;

        public bool WaitToComplete => this.m_Duration > float.Epsilon && this.m_WaitToComplete;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Transition()
        { }

        public Transition(TimeMode.UpdateMode time)
        {
            this.m_Time = time;
        }

        public Transition(float duration, Easing.Type easing, bool waitToComplete)
        {
            this.m_Duration = duration;
            this.m_Easing = easing;
            this.m_WaitToComplete = waitToComplete;
        }
    }
}