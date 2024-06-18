using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class MotionTransient
    {
        [NonSerialized] private readonly Character m_Character;
        [NonSerialized] private readonly TUnitMotion m_Motion;

        [NonSerialized] private Vector3 m_Direction;
        
        [NonSerialized] private float m_StartTime;
        [NonSerialized] private float m_Duration;
        [NonSerialized] private float m_Fade;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private float Opacity
        {
            get
            {
                if (this.m_Direction == Vector3.zero) return 0f;
                
                float currentTime = this.m_Character.Time.Time;

                if (currentTime <= this.m_StartTime + this.m_Duration) return 1f;
                if (this.m_Fade <= float.Epsilon) return 0f;

                if (currentTime >= this.m_StartTime + this.m_Duration + this.m_Fade) return 0f;
                
                float startFade = currentTime - (this.m_StartTime + this.m_Duration);
                float ratio = startFade / this.m_Fade;
                
                return 1f - ratio;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MotionTransient(TUnitMotion motion)
        {
            this.m_Character = motion.Character;
            this.m_Motion = motion;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Set(Vector3 direction, float speed, float duration, float fade)
        {
            this.m_Direction = direction.normalized * speed;

            this.m_StartTime = this.m_Character.Time.Time;
            this.m_Duration = duration;
            this.m_Fade = fade;
        }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal Character.MovementType Update()
        {
            if (this.m_Direction == Vector3.zero) return this.m_Motion.MovementType;
            
            float opacity = this.Opacity;
            if (opacity <= float.Epsilon) return this.m_Motion.MovementType;

            Vector3 motionDirection = Vector3.Lerp(this.m_Motion.MoveDirection, this.m_Direction, opacity);
            this.m_Motion.MoveDirection = motionDirection;
            this.m_Motion.MovePosition = this.m_Character.transform.TransformPoint(motionDirection);

            return Character.MovementType.MoveToDirection;
        }
    }
}