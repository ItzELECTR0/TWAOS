using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemFirstPerson : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemFirstPerson).GetHashCode();

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private Bone m_Mount = new Bone(HumanBodyBones.Head);

        [SerializeField]
        private InputPropertyValueVector2 m_InputRotate = InputValueVector2MotionSecondary.Create();
        
        [SerializeField]
        private PropertyGetDecimal m_SensitivityX = GetDecimalDecimal.Create(5f);
        
        [SerializeField]
        private PropertyGetDecimal m_SensitivityY = GetDecimalDecimal.Create(5f);

        [SerializeField, Range(1f, 179f)] private float m_MaxPitch = 60f;
        [SerializeField] private EnablerAngle180 m_MaxYaw = new EnablerAngle180(false, 120f);
        
        [SerializeField] private float m_SmoothTime = 0.1f;

        // MEMBERS: -------------------------------------------------------------------------------

        private Vector3 m_LastTargetPosition = Vector3.zero;
        
        private Vector2 m_CurrentRotation = new Vector2(0, 0f);
        private Vector2 m_TargetRotation = new Vector2(0, 0f);

        private float m_VelocityX;
        private float m_VelocityY;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public Vector2 Sensitivity
        {
            set
            {
                this.m_SensitivityX = new PropertyGetDecimal(value.x);
                this.m_SensitivityY = new PropertyGetDecimal(value.y);
            }
        }
        
        public float SmoothTime
        {
            get => this.m_SmoothTime;
            set => this.m_SmoothTime = value;
        }
        
        public float MaxPitch
        {
            get => this.m_MaxPitch;
            set => this.m_MaxPitch = value;
        }
        
        public float Pitch
        {
            get => this.m_TargetRotation.x;
            set => this.m_TargetRotation.x = value;
        }
        
        public float Yaw
        {
            get => this.m_TargetRotation.y;
            set => this.m_TargetRotation.y = value;
        }

        public GameObject Target
        {
            set => this.m_Character = GetGameObjectInstance.Create(value);
        }
        
        public Bone Bone
        {
            get => this.m_Mount;
            set => this.m_Mount = value;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetRotation(Quaternion rotation)
        {
            this.m_TargetRotation = rotation.eulerAngles;
            this.m_CurrentRotation = rotation.eulerAngles;
        }

        public void SetDirection(Vector3 direction)
        {
            this.SetRotation(Quaternion.LookRotation(direction, Vector3.up));
        }
        
        public Character GetTarget(IShotType shotType)
        {
            return this.m_Character.Get<Character>(shotType.ShotCamera);
        }

        // IMPLEMENTS: ----------------------------------------------------------------------------
        
        public override void OnAwake(TShotType shotType)
        {
            base.OnAwake(shotType);
            this.m_InputRotate.OnStartup();
        }

        public override void OnDestroy(TShotType shotType)
        {
            base.OnDestroy(shotType);
            this.m_InputRotate.OnDispose();
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            this.m_InputRotate.OnUpdate();

            if (shotType.IsActive)
            {
                double sensitivityX = this.m_SensitivityX.Get(shotType.Args);
                double sensitivityY = this.m_SensitivityY.Get(shotType.Args);
                
                Vector2 deltaInput = this.m_InputRotate.Read();
                
                this.ComputeInput(new Vector2(
                    deltaInput.x * (float) sensitivityX,
                    deltaInput.y * (float) sensitivityY
                ));
            }

            this.ConstrainPitch();
            this.ConstrainYaw(shotType);

            this.m_CurrentRotation = new Vector2(
                this.GetRotationDamp(
                    this.m_CurrentRotation.x, 
                    this.m_TargetRotation.x,
                    ref this.m_VelocityX,
                    this.m_SmoothTime,
                    shotType.ShotCamera.TimeMode.DeltaTime),
                this.GetRotationDamp(
                    this.m_CurrentRotation.y, 
                    this.m_TargetRotation.y, 
                    ref this.m_VelocityY,
                    this.m_SmoothTime,
                    shotType.ShotCamera.TimeMode.DeltaTime)
            );

            Vector3 position = this.GetTargetPosition(shotType);
            Quaternion rotation = Quaternion.Euler(
                this.m_CurrentRotation.x,
                this.m_CurrentRotation.y, 
                0f
            );

            shotType.Position = position;
            shotType.Rotation = rotation;
            
            this.m_LastTargetPosition = position;
        }

        public override void OnEnable(TShotType shotType, TCamera camera)
        {
            base.OnEnable(shotType, camera);
            this.SetRotation(camera.transform.rotation);
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

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private float GetRotationDamp(float current, float target, ref float velocity, 
            float smoothTime, float deltaTime)
        {
            return Mathf.SmoothDampAngle(
                current,
                target,
                ref velocity,
                smoothTime,
                Mathf.Infinity,
                deltaTime
            );
        }
        
        private void ConstrainPitch()
        {
            float maxPitch = this.m_MaxPitch * 0.5f;
            this.m_TargetRotation.x = Mathf.Clamp(
                this.m_TargetRotation.x,
                -maxPitch,
                maxPitch
            );
        }
        
        private void ConstrainYaw(TShotType shotType)
        {
            if (this.m_MaxYaw.IsEnabled)
            {
                float maxYaw = this.m_MaxYaw.Value * 0.5f;
                Character character = this.m_Character.Get<Character>(shotType.Args);
                
                if (character != null)
                {
                    float characterYaw = character.transform.rotation.eulerAngles.y;
                    float worldYaw = this.m_TargetRotation.y;

                    float localYaw = QuaternionUtils.ClampAngle(
                        worldYaw - characterYaw,
                        -maxYaw,
                        maxYaw
                    );
                    
                    this.m_TargetRotation.y = localYaw + characterYaw;
                }
            }

            this.m_TargetRotation.y = Mathf.Repeat(this.m_TargetRotation.y, 360f);
        }
        
        private Vector3 GetTargetPosition(TShotType shotType)
        {
            Character target = this.m_Character.Get<Character>(shotType.Args);
            if (target == null) return this.m_LastTargetPosition;

            Animator animator = target.Animim.Animator;
            if (animator == null) return this.m_LastTargetPosition;

            Transform mount = this.m_Mount.GetTransform(animator);
            return mount != null ? mount.position : this.m_LastTargetPosition;
        }

        private void ComputeInput(Vector2 deltaInput)
        {
            this.m_TargetRotation += new Vector2(
                deltaInput.y,
                deltaInput.x
            );
        }
    }
}