using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class TweenInput<T> : ITweenInput
    {
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly float m_StartTime;
        
        private readonly T m_ValueSource;
        private readonly T m_ValueTarget;

        private readonly Easing.Type m_Easing;
        private readonly Action<T, T, float> m_Update;
        private readonly TimeMode m_TimeMode;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public int Hash { get; }
        public float Duration { get; }

        public bool IsFinished { get; private set; }
        public bool IsComplete { get; private set; }
        public bool IsCanceled { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<bool> EventFinish;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public TweenInput(T source, T target, float duration, Action<T, T, float> update, int hash, 
            Easing.Type easing = Easing.Type.QuadInOut, 
            TimeMode.UpdateMode updateMode = TimeMode.UpdateMode.GameTime)
        {
            this.m_ValueSource = source;
            this.m_ValueTarget = target;

            this.m_Easing = easing;
            this.m_Update = update;
            
            this.m_TimeMode = new TimeMode(updateMode);

            this.m_StartTime = this.m_TimeMode.Time;
            this.Duration = duration;
            this.Hash = hash;
        }

        public TweenInput(T source, T target, float duration, int hash, 
            Easing.Type easing = Easing.Type.QuadInOut, 
            TimeMode.UpdateMode updateMode = TimeMode.UpdateMode.GameTime)
            : this(source, target, duration, null, hash, easing, updateMode)
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool OnUpdate()
        {
            float elapsed = this.m_TimeMode.Time - this.m_StartTime;
            float t = this.Duration > float.Epsilon ? Mathf.Clamp01(elapsed / this.Duration) : 1f;
            
            this.m_Update?.Invoke(
                this.m_ValueSource, 
                this.m_ValueTarget,
                Easing.GetEase(this.m_Easing, 0f, 1f, t)
            );
            
            return t >= 1f;
        }

        public void OnComplete()
        {
            this.IsFinished = true;
            this.IsComplete = true;
            this.EventFinish?.Invoke(true);
        }
        
        public void OnCancel()
        {
            this.IsFinished = true;
            this.IsCanceled = true;
            this.EventFinish?.Invoke(false);
        }
    }
}