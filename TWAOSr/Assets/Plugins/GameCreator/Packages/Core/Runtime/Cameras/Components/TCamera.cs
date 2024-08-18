using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Cameras
{
    [AddComponentMenu("")]
    
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT_LATER)]
    
    [Serializable]
    public abstract class TCamera : MonoBehaviour
    {
        private enum RunMode
        {
            MainUpdate,
            FixedUpdate
        }

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private TimeMode m_TimeMode;
        [SerializeField] private RunMode m_RunIn = RunMode.MainUpdate;
        
        [SerializeField] private CameraViewport m_Viewport = new CameraViewport();
        [SerializeField] private CameraTransition m_Transition = new CameraTransition();
        
        [SerializeReference] private TCameraClip m_Clip = new CameraClipNone();

        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly CameraShakeSustain m_ShakeSustain = new CameraShakeSustain();
        private readonly CameraShakeBurst m_ShakeBurst = new CameraShakeBurst();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public CameraViewport Viewport => this.m_Viewport;
        public CameraTransition Transition => this.m_Transition;
        
        public TimeMode Time
        {
            get => this.m_TimeMode;
            set => this.m_TimeMode = value;
        }

        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action<ShotCamera> EventCut;
        public event Action<ShotCamera, float, Easing.Type> EventTransition;
        
        public event Action EventBeforeUpdate;
        public event Action EventAfterUpdate;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void Awake()
        {
            this.m_Transition.EventCut += this.EventCut;
            this.m_Transition.EventTransition += this.EventTransition;
            
            this.Transition.OnAwake(this);
        }

        private void Start()
        {
            this.Transition.OnStart(this);
        }

        private void OnEnable()
        {
            this.Viewport.OnEnable(this);
        }
        
        private void OnDisable()
        {
            this.Viewport.OnDisable(this);
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        private void LateUpdate()
        {
            this.EventBeforeUpdate?.Invoke();

            if (this.m_RunIn == RunMode.MainUpdate)
            {
                this.Transition.NormalUpdate();
                Transform cameraTransform = this.transform;
            
                cameraTransform.position = this.Transition.Position;
                cameraTransform.rotation = this.Transition.Rotation;
            }
            
            this.UpdateShakeEffect();
            this.UpdateAvoidClipping();

            this.EventAfterUpdate?.Invoke();
        }
        
        private void FixedUpdate()
        {
            if (this.m_RunIn == RunMode.FixedUpdate)
            {
                this.Transition.FixedUpdate();
                Transform cameraTransform = this.transform;
            
                cameraTransform.position = this.Transition.Position;
                cameraTransform.rotation = this.Transition.Rotation;
            }
        }

        private void UpdateShakeEffect()
        {
            this.m_ShakeSustain.Update(this);
            this.m_ShakeBurst.Update(this);
            
            Vector3 shakeDeltaPosition = 
                this.m_ShakeSustain.AdditivePosition * ShakeEffect.COEF_SHAKE_POSITION +
                this.m_ShakeBurst.AdditivePosition * ShakeEffect.COEF_SHAKE_POSITION;
            
            Vector3 shakeDeltaRotation =
                this.m_ShakeSustain.AdditiveRotation * ShakeEffect.COEF_SHAKE_ROTATION +
                this.m_ShakeBurst.AdditiveRotation * ShakeEffect.COEF_SHAKE_ROTATION;

            Transform cameraTransform = this.transform;
            
            cameraTransform.localPosition += shakeDeltaPosition;
            cameraTransform.localEulerAngles += shakeDeltaRotation;
        }
        
        private void UpdateAvoidClipping()
        {
            if (this.Transition.CurrentShotCamera == null) return;
            if (this.Transition.CurrentShotCamera.HasTarget == false) return;
            
            Vector3 point = this.Transition.CurrentShotCamera.Target;
            Transform[] ignore = this.Transition.CurrentShotCamera.Ignore;
            
            this.transform.position = this.m_Clip.Update(this, point, ignore);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddSustainShake(int layer, float delay, float transition, ShakeEffect shakeEffect)
        {
            this.m_ShakeSustain.AddSustain(layer, delay, transition, shakeEffect);
        }
        
        public void RemoveSustainShake(int layer, float delay, float transition)
        {
            this.m_ShakeSustain.RemoveSustain(layer, delay, transition);            
        }

        public void AddBurstShake(float delay, float duration, ShakeEffect shakeEffect)
        {
            this.m_ShakeBurst.AddBurst(delay, duration, shakeEffect);
        }

        public void StopBurstShakes(float delay, float transition)
        {
            this.m_ShakeBurst.RemoveBursts(delay, transition);
        }

        public void Sync()
        {
            this.m_Transition.Sync();
        }

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            this.m_Clip?.OnDrawGizmos(this);
        }
    }   
}
