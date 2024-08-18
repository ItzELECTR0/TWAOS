using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public abstract class Clip : IClip, ISerializationCallbackReceiver
    {
        protected const float DEFAULT_PAD = 0.1f;
        protected const float DEFAULT_TIME = 0.3f;
        protected const float DEFAULT_DURATION = 0.3f;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_Time;
        [SerializeField] private float m_Duration;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsStart;
        [NonSerialized] private bool m_IsComplete;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float TimeStart => this.m_Time;
        public float TimeEnd => this.TimeStart + this.Duration;
        public float Duration => this.m_Duration;

        public float DurationToStart => this.m_Time;
        public float DurationToEnd => 1f - this.TimeEnd;

        public bool IsStart => this.m_IsStart;
        public bool IsComplete => this.m_IsComplete;

        protected virtual float MinDuration => 0f;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected Clip()
        {
            this.m_Time = DEFAULT_TIME;
            this.m_Duration = DEFAULT_DURATION;
        }
        
        protected Clip(float time) : this()
        {
            this.m_Time = time;
        }

        protected Clip(float time, float duration) : this(time)
        {
            this.m_Duration = duration;
        }

        // CLIP INTERFACE METHODS: ----------------------------------------------------------------
        
        void IClip.Reset()
        {
            this.m_IsStart = false;
            this.m_IsComplete = false;
            this.OnReset();
        }

        void IClip.Start(ITrack track, Args args)
        {
            this.m_IsStart = true;
            this.OnStart(track, args);
        }

        void IClip.Complete(ITrack track, Args args)
        {
            this.m_IsComplete = true;
            this.OnComplete(track, args);
        }

        void IClip.Cancel(ITrack track, Args args)
        {
            this.m_IsComplete = true;
            this.OnCancel(track, args);
        }

        void IClip.Update(ITrack track, Args args, float t)
        {
            this.OnUpdate(track, args, t);
        }

        // SERIALIZATION INTERFACE METHODS: -------------------------------------------------------
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (AssemblyUtils.IsReloading) return;
            this.ValidateTimes();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (AssemblyUtils.IsReloading) return;
            this.ValidateTimes();
        }

        private void ValidateTimes()
        {
            this.m_Time = Mathf.Clamp01(this.m_Time);
            this.m_Duration = Mathf.Clamp(
                this.m_Duration, 
                this.MinDuration, 
                Mathf.Max(this.MinDuration, 1f - this.m_Time)
            );
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void OnReset()
        { }
        
        protected virtual void OnStart(ITrack track, Args args)
        { }

        protected virtual void OnComplete(ITrack track, Args args)
        { }

        protected virtual void OnCancel(ITrack track, Args args)
        { }

        protected virtual void OnUpdate(ITrack track, Args args, float t)
        { }
    }
}