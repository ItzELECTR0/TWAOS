using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemPeek : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemPeek).GetHashCode();
        
        private const float DEFAULT_PAN = 1f;
        private const float DEFAULT_TILT = 30f;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_Pan = DEFAULT_PAN;
        [SerializeField] private float m_Tilt = DEFAULT_TILT;

        [SerializeField] private InputPropertyValueVector2 m_Input = InputValueVector2MotionSecondary.Create();
        [SerializeField] private float m_SmoothTime = 0.15f;
        
        [SerializeField] private bool m_Restitute;
        
        [SerializeField] 
        private PropertyGetDecimal m_SensitivityX = GetDecimalDecimal.Create(60f);
        
        [SerializeField]
        private PropertyGetDecimal m_SensitivityY = GetDecimalDecimal.Create(60f);

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private AnimVector3 m_Value = new AnimVector3(Vector3.zero);
        
        [NonSerialized] private Vector3 m_GizmoPosition = Vector3.zero;
        [NonSerialized] private Vector3 m_GizmoDirection = Vector3.forward;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        // IMPLEMENTS: ----------------------------------------------------------------------------
        
        public override void OnAwake(TShotType shotType)
        {
            base.OnAwake(shotType);
            this.m_Input.OnStartup();
        }

        public override void OnDestroy(TShotType shotType)
        {
            base.OnDestroy(shotType);
            this.m_Input.OnDispose();
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            if (!shotType.IsActive) return;

            this.m_Input.OnUpdate();
            Vector2 input = this.m_Input.Read();

            switch (this.m_Restitute)
            {
                case true:
                    this.m_Value.Target = input;
                    break;
                    
                case false:
                    float sensitivityX = (float) this.m_SensitivityX.Get(shotType.Args);
                    float sensitivityY = (float) this.m_SensitivityY.Get(shotType.Args);
                    
                    this.m_Value.Target += new Vector3(
                        input.x * sensitivityX,
                        input.y * sensitivityY,
                        0f
                    );
                    break;
            }

            this.m_Value.Target = new Vector3(
                Mathf.Clamp(this.m_Value.Target.x, -1f, 1f),
                Mathf.Clamp(this.m_Value.Target.y, -1f, 1f),
                0f
            );

            this.m_Value.Smooth = Vector3.one * this.m_SmoothTime;
            this.m_Value.UpdateWithDelta(shotType.ShotCamera.TimeMode.DeltaTime);

            Vector2 value = Vector2.ClampMagnitude(
                new Vector2(this.m_Value.Current.x, this.m_Value.Current.y),
                1f
            );
            
            float x = (value.x + 1f) / 2f;
            float y = (value.y + 1f) / 2f;

            Vector3 position = new Vector3(
                Mathf.Lerp(-this.m_Pan, this.m_Pan, x),
                Mathf.Lerp(-this.m_Pan, this.m_Pan, y),
                0f
            );
            
            Quaternion rotation = Quaternion.Euler(
                Mathf.Lerp(-this.m_Tilt, this.m_Tilt, y),
                Mathf.Lerp(-this.m_Tilt, this.m_Tilt, x),
                0f
            );

            Vector3 direction = rotation * Vector3.forward;
            direction = shotType.Rotation * direction;

            this.m_GizmoPosition = shotType.ShotCamera.transform.position;
            this.m_GizmoDirection = shotType.ShotCamera.transform.forward;
            
            shotType.Position = shotType.ShotCamera.transform.TransformPoint(position);
            shotType.Rotation = Quaternion.LookRotation(direction);
        }

        public override void OnEnable(TShotType shotType, TCamera camera)
        {
            base.OnEnable(shotType, camera);
            
            this.m_Value.Target = Vector3.zero;
            this.m_Value.Current = Vector2.zero;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmosSelected(TShotType shotType, Transform transform)
        {
            base.OnDrawGizmosSelected(shotType, transform);
            this.DoDrawGizmos(shotType, GIZMOS_COLOR_ACTIVE, transform);
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

        private const float GIZMO_DISTANCE = 0.5f;

        private void DoDrawGizmos(TShotType shotType, Color color, Transform transform)
        {
            Gizmos.color = color;
            
            Vector3 position1 = Application.isPlaying
                ? this.m_GizmoPosition
                : transform.position;
            
            Vector3 position2 = Application.isPlaying
                ? this.m_GizmoPosition + this.m_GizmoDirection * GIZMO_DISTANCE
                : transform.TransformPoint(Vector3.forward * GIZMO_DISTANCE);
            
            Vector3 normal = Application.isPlaying
                ? this.m_GizmoDirection
                : transform.forward;

            float tiltRadians = this.m_Tilt * Mathf.Deg2Rad;
            float aperture = Mathf.Tan(tiltRadians) * GIZMO_DISTANCE;

            Gizmos.DrawLine(position1, position2);
            
            GizmosExtension.Circle(position1, this.m_Pan, normal);
            GizmosExtension.Circle(position2, this.m_Pan + aperture, normal);
        }
    }
}