using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TUnitMotion : TUnit, IUnitMotion
    {
        private const float MIN_STOP_THRESHOLD = 0.01f;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] protected Character.MovementType m_MovementType;
        [NonSerialized] protected MotionTransient m_Transient;
        [NonSerialized] protected TMotion m_MotionData;

        [NonSerialized] protected bool m_IsJumping;
        [NonSerialized] protected float m_IsJumpingForce;

        [NonSerialized] protected float m_StopDistance;
        [NonSerialized] protected float m_FollowMinDistance;
        [NonSerialized] protected float m_FollowMaxDistance;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected MotionInteraction m_Interaction;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public Vector3 MoveDirection { get; set; }
        public Vector3 MovePosition  { get; set; }
        public Vector3 MoveRotation  { get; set; }

        public float StopThreshold => this.m_StopDistance;

        public float FollowMinDistance => this.m_FollowMinDistance;
        public float FollowMaxDistance => this.m_FollowMaxDistance;

        public Character.MovementType MovementType
        {
            get => this.m_MovementType;
            set => this.m_MovementType = value;
        }

        public bool IsJumping => this.m_IsJumping;
        public float IsJumpingForce => this.IsJumping ? this.m_IsJumpingForce : 0f;
        
        public abstract float LinearSpeed  { get; set; }
        public abstract float AngularSpeed { get; set; }
        
        public AnimFloat StandLevel { get; }
        
        public abstract float GravityUpwards   { get; set; }
        public abstract float GravityDownwards { get; set; }
        public abstract float TerminalVelocity { get; set; }
        
        public abstract float JumpForce { get; set; }
        public abstract float JumpCooldown { get; set; }
        
        public abstract int DashInSuccession { get; set; }
        public abstract bool DashInAir { get; set; }
        public abstract float DashCooldown { get; set; }

        public abstract float Mass   { get; set; }
        public abstract float Height { get; set; }
        public abstract float Radius { get; set; }
        
        public abstract bool UseAcceleration { get; set; }
        public abstract float Acceleration { get; set; }
        public abstract float Deceleration { get; set; }
        
        public abstract bool CanJump { get; set; }
        public abstract int AirJumps { get; set; }
        
        public float InteractionRadius
        {
            get => this.m_Interaction.Radius;
            set => this.m_Interaction.Radius = value;
        }

        public InteractionMode InteractionMode
        {
            get => this.m_Interaction.Mode;
            set => this.m_Interaction.Mode = value;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected TUnitMotion()
        {
            this.MoveDirection = Vector3.zero;
            this.StandLevel = new AnimFloat(1f, 1f);

            this.m_MovementType = Character.MovementType.None;
            this.m_Interaction = new MotionInteraction();
        }

        public virtual void OnStartup(Character character)
        {
            this.Character = character;
            this.m_Transient = new MotionTransient(this);
        }
        
        public virtual void AfterStartup(Character character)
        {
            this.Character = character;
        }

        public virtual void OnDispose(Character character)
        {
            this.Character = character;
        }

        public virtual void OnEnable()
        {
            this.Character.EventAfterUpdate += this.AfterCharacterUpdate;
        }

        public virtual void OnDisable()
        {
            this.Character.EventAfterUpdate -= this.AfterCharacterUpdate;
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        public virtual void OnUpdate()
        {
            if (this.Character.IsDead)
            {
                this.m_MovementType = Character.MovementType.None;
                return;
            }

            this.m_MovementType = this.m_MotionData?.Update() ?? Character.MovementType.None;
            this.m_MovementType = this.m_Transient?.Update() ?? Character.MovementType.None;
            
            this.StandLevel.UpdateWithDelta(this.Character.Time.DeltaTime);
        }
        
        public virtual void OnFixedUpdate()
        { }

        // INTERFACE MOVE METHODS: ----------------------------------------------------------------

        /// <summary>
        /// Creates a new Transient movement towards a direction. Useful for dashes
        /// </summary>
        /// <param name="direction">The world direction to move towards</param>
        /// <param name="duration">How long it takes to move at full speed</param>
        /// <param name="fade">How long it takes to blend out</param>
        public void SetMotionTransient(Vector3 direction, float speed, float duration, float fade)
        {
            this.m_Transient.Set(direction, speed, duration, fade);
        }
        
        /// <summary>
        /// Move the Character towards a particular direction.
        /// </summary>
        /// <param name="velocity">Direction multiplied by speed that the Character moves</param>
        /// <param name="space">Whether the velocity parameter is in relative space to itself</param>
        /// <param name="priority">Cancelable inputs should be set to 0. Otherwise 1</param>
        public virtual void MoveToDirection(Vector3 velocity, Space space, int priority)
        {
            if (!this.UpdateMotionData<MotionToDirection>(priority)) return;

            if (this.m_MotionData is MotionToDirection motion)
            {
                this.m_MovementType = motion.Setup(velocity, space);
            }
        }

        /// <summary>
        /// Stops the Character from moving towards a direction and resets the priority so other
        /// move commands can be placed.
        /// </summary>
        /// <param name="priority">If the current Motion has a higher priority this is ignored</param>
        public virtual void StopToDirection(int priority)
        {
            if (!this.UpdateMotionData<MotionToDirection>(priority)) return;
            if (this.m_MotionData is MotionToDirection motion && motion.Priority <= priority)
            {
                motion.Stop(true);
            }
        }

        /// <summary>
        /// Moves the Character towards a specific location.
        /// </summary>
        /// <param name="location">Position and/or rotation to move towards</param>
        /// <param name="stopDistance">Distance from which the movement is considered as complete</param>
        /// <param name="onFinish">Callback executed when the Character reaches its destination</param>
        /// <param name="priority">Higher priority grants permission to cancel other movements</param>
        public virtual void MoveToLocation(Location location, float stopDistance, 
            Action<Character, bool> onFinish, int priority)
        {
            this.m_StopDistance = Mathf.Max(stopDistance, MIN_STOP_THRESHOLD);
            if (!this.UpdateMotionData<MotionToLocation>(priority))
            {
                onFinish?.Invoke(this.Character, false);
                return;
            }

            if (this.m_MotionData is MotionToLocation motion)
            {
                this.m_MovementType = motion.Setup(location, this.m_StopDistance, onFinish);
            }
        }

        /// <summary>
        /// Moves the Character towards a specific game object.
        /// </summary>
        /// <param name="target">Game Object to reach</param>
        /// <param name="stopDistance">Distance from which the movement is considered as complete</param>
        /// <param name="onFinish">Callback executed when the Character reaches its destination</param>
        /// <param name="priority">Higher priority grants permission to cancel other movements</param>
        public virtual void MoveToTransform(Transform target, float stopDistance, 
            Action<Character, bool> onFinish, int priority )
        {
            this.m_StopDistance = Mathf.Max(stopDistance, MIN_STOP_THRESHOLD);
            if (!this.UpdateMotionData<MotionToTransform>(priority))
            {
                onFinish?.Invoke(this.Character, false);
                return;
            }
        
            if (this.m_MotionData is MotionToTransform motion)
            {
                this.m_MovementType = motion.Setup(target, this.m_StopDistance, onFinish);
            }
        }

        /// <summary>
        /// Moves the Character towards a specific Marker, defines a target position and a rotation.
        /// </summary>
        /// <param name="marker">The Marker object to reach, with a target position and rotation</param>
        /// <param name="stopDistance">Distance from which the movement is considered as complete</param>
        /// <param name="onFinish">Callback executed when the Character reaches its destination</param>
        /// <param name="priority">Higher priority grants permission to cancel other movements</param>
        public virtual void MoveToMarker(Marker marker, float stopDistance, 
            Action<Character, bool> onFinish, int priority)
        {
            this.m_StopDistance = Mathf.Max(stopDistance, marker.StopDistance, MIN_STOP_THRESHOLD);
            if (!this.UpdateMotionData<MotionToMarker>(priority))
            {
                onFinish?.Invoke(this.Character, false);
                return;
            }
            
            if (this.m_MotionData is MotionToMarker motion)
            {
                this.m_MovementType = motion.Setup(marker, this.m_StopDistance, onFinish);
            }
        }

        /// <summary>
        /// Starts moving towards a game object, keeping a distance within the boundaries of a
        /// defined maximum and minim radius. The Character will keep following it until the
        /// <see cref="StopFollowingTarget(int)"/> is called.
        /// </summary>
        /// <param name="target">Game object to follow</param>
        /// <param name="minRadius">Distance to the target the Character will try to reach</param>
        /// <param name="maxRadius">Distance the Character starts moving towards the target</param>
        /// <param name="priority">Higher priority grants permission to cancel other movements</param>
        public virtual void StartFollowingTarget(Transform target, float minRadius, 
            float maxRadius, int priority)
        {
            this.m_FollowMinDistance = minRadius;
            this.m_FollowMaxDistance = maxRadius;

            if (!this.UpdateMotionData<MotionFollow>(priority)) return;

            if (this.m_MotionData is MotionFollow motion)
            {
                this.m_MovementType = motion.Setup(target, minRadius, maxRadius);
            }
        }

        /// <summary>
        /// Cancels the <see cref="StartFollowingTarget"/> as long as the priority is higher or
        /// equal to the current motion priority.
        /// </summary>
        /// <param name="priority">Higher priority grants permission to cancel other movements</param>
        public virtual void StopFollowingTarget(int priority)
        {
            if (this.m_MotionData is MotionFollow motion && motion.Priority <= priority)
            {
                motion.Stop(true);
            }
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected bool UpdateMotionData<T>(int priority) where T : TMotion, new()
        {
            if (priority < this.m_MotionData?.Priority) return false;
            if (this.m_MotionData is T)
            {
                this.m_MotionData.Priority = priority;
                return true;
            }
            
            this.m_MotionData?.Stop(false);
            this.m_MotionData = new T();
            this.m_MotionData.Initialize(this, priority);
            
            return true;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Jump()
        {
            this.Jump(this.JumpForce);
        }
        
        public virtual void Jump(float force)
        {
            if (this.Character.IsDead) return;
            if (!this.CanJump) return;
            
            this.m_IsJumping = true;
            this.m_IsJumpingForce = force;
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------
        
        private void AfterCharacterUpdate()
        {
            this.m_IsJumping = false;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public virtual void OnDrawGizmos(Character character)
        {
            if (character.IsDead) Gizmos.color = Color.red;
            else Gizmos.color = character.IsPlayer ? Color.green : Color.cyan;

            Vector3 position = character.transform.position - (Vector3.up * this.Height * 0.5f);
            GizmosExtension.Cylinder(position, this.Height, this.Radius);

            GizmosExtension.Triangle(
                position + character.transform.TransformDirection(Vector3.forward) * (this.Radius + 0.1f),
                character.transform.rotation,
                0.25f
            );

            this.m_Interaction.DrawGizmos(character);

            if (Application.isPlaying)
            {
                this.m_MotionData?.OnDrawGizmos();
            }
        }
    }
}