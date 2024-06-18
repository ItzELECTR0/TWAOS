using System;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Kinematic")]
    [Image(typeof(IconCharacterRun), ColorTheme.Type.Green)]
    
    [Category("Kinematic")]
    [Description("Default animation system for characters")]

    [Serializable]
    public class UnitAnimimKinematic : TUnitAnimim
    {
        private const float SMOOTH_PIVOT = 0.01f;
        private const float SMOOTH_GROUNDED = 0.2f;
        private const float SMOOTH_STAND = 0.1f;
        
        // STATIC PROPERTIES: ---------------------------------------------------------------------

        private static readonly int K_SPEED_Z = Animator.StringToHash("Speed-Z");
        private static readonly int K_SPEED_X = Animator.StringToHash("Speed-X");
        private static readonly int K_SPEED_Y = Animator.StringToHash("Speed-Y");
        
        private static readonly int K_SURFACE_SPEED = Animator.StringToHash("Movement");
        private static readonly int K_PIVOT_SPEED = Animator.StringToHash("Pivot");

        private static readonly int K_GROUNDED = Animator.StringToHash("Grounded");
        private static readonly int K_STAND = Animator.StringToHash("Stand");

        // MEMBERS: -------------------------------------------------------------------------------
        
        protected Dictionary<int, AnimFloat> m_SmoothParameters;
        protected Dictionary<int, AnimFloat> m_IndependentParameters;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);

            this.m_SmoothParameters = new Dictionary<int, AnimFloat>
            {
                { K_SPEED_Z, new AnimFloat(0f, this.m_SmoothTime) },
                { K_SPEED_X, new AnimFloat(0f, this.m_SmoothTime) },
                { K_SPEED_Y, new AnimFloat(0f, this.m_SmoothTime) },
                { K_SURFACE_SPEED, new AnimFloat(0f, this.m_SmoothTime) },
            };
            
            this.m_IndependentParameters = new Dictionary<int, AnimFloat>
            {
                { K_PIVOT_SPEED, new AnimFloat(0f, SMOOTH_PIVOT) },
                { K_GROUNDED, new AnimFloat(1f, SMOOTH_GROUNDED) },
                { K_STAND, new AnimFloat(1f, SMOOTH_STAND) },
            };
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (this.m_Animator == null) return;
            if (!this.m_Animator.gameObject.activeInHierarchy) return;

            this.m_Animator.updateMode = this.Character.Time.UpdateTime == TimeMode.UpdateMode.GameTime
                ? AnimatorUpdateMode.Normal
                : AnimatorUpdateMode.UnscaledTime;

            IUnitMotion motion = this.Character.Motion;
            IUnitDriver driver = this.Character.Driver;
            IUnitFacing facing = this.Character.Facing;

            Vector3 movementDirection = motion.LinearSpeed > 0f
                ? driver.LocalMoveDirection / motion.LinearSpeed
                : Vector3.zero;
            
            float movementMagnitude = Vector3.Scale(movementDirection, Vector3Plane.NormalUp).magnitude;
            float pivot = facing.PivotSpeed;

            foreach (KeyValuePair<int, AnimFloat> parameter in this.m_SmoothParameters)
            {
                parameter.Value.Smooth = this.m_SmoothTime;
            }

            float deltaTime = this.Character.Time.DeltaTime;

            // Update anim parameters:
            this.m_SmoothParameters[K_SPEED_Z].UpdateWithDelta(movementDirection.z, deltaTime);
            this.m_SmoothParameters[K_SPEED_X].UpdateWithDelta(movementDirection.x, deltaTime);
            this.m_SmoothParameters[K_SPEED_Y].UpdateWithDelta(movementDirection.y, deltaTime);
            this.m_SmoothParameters[K_SURFACE_SPEED].UpdateWithDelta(movementMagnitude, deltaTime);

            this.m_IndependentParameters[K_PIVOT_SPEED].UpdateWithDelta(pivot, deltaTime);
            this.m_IndependentParameters[K_GROUNDED].UpdateWithDelta(driver.IsGrounded, deltaTime);
            this.m_IndependentParameters[K_STAND].UpdateWithDelta(motion.StandLevel.Current, deltaTime);

            // Update animator parameters:
            this.m_Animator.SetFloat(K_SPEED_Z, this.m_SmoothParameters[K_SPEED_Z].Current);
            this.m_Animator.SetFloat(K_SPEED_X, this.m_SmoothParameters[K_SPEED_X].Current);
            this.m_Animator.SetFloat(K_SPEED_Y, this.m_SmoothParameters[K_SPEED_Y].Current);
            this.m_Animator.SetFloat(K_SURFACE_SPEED, this.m_SmoothParameters[K_SURFACE_SPEED].Current);

            this.m_Animator.SetFloat(K_PIVOT_SPEED, this.m_IndependentParameters[K_PIVOT_SPEED].Current);
            this.m_Animator.SetFloat(K_GROUNDED, this.m_IndependentParameters[K_GROUNDED].Current);
            this.m_Animator.SetFloat(K_STAND, this.m_IndependentParameters[K_STAND].Current);
        }
    }
}