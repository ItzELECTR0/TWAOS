using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
	public class AnimFloat
	{
        private const float SMOOTH = 0.1f;
        
        // TRANSIENT STRUCT: ----------------------------------------------------------------------
        
        public struct Transient
        {
            [NonSerialized] private readonly float m_StartTime;
            
            [NonSerialized] private readonly float m_Value;
            [NonSerialized] private readonly float m_SmoothIn;
            [NonSerialized] private readonly float m_SmoothOut;
            [NonSerialized] private readonly float m_Duration;

            public Transient(float value, float smoothIn, float duration, float smoothOut)
            {
                this.m_StartTime = Time.time;

                this.m_Value = value;
                this.m_SmoothIn = smoothIn;
                this.m_Duration = duration;
                this.m_SmoothOut = smoothOut;
            }

            public bool IsActive
            {
                get
                {
                    float totalDuration = this.m_SmoothIn + this.m_Duration + this.m_SmoothOut;
                    return Time.time - this.m_StartTime < totalDuration;
                }
            }
            
            public float Update(float current, float target)
            {
                if (Time.time <= this.m_StartTime + this.m_SmoothIn)
                {
                    float tIn = (Time.time - this.m_StartTime) / this.m_SmoothIn;
                    return Mathf.LerpUnclamped(current, this.m_Value, Easing.BackOut(0f, 1f, tIn));
                }
                
                if (Time.time <= this.m_StartTime + this.m_SmoothIn + this.m_Duration)
                {
                    return this.m_Value;
                }

                float startTime = this.m_StartTime + this.m_SmoothIn + this.m_Duration;
                float tOut = (Time.time - startTime) / this.m_SmoothOut;
                
                return Mathf.LerpUnclamped(this.m_Value, target, Easing.QuadOut(0f, 1f, tOut));
            }
        }

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private Transient m_Transient;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public float Current { get; set; }
        [field: NonSerialized] public float Target  { get; set; }
        [field: NonSerialized] public float Smooth  { get; set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public AnimFloat(float value, float smooth = SMOOTH)
		{
            this.Current = value;
			this.Target = value;
			this.Smooth = smooth;
		}

        public AnimFloat(float value, float target, float smooth) : this(value, smooth)
        {
            this.Target = target;
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public void UpdateWithDelta(float deltaTime)
        {
            if (this.m_Transient.IsActive)
            {
                this.Current = this.m_Transient.Update(this.Current, this.Target);
                return;
            }
            
            float sign = Math.Sign(this.Target - this.Current);
                
            if (this.Smooth <= float.Epsilon) this.Current = this.Target;
            else this.Current += deltaTime * sign / this.Smooth;
            
            if (sign <= 0f) this.Current = Math.Max(this.Current, this.Target);
            if (sign >= 0f) this.Current = Math.Min(this.Current, this.Target);
        }
        
        public void UpdateWithDelta(float target, float deltaTime)
        {
            this.Target = target;
            this.UpdateWithDelta(deltaTime);
        }
        
        public void UpdateWithDelta(float target, float smooth, float deltaTime)
        {
            this.Smooth = smooth;
            this.UpdateWithDelta(target, deltaTime);
        }
        
        public void UpdateWithDelta(bool target, float deltaTime)
        {
            this.UpdateWithDelta(target ? 1f : 0f, deltaTime);
        }
        
        public void UpdateWithDelta(bool target, float smooth, float deltaTime)
        {
            this.UpdateWithDelta(target ? 1f : 0f, smooth, deltaTime);
        }
        
        public void Update()
        {
            this.UpdateWithDelta(Time.deltaTime);
        }

        public void Update(float target)
        {
            this.Target = target;
            this.Update();
        }

        public void Update(bool target)
        {
            this.Update(target ? 1f : 0f);
        }

        public void Update(float target, float smooth)
        {
            this.Smooth = smooth;
            this.Update(target);
        }

        public void Update(bool target, float smooth)
        {
            this.Smooth = smooth;
            this.Update(target);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetTransient(Transient transient)
        {
            this.m_Transient = transient;
        }
    }
}