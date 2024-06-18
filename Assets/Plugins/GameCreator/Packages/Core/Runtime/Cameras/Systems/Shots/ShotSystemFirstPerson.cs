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

        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetDirection m_Offset = GetDirectionLocalValue.CreateTarget(
            new Vector3(0f, 0.75f, 0f)
        );

        [SerializeField]
        private InputPropertyValueVector2 m_InputRotate = InputValueVector2MotionSecondary.Create();
        
        [SerializeField]
        private PropertyGetDecimal m_InputSensitivityX = GetDecimalDecimal.Create(180f);
        
        [SerializeField]
        private PropertyGetDecimal m_InputSensitivityY = GetDecimalDecimal.Create(180f);

        [SerializeField, Range(1f, 179f)] private float m_MaxPitch = 60f;
        [SerializeField] private float m_SmoothTime = 0.1f;

        // MEMBERS: -------------------------------------------------------------------------------

        private Vector3 m_LastTargetPosition = Vector3.zero;
        
        private Vector2 m_OrbitAnglesCurrent = new Vector2(0, 0f);
        private Vector2 m_OrbitAnglesTarget = new Vector2(0, 0f);

        private float m_VelocityX;
        private float m_VelocityY;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public Vector2 Sensitivity
        {
            set
            {
                this.m_InputSensitivityX = new PropertyGetDecimal(value.x);
                this.m_InputSensitivityY = new PropertyGetDecimal(value.y);
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
            get => this.m_OrbitAnglesTarget.x;
            set => this.m_OrbitAnglesTarget.x = value;
        }
        
        public float Yaw
        {
            get => this.m_OrbitAnglesTarget.y;
            set => this.m_OrbitAnglesTarget.y = value;
        }

        public GameObject Target
        {
            set => this.m_Target = GetGameObjectInstance.Create(value);
        }
        
        public Vector3 Offset
        {
            set => this.m_Offset = GetDirectionLocalValue.CreateTarget(value);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetRotation(Quaternion rotation)
        {
            this.m_OrbitAnglesTarget = rotation.eulerAngles;
            this.m_OrbitAnglesCurrent = rotation.eulerAngles;
        }

        public void SetDirection(Vector3 direction)
        {
            this.SetRotation(Quaternion.LookRotation(direction, Vector3.up));
        }
        
        public Character GetTarget(IShotType shotType)
        {
            return this.m_Target.Get<Character>(shotType.ShotCamera);
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
                double sensitivityX = this.m_InputSensitivityX.Get(shotType.Args);
                double sensitivityY = this.m_InputSensitivityY.Get(shotType.Args);
                
                Vector2 deltaInput = this.m_InputRotate.Read();
                
                this.ComputeInput(new Vector2(
                    deltaInput.x * shotType.ShotCamera.TimeMode.DeltaTime * (float) sensitivityX,
                    deltaInput.y * shotType.ShotCamera.TimeMode.DeltaTime * (float) sensitivityY
                ));
            }

            this.ConstrainTargetAngles();

            this.m_OrbitAnglesCurrent = new Vector2(
                this.GetRotationDamp(
                    this.m_OrbitAnglesCurrent.x, 
                    this.m_OrbitAnglesTarget.x,
                    ref this.m_VelocityX,
                    this.m_SmoothTime,
                    shotType.ShotCamera.TimeMode.DeltaTime),
                this.GetRotationDamp(
                    this.m_OrbitAnglesCurrent.y, 
                    this.m_OrbitAnglesTarget.y, 
                    ref this.m_VelocityY,
                    this.m_SmoothTime,
                    shotType.ShotCamera.TimeMode.DeltaTime)
            );

            Vector3 position = this.GetTargetPosition(shotType);
            Quaternion rotation = Quaternion.Euler(
                this.m_OrbitAnglesCurrent.x,
                this.m_OrbitAnglesCurrent.y, 
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
        
        private void ConstrainTargetAngles()
        {
            float angle = this.m_MaxPitch / 2f;
            m_OrbitAnglesTarget.x = Mathf.Clamp(m_OrbitAnglesTarget.x, -angle, angle);

            if (m_OrbitAnglesTarget.y < 0f) m_OrbitAnglesTarget.y += 360f;
            if (m_OrbitAnglesTarget.y >= 360f) m_OrbitAnglesTarget.y -= 360f;
        }
        
        private Vector3 GetTargetPosition(TShotType shotType)
        {
            Character target = this.m_Target.Get<Character>(shotType.Args);
            if (target == null) return this.m_LastTargetPosition;
            
            return target.transform.position + this.m_Offset.Get(shotType.Args);
        }

        private void ComputeInput(Vector2 deltaInput)
        {
            this.m_OrbitAnglesTarget += new Vector2(
                deltaInput.y,
                deltaInput.x
            );
        }
    }
}