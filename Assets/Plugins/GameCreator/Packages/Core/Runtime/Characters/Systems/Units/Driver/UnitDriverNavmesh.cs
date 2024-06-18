using System;
using UnityEngine;
using UnityEngine.AI;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("NavMesh Agent")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Red, typeof(OverlayArrowRight))]
    
    [Category("NavMesh Agent")]
    [Description(
        "Moves the Character using Unity's Navigation Mesh Agent. " +
        "Requires a scene with a baked navigation mesh"
    )]
    
    [Serializable]
    public class UnitDriverNavmesh : TUnitDriver
    {
        private const ObstacleAvoidanceType DEFAULT_QUALITY =
            ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private ObstacleAvoidanceType m_AvoidQuality = DEFAULT_QUALITY;
        [SerializeField] private int m_AvoidPriority = 50;
        
        [SerializeField] private bool m_AutoMeshLink = true;
        [SerializeField] private DriverNavmeshAgentType m_AgentType = new DriverNavmeshAgentType();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] protected NavMeshAgent m_Agent;
        [NonSerialized] protected CapsuleCollider m_Capsule;

        [NonSerialized] protected Vector3 m_MoveDirection;

        [NonSerialized] private Vector3 m_Velocity = Vector3.zero;
        [NonSerialized] private Vector3 m_PreviousPosition = Vector3.zero;

        [NonSerialized] private DriverAdditionalTranslation m_AddTranslation;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public override Vector3 WorldMoveDirection => this.m_Velocity;
        public override Vector3 LocalMoveDirection => this.Transform.InverseTransformDirection(
            this.WorldMoveDirection
        );
        
        public override float SkinWidth => 0f;
        public override bool IsGrounded => this.m_Agent.isOnNavMesh;
        public override Vector3 FloorNormal => Vector3.up;
        
        public override bool Collision
        {
            get => this.m_Capsule.enabled;
            set => this.m_Capsule.enabled = value;
        }
        
        public override Axonometry Axonometry
        {
            get => null;
            set => _ = value;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public UnitDriverNavmesh()
        {
            this.m_MoveDirection = Vector3.zero;
        }

        public override void OnDispose(Character character)
        {
            base.OnDispose(character);
            UnityEngine.Object.Destroy(this.m_Agent);
        }

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);

            this.m_Agent = this.Character.GetComponent<NavMeshAgent>();
            if (this.m_Agent == null)
            {
                GameObject instance = this.Character.gameObject;
                this.m_Agent = instance.AddComponent<NavMeshAgent>();
                this.m_Agent.hideFlags = HideFlags.HideInInspector;
            }

            this.m_Agent.updatePosition = true;
            this.m_Agent.updateRotation = false;
            this.m_Agent.updateUpAxis = false;

            this.m_Agent.autoBraking = false;
            this.m_Agent.autoRepath = false;
            this.m_Agent.agentTypeID = this.m_AgentType.AgentType;

            this.m_Capsule = this.Character.GetComponent<CapsuleCollider>();
            if (this.m_Capsule == null)
            {
                GameObject instance = this.Character.gameObject;
                this.m_Capsule = instance.AddComponent<CapsuleCollider>();
                this.m_Capsule.hideFlags = HideFlags.HideInInspector;
            }
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public override void OnUpdate()
        {
            if (this.Character.IsDead) return;
            
            this.UpdateProperties(this.Character.Motion);
            this.UpdateTranslation(this.Character.Motion);
        }

        protected virtual void UpdateProperties(IUnitMotion motion)
        {
            this.m_MoveDirection = Vector3.zero;

            this.m_Agent.speed = motion.LinearSpeed;
            this.m_Agent.angularSpeed = motion.AngularSpeed;

            this.m_Agent.acceleration = motion.UseAcceleration switch
            {
                true => (motion.Acceleration + motion.Deceleration) / 2f,
                false => 9999f
            };

            this.m_Agent.radius = motion.Radius;
            this.m_Agent.height = motion.Height;

            if (Math.Abs(this.m_Capsule.height - motion.Height) > float.Epsilon)
            {
                this.m_Capsule.height = motion.Height;   
            }
            
            if (Math.Abs(this.m_Capsule.radius - motion.Radius) > float.Epsilon)
            {
                this.m_Capsule.radius = motion.Radius;
            }
            
            if (this.m_Capsule.center != Vector3.zero)
            {
                this.m_Capsule.center = Vector3.zero;
            }
            
            this.m_Agent.baseOffset = this.m_Agent.height / 2f;
            this.m_Agent.autoTraverseOffMeshLink = this.m_AutoMeshLink;

            this.m_Agent.obstacleAvoidanceType = this.m_AvoidQuality;
            this.m_Agent.avoidancePriority = this.m_AvoidPriority;
        }

        protected virtual void UpdateTranslation(IUnitMotion motion)
        {
            if (!this.m_Agent.isOnNavMesh)
            {
                Debug.LogWarning("No Navigation Mesh bound to Agent", this.Character.gameObject);
                return;
            }

            if (this.Character.RootMotionPosition > 0.5f)
            {
                this.m_Agent.autoBraking = false;
                this.m_Agent.autoRepath = false;
                
                this.m_Agent.velocity = Vector3.zero;
                this.m_Agent.isStopped = true;

                this.m_MoveDirection = this.Character.Animim.RootMotionDeltaPosition;
                this.m_Agent.Move(this.m_MoveDirection);
            }
            else
            {
                switch (motion.MovementType)
                {
                    case Character.MovementType.MoveToDirection:
                        this.m_Agent.autoBraking = false;
                        this.m_Agent.velocity = Vector3.zero;

                        Vector3 movement = this.UpdateMoveToDirection(motion);
                        this.m_Agent.Move(movement);
                        break;

                    case Character.MovementType.MoveToPosition:
                        this.m_Agent.autoBraking = true;
                        this.UpdateMoveToPosition(motion);
                        break;
                    
                    case Character.MovementType.None:
                        this.m_Agent.autoBraking = true;
                        this.m_Agent.autoRepath = false;
                        this.m_Agent.isStopped = true;
                        break;
                    
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            Vector3 additionalTranslation = this.m_AddTranslation.Consume();
            if (additionalTranslation != Vector3.zero) this.m_Agent.Move(additionalTranslation);

            Vector3 currentPosition = this.Transform.position;
            
            this.m_Velocity = 
                Vector3.Normalize(currentPosition - this.m_PreviousPosition) *
                this.m_MoveDirection.magnitude;

            this.m_PreviousPosition = currentPosition;
        }

        // POSITION METHODS: ----------------------------------------------------------------------

        protected virtual Vector3 UpdateMoveToDirection(IUnitMotion motion)
        {
            this.m_Agent.autoRepath = false;
            this.m_Agent.isStopped = true;

            this.m_MoveDirection = motion.MoveDirection;
            return this.m_MoveDirection * this.Character.Time.DeltaTime;
        }

        protected virtual void UpdateMoveToPosition(IUnitMotion motion)
        {
            this.m_Agent.autoRepath = true;
            this.m_Agent.isStopped = false;

            this.m_Agent.SetDestination(motion.MovePosition);
            this.m_MoveDirection = this.m_Agent.velocity;
        }

        // INTERFACE METHODS: ---------------------------------------------------------------------

        public override void SetPosition(Vector3 position)
        {
            this.m_Agent.Warp(position);
        }

        public override void SetRotation(Quaternion rotation)
        {
            this.Transform.rotation = rotation;
        }

        public override void SetScale(Vector3 scale)
        {
            this.Transform.localScale = scale;
        }

        public override void AddPosition(Vector3 amount)
        {
            this.m_AddTranslation.Add(amount);
        }

        public override void AddRotation(Quaternion amount)
        {
            this.Transform.rotation *= amount;
        }
        
        public override void AddScale(Vector3 scale)
        {
            this.Transform.localScale += scale;
        }

        // OTHER METHODS: -------------------------------------------------------------------------

        public void ChangeHeight(float height)
        {
            this.m_Agent.height = height;
        }
        
        // GRAVITY METHODS: -----------------------------------------------------------------------

        public override void ResetVerticalVelocity()
        { }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmos(Character character)
        {
            if (!Application.isPlaying) return;

            switch (character.Motion.MovementType)
            {
                case Character.MovementType.MoveToPosition:
                    this.OnDrawGizmosToTarget();
                    break;
            }
        }

        protected void OnDrawGizmosToTarget()
        {
            if (this.m_Agent == null || !this.m_Agent.hasPath || this.m_Agent.isPathStale) return;

            switch (this.m_Agent.path.status)
            {
                case NavMeshPathStatus.PathComplete: Gizmos.color = Color.yellow; break;
                case NavMeshPathStatus.PathPartial: Gizmos.color = Color.red; break;
                case NavMeshPathStatus.PathInvalid: Gizmos.color = Color.grey; break;
            }

            Vector3[] corners = this.m_Agent.path.corners;
            if (corners.Length <= 1) return;

            for (int i = 1; i < corners.Length; i++)
            {
                Gizmos.DrawLine(corners[i - 1], corners[i]);
            }
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Navmesh Agent";
    }
}