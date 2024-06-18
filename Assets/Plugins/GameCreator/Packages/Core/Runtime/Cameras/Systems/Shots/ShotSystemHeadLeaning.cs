using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemHeadLeaning : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemHeadLeaning).GetHashCode();
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private bool m_IsActive = true;
        [SerializeField] private float m_SmoothTime = 0.2f; 
        
        [SerializeField] private PropertyGetDecimal m_AngleForward = GetDecimalDecimal.Create(5f);
        [SerializeField] private PropertyGetDecimal m_AngleSideways = GetDecimalDecimal.Create(2f);

        // MEMBERS: -------------------------------------------------------------------------------

        private Quaternion m_LeaningCurrent;
        private Quaternion m_LeaningVelocity;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public bool IsActive
        {
            get => this.m_IsActive;
            set => this.m_IsActive = value;
        }

        // IMPLEMENTS: ----------------------------------------------------------------------------
        
        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            Quaternion rotation = Quaternion.identity;
            
            if (this.m_IsActive)
            {
                ShotTypeFirstPerson shotTypeFirstPerson = shotType as ShotTypeFirstPerson;
                if (shotTypeFirstPerson == null) return;

                Character character = shotTypeFirstPerson.Character;

                if (character != null)
                {
                    Vector3 direction = Vector3.ClampMagnitude(
                        character.Motion.LinearSpeed > 0f
                            ? character.Driver.LocalMoveDirection / character.Motion.LinearSpeed
                            : Vector3.zero,
                        1f
                    );

                    float tiltForward = (float) this.m_AngleForward.Get(shotType.Args);
                    float tiltSideways = (float) this.m_AngleSideways.Get(shotType.Args);
                    
                    rotation = Quaternion.Euler(
                        direction.z * +tiltForward,
                        0f,
                        direction.x * tiltSideways * -1
                    );
                }
            }

            this.m_LeaningCurrent = QuaternionUtils.SmoothDamp(
                this.m_LeaningCurrent,
                rotation,
                ref this.m_LeaningVelocity,
                this.m_SmoothTime,
                shotType.ShotCamera.TimeMode.DeltaTime
            );
            
            shotType.Rotation *= this.m_LeaningCurrent;
        }
    }
}