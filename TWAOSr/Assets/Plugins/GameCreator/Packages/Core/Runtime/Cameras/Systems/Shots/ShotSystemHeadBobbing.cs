using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemHeadBobbing : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemHeadBobbing).GetHashCode();
        
        private const float BOB_SMOOTH_TIME = 0.35f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private bool m_IsActive = true;
        [SerializeField] private float m_SmoothTime = BOB_SMOOTH_TIME;
        
        [SerializeField] private PropertyGetDecimal m_StepLength = GetDecimalDecimal.Create(0.75f);
        [SerializeField] private PropertyGetDecimal m_StepHeight = GetDecimalDecimal.Create(0.02f);
        [SerializeField] private PropertyGetDecimal m_StepWidth = GetDecimalDecimal.Create(0.01f);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private AnimFloat m_Speed;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public bool IsActive
        {
            get => this.m_IsActive;
            set => this.m_IsActive = value;
        }

        // IMPLEMENTS: ----------------------------------------------------------------------------

        public override void OnAwake(TShotType shotType)
        {
            base.OnAwake(shotType);
            this.m_Speed = new AnimFloat(0f, this.m_SmoothTime);
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);

            if (shotType is not ShotTypeFirstPerson shotTypeFirstPerson) return;
            float speed;

            if (this.m_IsActive)
            {
                Character character = shotTypeFirstPerson.Character;
                if (character == null || !character.Driver.IsGrounded) speed = 0f;
                else speed = this.GetStepSpeedCoefficient(shotTypeFirstPerson);
            }
            else
            {
                speed = 0f;
            }
            
            this.m_Speed.UpdateWithDelta(
                speed,
                this.m_SmoothTime,
                shotType.ShotCamera.TimeMode.DeltaTime
            );

            float x = this.BobStepBalance(shotTypeFirstPerson);
            float y = this.BobStepHeight(shotTypeFirstPerson);

            Vector3 movement = shotType.Transform.TransformDirection(new Vector3(x, y, 0f));
            shotType.Position += movement;
        }
        
        // MAGIC NUMBERS: -------------------------------------------------------------------------

        private float GetStepFrequency(ShotTypeFirstPerson shotType)
        {
            Character character = shotType.Character;
            float stepLength = (float) this.m_StepLength.Get(shotType.Args);
            
            return character != null && character.Motion.LinearSpeed > 0f
                ? Mathf.Clamp01(stepLength / character.Motion.LinearSpeed) 
                : 0f;
        }
        
        private float GetStepSpeedCoefficient(ShotTypeFirstPerson shotType)
        {
            Character character = shotType.Character;
            Vector3 velocity = Vector3.Scale(Vector3Plane.NormalUp, character.Driver.WorldMoveDirection);
            
            return character != null && character.Motion.LinearSpeed > 0f
                ? Mathf.Clamp01(velocity.magnitude / character.Motion.LinearSpeed) 
                : 0f;
        }
        
        private float GetStepPeriod(ShotTypeFirstPerson shotType)
        {
            float speed = this.GetStepFrequency(shotType);
            if (speed <= float.Epsilon) return 0f;
            
            return shotType.ShotCamera.TimeMode.Time / speed;
        }
        
        // +--------------------------------------------------------------------------------------+
        // | y = cos(x * 2) - 1                                                                   |
        // +--------------------------------------------------------------------------------------+
        private float BobStepHeight(ShotTypeFirstPerson shotType)
        {
            float stepHeight = (float) this.m_StepHeight.Get(shotType.Args);
            float t = this.GetStepPeriod(shotType);

            float maxValue = (Mathf.Cos(t * 2f) - 1f) * stepHeight * this.m_Speed.Current;
            return Mathf.Lerp(0f, maxValue, t);
        }

        // +--------------------------------------------------------------------------------------+
        // | y = sin(x)                                                                           |
        // +--------------------------------------------------------------------------------------+
        private float BobStepBalance(ShotTypeFirstPerson shotType)
        {
            float stepWidth = (float) this.m_StepWidth.Get(shotType.Args);
            float t = this.GetStepPeriod(shotType);
            float maxValue = Mathf.Sin(t) * stepWidth * this.m_Speed.Current; 
            return Mathf.Lerp(0f, maxValue, t);
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmosSelected(TShotType shotType, Transform transform)
        {
            base.OnDrawGizmosSelected(shotType, transform);
            this.DoDrawGizmos(shotType, GIZMOS_COLOR_ACTIVE);
        }
        
        private void DoDrawGizmos(TShotType shotType, Color color)
        {
            Gizmos.color = color;
        }
    }
}