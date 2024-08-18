using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class CameraTransition
    {
        private const float EPSILON = 0.005f;
        private const float DEFAULT_SMOOTH_TIME = 0.1f;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private ShotCamera m_CurrentShotCamera;
        [SerializeField] private float m_SmoothTimePosition;
        [SerializeField] private float m_SmoothTimeRotation;

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private TCamera m_Camera;
        [NonSerialized] private ShotCamera m_PreviousShotCamera;

        [NonSerialized] private float m_ChangeDuration;
        [NonSerialized] private float m_ChangeTime;

        [NonSerialized] private Vector3 m_PositionVelocity;
        [NonSerialized] private Quaternion m_RotationVelocity;

        [NonSerialized] private Vector3 m_PreviousCameraPosition;
        [NonSerialized] private Quaternion m_PreviousCameraRotation;

        [NonSerialized] private Easing.Type m_Easing = Easing.Type.QuadInOut;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public Vector3 Position { get; private set; }
        [field: NonSerialized] public Quaternion Rotation { get; private set; }
        
        public ShotCamera CurrentShotCamera
        {
            get => this.m_CurrentShotCamera;
            set => this.m_CurrentShotCamera = value;
        }

        public float SmoothTimePosition
        {
            get => this.m_SmoothTimePosition;
            set => this.m_SmoothTimePosition = value;
        }
        
        public float SmoothTimeRotation
        {
            get => this.m_SmoothTimeRotation;
            set => this.m_SmoothTimeRotation = value;
        }

        public ShotCamera PreviousShotCamera => m_PreviousShotCamera;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<ShotCamera> EventCut;
        public event Action<ShotCamera, float, Easing.Type> EventTransition;

        // INITIALIZERS: --------------------------------------------------------------------------

        internal CameraTransition()
        {
            this.m_SmoothTimePosition = DEFAULT_SMOOTH_TIME;
            this.m_SmoothTimeRotation = DEFAULT_SMOOTH_TIME;
        }

        internal void OnAwake(TCamera camera)
        {
            this.m_Camera = camera;
            Transform cameraTransform = this.m_Camera.transform;
            
            this.Position = cameraTransform.position;
            this.Rotation = cameraTransform.rotation;
        }

        internal void OnStart(TCamera camera)
        {
            if (this.m_CurrentShotCamera) this.ChangeToShot(this.CurrentShotCamera);
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        internal void NormalUpdate()
        {
            if (this.m_CurrentShotCamera == null) return;

            float elapsedTime = this.m_Camera.Time.Time - this.m_ChangeTime;
            float t = Mathf.Clamp01(this.m_ChangeDuration > float.Epsilon 
                ? elapsedTime / this.m_ChangeDuration 
                : 1f
            );

            this.Update(t, this.m_Camera.Time.DeltaTime);
        }

        internal void FixedUpdate()
        {
            if (this.m_CurrentShotCamera == null) return;
            
            float elapsedTime = this.m_Camera.Time.FixedTime - this.m_ChangeTime;
            float t = Mathf.Clamp01(this.m_ChangeDuration > float.Epsilon 
                ? elapsedTime / this.m_ChangeDuration 
                : 1f
            );

            this.Update(t, this.m_Camera.Time.FixedDeltaTime);
        }

        internal void Sync()
        {
            if (this.m_CurrentShotCamera == null) return;
            
            this.Position = this.m_CurrentShotCamera.Position;
            this.Rotation = this.m_CurrentShotCamera.Rotation;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Update(float t, float deltaTime)
        {
            float ease = t < 1f ? Easing.GetEase(this.m_Easing, 0f, 1f, t) : 1f;

            Vector3 position = Vector3.LerpUnclamped(
                this.m_PreviousCameraPosition, 
                this.m_CurrentShotCamera.Position, 
                ease
            );
            
            Quaternion rotation = Quaternion.LerpUnclamped(
                this.m_PreviousCameraRotation, 
                this.m_CurrentShotCamera.Rotation, 
                ease
            );

            this.UpdatePosition(position, deltaTime);
            this.UpdateRotation(rotation, deltaTime);
        }
        
        private void UpdatePosition(Vector3 position, float deltaTime)
        {
            if (this.m_CurrentShotCamera.UseSmoothPosition && this.Position != position)
            {
                this.Position = Vector3.SmoothDamp(
                    this.Position, position,
                    ref this.m_PositionVelocity,
                    this.m_SmoothTimePosition,
                    Mathf.Infinity,
                    deltaTime
                );
            }
            else
            {
                this.Position = position;
            }
        }

        private void UpdateRotation(Quaternion rotation, float deltaTime)
        {
            if (this.m_CurrentShotCamera.UseSmoothRotation && this.Rotation != rotation)
            {
                this.Rotation = QuaternionUtils.SmoothDamp(
                    this.Rotation, rotation,
                    ref this.m_RotationVelocity,
                    this.m_SmoothTimeRotation,
                    deltaTime
                );
            }
            else
            {
                this.Rotation = rotation;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ChangeToShot(ShotCamera shotCamera, float duration = 0f,
            Easing.Type easing = Easing.Type.QuadInOut)
        {
            this.m_Easing = easing;

            this.m_PreviousCameraPosition = this.m_Camera.transform.position;
            this.m_PreviousCameraRotation = this.m_Camera.transform.rotation;

            if (this.m_CurrentShotCamera != null)
            {
                this.m_CurrentShotCamera.OnDisableShot(this.m_Camera);
                this.m_PreviousShotCamera = this.m_CurrentShotCamera;
            }

            this.m_CurrentShotCamera = shotCamera;
            this.m_CurrentShotCamera.OnEnableShot(this.m_Camera);
            
            if (duration <= EPSILON)
            {
                this.Position = this.m_CurrentShotCamera.Position;
                this.Rotation = this.m_CurrentShotCamera.Rotation;
            }

            this.m_ChangeDuration = duration <= EPSILON ? 0f : duration;
            this.m_ChangeTime = this.m_Camera.Time.Time;
            
            if (duration <= EPSILON) this.EventCut?.Invoke(this.m_CurrentShotCamera);
            else this.EventTransition?.Invoke(this.m_CurrentShotCamera, duration, easing);
        }

        public void ChangeToPreviousShot(float duration = 0f)
        {
            this.ChangeToShot(this.m_PreviousShotCamera, duration);
        }
    }
}
