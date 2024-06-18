using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Controller")]
    [Image(typeof(IconCapsuleSolid), ColorTheme.Type.Green)]
    
    [Category("Character Controller")]
    [Description("Moves the Character using Unity's default Character Controller")]
    
    [Serializable]
    public class UnitDriverController : TUnitDriver
    {
        private const float MAX_SLOPE_SLIDE_FROM_CHARACTER = 90;
        private const float EPSILON_SLIDE_FROM_CHARACTER = 0.001f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected float m_SkinWidth = 0.08f;
        [SerializeField] protected float m_PushForce = 1.0f;
        [SerializeField] protected float m_MaxSlope = 45f;
        [SerializeField] protected float m_StepHeight = 0.3f;
        [SerializeField] private Axonometry m_Axonometry = new Axonometry();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] protected CharacterController m_Controller;

        [NonSerialized] protected Vector3 m_MoveDirection;
        [NonSerialized] protected float m_VerticalSpeed;
 
        [NonSerialized] protected AnimFloat m_IsGrounded;
        [NonSerialized] protected AnimVector3 m_FloorNormal;
 
        [NonSerialized] protected int m_GroundFrame = -100;
        [NonSerialized] protected float m_GroundTime = -100f;
        [NonSerialized] protected float m_JumpTime = -100f;

        [NonSerialized] private DriverControllerComponent m_Helper;
        [NonSerialized] private DriverAdditionalTranslation m_AddTranslation;
        
        [NonSerialized] private Vector3 m_SlideFromCharacter;
        [NonSerialized] private int m_FrameSlideFromCharacter;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public override Vector3 WorldMoveDirection => this.m_Controller != null
            ? this.m_Controller.velocity
            : Vector3.zero;
        
        public override Vector3 LocalMoveDirection => this.Transform.InverseTransformDirection(
            this.WorldMoveDirection
        );

        public override float SkinWidth => this.m_Controller != null 
            ? this.m_Controller.skinWidth
            : 0f;
        
        public override bool IsGrounded
        {
            get
            {
                if (this.m_Controller == null) return false;

                bool inSlideFrame = this.m_FrameSlideFromCharacter < Time.frameCount;
                return this.m_Controller.isGrounded && inSlideFrame;
            }
        }

        public override Vector3 FloorNormal => this.m_FloorNormal.Current;

        public override bool Collision
        {
            get => this.m_Controller.detectCollisions;
            set => this.m_Controller.detectCollisions = value;
        }

        public override Axonometry Axonometry
        {
            get => this.m_Axonometry;
            set => this.m_Axonometry = value;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public UnitDriverController()
        {
            this.m_MoveDirection = Vector3.zero;
            this.m_VerticalSpeed = 0f;
            
            this.m_SlideFromCharacter = Vector3.zero;
            this.m_FrameSlideFromCharacter = -1;
        }

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);

            this.m_IsGrounded = new AnimFloat(1f, 0.01f);
            this.m_FloorNormal = new AnimVector3(Vector3.up, 0.05f);

            this.m_Controller = this.Character.GetComponent<CharacterController>();
            if (this.m_Controller == null)
            {
                GameObject instance = this.Character.gameObject;
                this.m_Controller = instance.AddComponent<CharacterController>();
                this.m_Controller.hideFlags = HideFlags.HideInInspector;
            }
            
            this.m_Helper = DriverControllerComponent.Register(
                this.Character,
                this.OnControllerColliderHit
            );

            character.Ragdoll.EventBeforeStartRagdoll += this.OnStartRagdoll;
            character.Ragdoll.EventAfterStartRecover += this.OnEndRagdoll;
        }

        public override void OnDispose(Character character)
        {
            base.OnDispose(character);

            UnityEngine.Object.Destroy(this.m_Helper);
            UnityEngine.Object.Destroy(this.m_Controller);
            
            character.Ragdoll.EventBeforeStartRagdoll -= this.OnStartRagdoll;
            character.Ragdoll.EventAfterStartRecover -= this.OnEndRagdoll;
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public override void OnUpdate()
        {
            if (this.Character.IsDead) return;
            if (this.m_Controller == null) return;
            
            this.UpdateProperties();

            this.UpdateGravity(this.Character.Motion);
            this.UpdateJump(this.Character.Motion);

            this.UpdateTranslation(this.Character.Motion);
            this.m_Axonometry?.ProcessPosition(this, this.Transform.position);
        }

        public override void OnFixedUpdate()
        {
            if (this.Character.IsDead) return;
            if (this.m_Controller == null) return;
            
            base.OnFixedUpdate();
            this.UpdatePhysicProperties();
        }

        protected virtual void UpdateProperties()
        {
            this.m_FloorNormal.UpdateWithDelta(this.Character.Time.DeltaTime);
            this.m_MoveDirection = Vector3.zero;
            this.m_IsGrounded.Update(this.IsGrounded, COYOTE_TIME);
            
            if (Math.Abs(this.m_Controller.skinWidth - this.m_SkinWidth) > float.Epsilon)
            {
                this.m_Controller.skinWidth = this.m_SkinWidth;
            }
            
            if (Math.Abs(this.m_Controller.slopeLimit - this.m_MaxSlope) > float.Epsilon)
            {
                this.m_Controller.slopeLimit = this.m_MaxSlope;
            }
            
            if (Math.Abs(this.m_Controller.stepOffset - this.m_StepHeight) > float.Epsilon)
            {
                this.m_Controller.stepOffset = this.m_StepHeight;
            }
        }
        
        protected virtual void UpdatePhysicProperties()
        {
            float height = this.Character.Motion.Height;
            float radius = this.Character.Motion.Radius;

            if (Math.Abs(this.m_Controller.height - height) > float.Epsilon)
            {
                float offset = (this.m_Controller.height - height) * 0.5f;
                
                this.Transform.localPosition += Vector3.down * offset;
                this.m_Controller.height = height;
                
                this.Character.Animim.ApplyMannequinPosition();
            }

            if (Math.Abs(this.m_Controller.radius - radius) > float.Epsilon)
            {
                this.m_Controller.radius = radius;
            }

            if (this.m_Controller.center != Vector3.zero)
            {
                this.m_Controller.center = Vector3.zero;   
            }
        }

        protected virtual void UpdateJump(IUnitMotion motion)
        {
            if (!motion.IsJumping) return;
            if (!motion.CanJump) return;
            
            bool jumpCooldown = this.m_JumpTime + motion.JumpCooldown < this.Character.Time.Time;
            if (!jumpCooldown) return;
            
            this.m_VerticalSpeed = motion.IsJumpingForce;
            this.m_JumpTime = this.Character.Time.Time;
            this.Character.OnJump(motion.IsJumpingForce);
        }

        protected virtual void UpdateGravity(IUnitMotion motion)
        {
            float gravity = this.WorldMoveDirection.y >= 0f 
                ? motion.GravityUpwards 
                : motion.GravityDownwards;

            gravity *= this.GravityInfluence;
            
            this.m_VerticalSpeed += gravity * this.Character.Time.DeltaTime;

            if (this.m_Controller.isGrounded)
            {
                if (this.Character.Time.Time - this.m_GroundTime > COYOTE_TIME &&
                    this.Character.Time.Frame - this.m_GroundFrame > COYOTE_FRAMES)
                {
                    this.Character.OnLand(this.m_VerticalSpeed);
                }
                
                this.m_GroundTime = this.Character.Time.Time;
                this.m_GroundFrame = this.Character.Time.Frame;

                this.m_VerticalSpeed = Mathf.Max(
                    this.m_VerticalSpeed, gravity
                );
            }

            this.m_VerticalSpeed = Mathf.Max(
                this.m_VerticalSpeed,
                motion.TerminalVelocity
            );
        }

        protected virtual void UpdateTranslation(IUnitMotion motion)
        {
            Vector3 movement = Vector3.up * (this.m_VerticalSpeed * this.Character.Time.DeltaTime);

            Vector3 kinetic = motion.MovementType switch
            {
                Character.MovementType.MoveToDirection => this.UpdateMoveToDirection(motion),
                Character.MovementType.MoveToPosition => this.UpdateMoveToPosition(motion),
                _ => Vector3.zero
            };

            Vector3 rootMotion = this.Character.Animim.RootMotionDeltaPosition;
            Vector3 translation = Vector3.Lerp(kinetic, rootMotion, this.Character.RootMotionPosition);
            
            movement += this.m_Axonometry?.ProcessTranslation(this, translation) ?? translation;

            if (this.m_FrameSlideFromCharacter >= Time.frameCount - 1)
            {
                float deltaSpeed = motion.LinearSpeed * this.Character.Time.DeltaTime;
                movement += this.m_SlideFromCharacter * deltaSpeed;
            }
            
            movement += this.m_AddTranslation.Consume();
            
            if (this.m_Controller.enabled)
            {
                this.m_Controller.Move(movement);
            }
        }

        // POSITION METHODS: ----------------------------------------------------------------------

        protected virtual Vector3 UpdateMoveToDirection(IUnitMotion motion)
        {
            this.m_MoveDirection = motion.MoveDirection;
            return this.m_MoveDirection * this.Character.Time.DeltaTime;
        }

        protected virtual Vector3 UpdateMoveToPosition(IUnitMotion motion)
        {
            float distance = Vector3.Distance(this.Character.Feet, motion.MovePosition);
            float brakeRadiusHeuristic = Math.Max(motion.Height, motion.Radius * 2f);
            float velocity = motion.MoveDirection.magnitude;
            
            if (distance < brakeRadiusHeuristic)
            {
                velocity = Mathf.Lerp(
                    motion.LinearSpeed, Mathf.Max(motion.LinearSpeed * 0.25f, 1f),
                    1f - Mathf.Clamp01(distance / brakeRadiusHeuristic)
                );
            }
            
            this.m_MoveDirection = motion.MoveDirection;
            return this.m_MoveDirection.normalized * (velocity * this.Character.Time.DeltaTime);
        }

        // INTERFACE METHODS: ---------------------------------------------------------------------

        public override void SetPosition(Vector3 position)
        {
            position += Vector3.up * (this.Character.Motion.Height * 0.5f);
            this.Transform.position = position;
            Physics.SyncTransforms();
        }

        public override void SetRotation(Quaternion rotation)
        {
            this.Transform.rotation = rotation;
            Physics.SyncTransforms();
        }

        public override void SetScale(Vector3 scale)
        {
            this.Transform.localScale = scale;
            Physics.SyncTransforms();
        }

        public override void AddPosition(Vector3 amount)
        {
            this.m_AddTranslation.Add(amount);
        }

        public override void AddRotation(Quaternion amount)
        {
            this.Transform.rotation *= amount;
            Physics.SyncTransforms();
        }

        public override void AddScale(Vector3 scale)
        {
            this.Transform.localScale += scale;
            Physics.SyncTransforms();
        }
        
        // GRAVITY METHODS: -----------------------------------------------------------------------

        public override void ResetVerticalVelocity()
        {
            this.m_VerticalSpeed = 0f;
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            this.m_FloorNormal.Target = hit.normal;
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            
            this.OnColliderHitPushRigidbodies(hit, angle);
            this.OnColliderHitSlideFromCharacters(hit, angle);
        }

        private void OnColliderHitSlideFromCharacters(ControllerColliderHit hit, float angle)
        {
            if (this.WorldMoveDirection.y > 0f || angle >= MAX_SLOPE_SLIDE_FROM_CHARACTER) return;
            
            Character other = hit.collider.Get<Character>();
            if (other == null) return;
            
            Vector3 slideDirection =
                Vector3.Scale(this.Character.transform.position, Vector3Plane.NormalUp) -
                Vector3.Scale(other.transform.position, Vector3Plane.NormalUp);

            slideDirection = slideDirection.sqrMagnitude > EPSILON_SLIDE_FROM_CHARACTER
                ? slideDirection.normalized
                : other.transform.forward;
            
            slideDirection.y = -1f;
                    
            this.m_SlideFromCharacter = slideDirection;
            this.m_FrameSlideFromCharacter = Time.frameCount;
        }

        private void OnColliderHitPushRigidbodies(ControllerColliderHit hit, float angle)
        {
            if (this.m_PushForce < float.Epsilon) return;
            
            if (angle > 90f) return;
            if (angle < 5f) return;

            Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
            if (!hitRigidbody || hitRigidbody.isKinematic) return;
            
            Vector3 force = hit.controller.velocity * this.m_PushForce;
            force /= this.Character.Time.FixedDeltaTime;

            hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Force);
        }
        
        private void OnStartRagdoll()
        {
            this.m_Controller.enabled = false;
            this.m_Controller.detectCollisions = false;
        }
        
        private void OnEndRagdoll()
        {
            this.m_Controller.enabled = true;
            this.m_Controller.detectCollisions = true;
            
            this.m_Controller.Move(Vector3.zero);
            this.m_MoveDirection = Vector3.zero;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmos(Character character)
        {
            if (!Application.isPlaying) return;

            IUnitMotion motion = character.Motion;
            if (motion == null) return;

            switch (motion.MovementType)
            {
                case Character.MovementType.MoveToPosition:
                    this.OnDrawGizmosToTarget(motion);
                    break;
            }
        }

        protected void OnDrawGizmosToTarget(IUnitMotion motion)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.Character.Feet, motion.MovePosition);
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Character Controller";
    }
}