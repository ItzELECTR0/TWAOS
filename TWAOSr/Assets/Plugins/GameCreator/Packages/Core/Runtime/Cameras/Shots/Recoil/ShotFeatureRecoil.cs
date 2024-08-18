using System;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    public class ShotFeatureRecoil
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly ShotCamera m_ShotCamera;
        
        [NonSerialized] private float m_StartTime;
        [NonSerialized] private float m_Duration;

        [NonSerialized] private bool m_HasRecoil;
        [NonSerialized] private Vector2 m_Recoil;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShotFeatureRecoil(ShotCamera shotCamera)
        {
            this.m_ShotCamera = shotCamera;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Run(float duration, Vector2 recoil)
        {
            this.m_StartTime = this.m_ShotCamera.TimeMode.Time;
            this.m_Duration = duration;

            this.m_Recoil = recoil;
            this.m_HasRecoil = true;
        }
        
        public void Update(out float pitch, out float yaw)
        {
            pitch = 0f;
            yaw = 0f;

            bool hasRecoil = this.m_HasRecoil;
            this.m_HasRecoil = false;
            
            if (this.m_Duration <= 0f)
            {
                if (hasRecoil)
                {
                    pitch = this.m_Recoil.x;
                    yaw = this.m_Recoil.y;
                }
                
                return;
            }

            float elapsedTime = this.m_ShotCamera.TimeMode.Time - this.m_StartTime;
            if (elapsedTime > this.m_Duration) return;

            pitch = this.m_Recoil.x * this.m_ShotCamera.TimeMode.DeltaTime;
            yaw = this.m_Recoil.y * this.m_ShotCamera.TimeMode.DeltaTime;
        }
    }
}