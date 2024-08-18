using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    public class ThirdPersonAim
    {
        private const float EPSILON = 0.001f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly ShotTypeThirdPerson m_System;
        
        [NonSerialized] private float m_TransitionTime;
        [NonSerialized] private float m_TransitionDuration;
        
        [NonSerialized] private float m_StartShoulder;
        [NonSerialized] private float m_StartLift;
        [NonSerialized] private float m_StartRadius;
        
        [NonSerialized] private Quaternion m_Aim;
        
        [NonSerialized] private float m_TargetShoulder;
        [NonSerialized] private float m_TargetLift;
        [NonSerialized] private float m_TargetRadius;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] private float T { get; set; }
        
        [field: NonSerialized] public float Shoulder { get; private set; }
        [field: NonSerialized] public float Lift { get; private set; }
        [field: NonSerialized] public float Radius { get; private set; }
        
        [field: NonSerialized] public Quaternion Aim { get; private set; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ThirdPersonAim(float shoulder, float lift, float radius, ShotTypeThirdPerson system)
        {
            this.m_System = system;
            
            this.m_StartShoulder = shoulder;
            this.m_StartLift = lift;
            this.m_StartRadius = radius;

            this.m_TargetShoulder = shoulder;
            this.m_TargetLift = lift;
            this.m_TargetRadius = radius;

            this.Shoulder = shoulder;
            this.Lift = lift;
            this.Radius = radius;

            this.m_Aim = Quaternion.identity;
            this.Aim = Quaternion.identity;

            this.m_TransitionTime = 0f;
            this.m_TransitionDuration = 0f;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void Switch(float shoulder, float lift, float radius, Quaternion aim, float duration)
        {
            this.m_StartShoulder = this.Shoulder;
            this.m_StartLift = this.Lift;
            this.m_StartRadius = this.Radius;
            
            this.m_TargetShoulder = shoulder;
            this.m_TargetLift = lift;
            this.m_TargetRadius = radius;

            this.m_Aim = aim;
            this.Aim = aim;
            
            this.m_TransitionTime = this.m_System.ShotCamera.TimeMode.Time;
            this.m_TransitionDuration = duration;
        }
        
        public void Update()
        {
            float currentTime = this.m_System.ShotCamera.TimeMode.Time;
            float t = this.m_TransitionDuration > EPSILON
                ? (currentTime - this.m_TransitionTime) / this.m_TransitionDuration
                : 1f;

            this.T = Easing.QuadOut(0f, 1f, t);
            
            this.Shoulder = Mathf.Lerp(this.m_StartShoulder, this.m_TargetShoulder, this.T);
            this.Lift = Mathf.Lerp(this.m_StartLift, this.m_TargetLift, this.T);
            this.Radius = Mathf.Lerp(this.m_StartRadius, this.m_TargetRadius, this.T);

            this.Aim = Quaternion.Euler(
                Mathf.Lerp(QuaternionUtils.Convert180(this.m_Aim.eulerAngles.x), 0f, this.T),
                Mathf.Lerp(QuaternionUtils.Convert180(this.m_Aim.eulerAngles.y), 0f, this.T),
                0f
            );
        }
    }
}