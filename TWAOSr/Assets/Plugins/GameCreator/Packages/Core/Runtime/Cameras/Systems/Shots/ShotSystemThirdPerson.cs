using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemThirdPerson : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemThirdPerson).GetHashCode();
        
        private const float MIN_RADIUS = 0.01f;
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // CLASSES: -------------------------------------------------------------------------------
        
        [Serializable]
        public class Align
        {
            // EXPOSED MEMBERS: -------------------------------------------------------------------
            
            [SerializeField] private bool m_AutoAlign;
            [SerializeField] private float m_Delay = 3f;
            [SerializeField] private float m_SmoothTime = 3f;
            
            // PROPERTIES: ------------------------------------------------------------------------
            
            public bool AutoAlign
            {
                get => this.m_AutoAlign;
                set => this.m_AutoAlign = value;
            }
        
            public float Delay
            {
                get => this.m_Delay;
                set => this.m_Delay = value;
            }
        
            public float SmoothTime
            {
                get => this.m_SmoothTime;
                set => this.m_SmoothTime = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Pivot = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetDecimal m_Shoulder = GetDecimalDecimal.Create(0f);
        [SerializeField] private PropertyGetDecimal m_Lift = GetDecimalDecimal.Create(0.5f);
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(5f);
        
        [SerializeField]
        private InputPropertyValueVector2 m_InputRotate = InputValueVector2MotionSecondary.Create();
        
        [SerializeField]
        private PropertyGetDecimal m_SensitivityX = GetDecimalDecimal.Create(5f);
        
        [SerializeField]
        private PropertyGetDecimal m_SensitivityY = GetDecimalDecimal.Create(5f);
        
        [SerializeField, Range(1f, 179f)] private float m_MaxPitch = 60f;
        [SerializeField] private EnablerAngle180 m_MaxYaw = new EnablerAngle180(false, 120f);

        [SerializeField]
        private PropertyGetDecimal m_SmoothTime = GetDecimalDecimal.Create(0.15f);

        [SerializeField] private Align m_Align = new Align();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private ShotSystemZoom m_Zoom;
        [NonSerialized] private ThirdPersonAim m_Aim;
        
        [NonSerialized] private Vector2 m_CurrentRotation = Vector2.zero;
        [NonSerialized] private Vector2 m_TargetRotation = Vector2.zero;
        
        [NonSerialized] private float m_VelocityRotationX;
        [NonSerialized] private float m_VelocityRotationY;
        [NonSerialized] private float m_VelocityAlign;
        
        [NonSerialized] private float m_LastInputTime;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;
        
        [field: NonSerialized] public GameObject Pivot { get; private set; }
        
        [field: NonSerialized] public Vector3 PivotPosition { get; private set; }
        [field: NonSerialized] public float Radius { get; private set; }

        public Vector2 Sensitivity
        {
            set
            {
                this.m_SensitivityX = new PropertyGetDecimal(value.x);
                this.m_SensitivityY = new PropertyGetDecimal(value.y);
            }
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
        
        public Align Alignment => this.m_Align;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetRotation(Vector2 rotation)
        {
            rotation.x = QuaternionUtils.Convert180(rotation.x);
            rotation.y = QuaternionUtils.Convert180(rotation.y);
            
            this.m_CurrentRotation = rotation;
            this.m_TargetRotation = rotation;
            
            this.m_VelocityRotationX = 0f;
            this.m_VelocityRotationY = 0f;
        }
        
        public void SetRotation(Quaternion rotation)
        {
            Vector2 angles = new Vector2(rotation.eulerAngles.x, rotation.eulerAngles.y);
            this.SetRotation(angles);
        }
        
        public void SetDirection(Vector3 direction)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            this.SetRotation(rotation);
        }

        public void Aim(float shoulder, float lift, float radius, float duration)
        {
            this.m_Aim.Switch(shoulder, lift, radius, Quaternion.identity, duration);
        }
        
        public void Aim(float shoulder, float lift, float radius, Vector3 focus, float duration)
        {
            if (this.PivotPosition == focus) return;

            Quaternion currentRotation = Quaternion.Euler(this.m_CurrentRotation) * this.m_Aim.Aim; 
            Vector3 currentDirection = currentRotation * Vector3.forward;
            Vector3 currentHAxis = Vector3.Cross(Vector3.up, currentDirection).normalized;
            if (currentHAxis == Vector3.zero) return;
            
            Vector3 newPivotPosition = this.PivotPosition;
            newPivotPosition += currentHAxis * (shoulder - this.m_Aim.Shoulder);
            newPivotPosition += Vector3.up * (lift - this.m_Aim.Lift);
            
            Vector3 nextDirection = (focus - newPivotPosition).normalized;
            if (nextDirection == Vector3.zero) return;
            
            Quaternion nextRotation = Quaternion.LookRotation(nextDirection);
            this.SetRotation(nextRotation);
            
            Quaternion offset = currentRotation * Quaternion.Inverse(nextRotation);
            this.m_Aim.Switch(shoulder, lift, radius, offset, duration);
        }

        // IMPLEMENTS: ----------------------------------------------------------------------------
        
        public override void OnAwake(TShotType shotType)
        {
            base.OnAwake(shotType);

            this.m_Zoom = shotType.GetSystem(ShotSystemZoom.ID) as ShotSystemZoom;
            this.m_InputRotate.OnStartup();

            this.m_Aim = new ThirdPersonAim(
                (float) this.m_Shoulder.Get(shotType.Args),
                (float) this.m_Lift.Get(shotType.Args),
                (float) this.m_Radius.Get(shotType.Args),
                shotType as ShotTypeThirdPerson
            );
        }
        
        public override void OnEnable(TShotType shotType, TCamera camera)
        {
            base.OnEnable(shotType, camera);
            this.SetRotation(camera.transform.rotation);
        }

        public override void OnDestroy(TShotType shotType)
        {
            base.OnDestroy(shotType);
            this.m_InputRotate.OnDispose();
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            
            this.m_Aim.Update();
            
            this.Pivot = this.m_Pivot.Get(shotType.Args);
            this.UpdateInput(shotType);
            
            this.CalculateDistance();
            this.CalculatePositions();

            this.UpdateOrbit(shotType);
            this.UpdateLook(shotType);
        }

        // PRIVATE UPDATE METHODS: ----------------------------------------------------------------
        
        private void UpdateInput(TShotType shotType)
        {
            this.m_InputRotate.OnUpdate();
            
            if (shotType.IsActive)
            {
                double sensitivityX = this.m_SensitivityX.Get(shotType.Args);
                double sensitivityY = this.m_SensitivityY.Get(shotType.Args);
                
                Vector2 deltaInput = this.m_InputRotate.Read();
                
                if (deltaInput == Vector2.zero)
                {
                    this.ComputeAlign(shotType);
                }
                else
                {
                    this.ComputeInput(shotType, new Vector2(
                        deltaInput.x * (float) sensitivityX,
                        deltaInput.y * (float) sensitivityY
                    ));
                }
            }

            this.ConstrainPitch();
            this.ConstrainYaw();

            float smoothTime = (float) this.m_SmoothTime.Get(shotType.Args);
            
            this.m_CurrentRotation = new Vector2(
                this.GetRotationDamp(
                    this.m_CurrentRotation.x, 
                    this.m_TargetRotation.x,
                    ref this.m_VelocityRotationX,
                    smoothTime,
                    shotType.ShotCamera.TimeMode.DeltaTime),
                this.GetRotationDamp(
                    this.m_CurrentRotation.y, 
                    this.m_TargetRotation.y, 
                    ref this.m_VelocityRotationY,
                    smoothTime,
                    shotType.ShotCamera.TimeMode.DeltaTime)
            );
        }
        
        private void UpdateOrbit(TShotType shotType)
        {
            Quaternion rotation = Quaternion.Euler(this.m_CurrentRotation) * this.m_Aim.Aim;
            Vector3 direction = rotation * Vector3.forward;
            Vector3 position = this.PivotPosition - direction * this.Radius;
            
            shotType.Position = position;
        }
        
        private void UpdateLook(TShotType shotType)
        {
            Quaternion rotation = Quaternion.Euler(this.m_CurrentRotation) * this.m_Aim.Aim;
            shotType.Rotation = Quaternion.Euler(
                rotation.eulerAngles.x,
                rotation.eulerAngles.y,
                0f
            );
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void CalculateDistance()
        {
            float minDistance = this.m_Zoom.MinDistance;
            
            float maxRadius = Mathf.Max(MIN_RADIUS, this.m_Aim.Radius - minDistance);
            this.Radius = minDistance + maxRadius * this.m_Zoom.Level;
        }
        
        private bool CalculatePositions()
        {
            if (this.Pivot == null) return false;
            
            Transform pivotTransform = this.Pivot.transform;

            Quaternion rotation = Quaternion.Euler(this.m_CurrentRotation) * this.m_Aim.Aim;
            Vector3 direction = rotation * Vector3.forward;
            Vector3 horizontalAxis = Vector3.Cross(Vector3.up, direction).normalized;
            
            this.PivotPosition = pivotTransform.position;
            this.PivotPosition += horizontalAxis * this.m_Aim.Shoulder;
            this.PivotPosition += Vector3.up * this.m_Aim.Lift;
            
            return true;
        }
        
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
        
        private void ConstrainYaw()
        {
            if (this.m_MaxYaw.IsEnabled)
            {
                float maxYaw = this.m_MaxYaw.Value * 0.5f;
                
                if (this.Pivot != null)
                {
                    float pivotYaw = this.Pivot.transform.rotation.eulerAngles.y;
                    float worldYaw = this.m_TargetRotation.y;
                    
                    float localYaw = QuaternionUtils.ClampAngle(
                        worldYaw - pivotYaw,
                        -maxYaw,
                        maxYaw
                    );
                    
                    this.m_TargetRotation.y = localYaw + pivotYaw;
                }
                
                this.m_TargetRotation.y = Mathf.Repeat(this.m_TargetRotation.y, 360f);
            }
        }
        
        private void ComputeAlign(TShotType shotType)
        {
            float time = shotType.ShotCamera.TimeMode.Time;
            
            if (this.m_Align.AutoAlign && time - this.m_LastInputTime > this.m_Align.Delay)
            {
                if (this.Pivot != null)
                {
                    Quaternion pivotRotation = this.Pivot.transform.rotation;
                    this.m_TargetRotation.y = this.GetRotationDamp(
                        this.m_TargetRotation.y,
                        pivotRotation.eulerAngles.y,
                        ref this.m_VelocityAlign,
                        this.m_Align.SmoothTime,
                        shotType.ShotCamera.TimeMode.DeltaTime
                    );
                }
            }
        }
        
        private void ComputeInput(IShotType shotType, Vector2 deltaInput)
        {
            this.m_LastInputTime = shotType.ShotCamera.TimeMode.Time;
            this.m_VelocityAlign = 0f;

            this.m_TargetRotation += new Vector2(
                deltaInput.y,
                deltaInput.x
            );
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