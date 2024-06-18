using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TUnitAnimim : TUnit, IUnitAnimim
    {
        protected const float LAND_RECOVERY_SMOOTH_IN = 0.3f;
        protected const float LAND_RECOVERY_DURATION = 0.1f;
        protected const float LAND_RECOVERY_SMOOTH_OUT = 0.5f;

        protected const float MODEL_OFFSET_WEIGHT_SMOOTH = 5f;

        public static readonly int[] PHASES = 
        {
            Animator.StringToHash("Phase-0"),
            Animator.StringToHash("Phase-1"),
            Animator.StringToHash("Phase-2"),
            Animator.StringToHash("Phase-3"),
        };

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected Vector3 m_Position;
        [SerializeField] protected Vector3 m_Rotation = Vector3.zero;
        [SerializeField] protected Vector3 m_Scale = Vector3.one;

        [SerializeField] protected float m_SmoothTime = 0.5f;
        [SerializeField] protected Transform m_Mannequin;
        [SerializeField] protected Animator m_Animator;
        [SerializeField] protected State m_StartState;
        [SerializeField] protected Reaction m_Reaction;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private AnimimAnimatorProxy m_AnimatorProxy;
        
        [NonSerialized] private AnimVector3 m_Offset = new AnimVector3(
            Vector3.zero,
            MODEL_OFFSET_WEIGHT_SMOOTH
        );

        // PROPERTIES: ----------------------------------------------------------------------------

        public float SmoothTime
        {
            get => this.m_SmoothTime;
            set => this.m_SmoothTime = Math.Max(0f, value);
        }

        public Vector3 Position
        {
            get => this.m_Offset.Target;
            set => this.m_Offset.Target = value;
        }

        public Quaternion Rotation
        {
            get => Quaternion.Euler(this.m_Rotation);
            set => this.m_Rotation = value.eulerAngles;
        }

        public Vector3 Scale
        {
            get => this.m_Scale;
            set => this.m_Scale = value;
        }

        public Transform Mannequin
        {
            get
            {
                if (this.m_Mannequin == null) return this.Animator != null 
                    ? this.Animator.transform 
                    : null;

                return this.m_Mannequin;
            }
            set => this.m_Mannequin = value;
        }

        public Animator Animator
        {
            get => this.m_Animator;
            set => this.m_Animator = value;
        }

        public Reaction Reaction
        {
            get => this.m_Reaction;
            set => this.m_Reaction = value;
        }

        public Vector3 RootMotionDeltaPosition { get; private set; }
        public Quaternion RootMotionDeltaRotation { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int> EventOnAnimatorIK;

        // INITIALIZATION: ------------------------------------------------------------------------
        
        public virtual void OnStartup(Character character)
        {
            this.Character = character;
        }

        public virtual void AfterStartup(Character character)
        {
            this.Character = character;
            
            if (this.m_StartState != null)
            {
                _ = this.Character.States.SetState(
                    this.m_StartState, -1, BlendMode.Blend,
                    new ConfigState(0f, 1f, 1f, 0f, 0f)
                );
            }
        }
        
        public virtual void OnDispose(Character character)
        {
            this.Character = character;
            
            if (this.Character.Ragdoll.IsRagdoll)
            {
                if (this.m_Animator != null)
                {
                    UnityEngine.Object.Destroy(this.m_Animator.gameObject);
                }
            }
        }

        public virtual void OnEnable()
        {
            this.Character.EventLand += this.OnLand;
        }

        public virtual void OnDisable()
        {
            this.Character.EventLand -= this.OnLand;
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        public virtual void OnUpdate()
        {
            this.RequireAnimatorProxy();
            this.OnUpdateModelLocation();
        }

        public virtual void OnFixedUpdate()
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ApplyMannequinPosition()
        {
            if (this.Character.Ragdoll.IsRagdoll) return;
            
            Vector3 center = Vector3.up * (this.Character.Motion.Height * -0.5f);
            Vector3 offset = this.m_Offset.Current - Vector3.up * this.Character.Driver.SkinWidth;
            Vector3 position = center + offset + this.m_Position;
            
            this.Mannequin.localPosition = position;
        }

        public void ApplyMannequinRotation()
        {
            if (this.Character.Ragdoll.IsRagdoll) return;
            this.Mannequin.localRotation = Quaternion.Euler(this.m_Rotation);
        }
        
        public void ApplyMannequinScale()
        {
            if (this.Character.Ragdoll.IsRagdoll) return;
            this.Mannequin.localScale = this.m_Scale;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void RequireAnimatorProxy()
        {
            if (this.m_AnimatorProxy != null) return;

            this.m_AnimatorProxy = this.Animator.gameObject.AddComponent<AnimimAnimatorProxy>();
            this.m_AnimatorProxy.Animim = this;
        }

        private void OnUpdateModelLocation()
        {
            if (this.Character.Ragdoll.IsRagdoll) return;
            this.m_Offset.UpdateWithDelta(this.Character.Time.DeltaTime);
            
            this.ApplyMannequinPosition();
            this.ApplyMannequinRotation();
            this.ApplyMannequinScale();
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnLand(float velocity)
        {
            IUnitMotion motion = this.Character.Motion;
            float amount = Math.Abs(velocity) / (motion.JumpForce * 4f);
            
            motion.StandLevel.SetTransient(new AnimFloat.Transient(
                Mathf.Clamp01(motion.StandLevel.Current - amount),
                LAND_RECOVERY_SMOOTH_IN, 
                LAND_RECOVERY_DURATION, 
                LAND_RECOVERY_SMOOTH_OUT
            ));
        }
        
        public void OnAnimatorIK(int layerIndex)
        {
            this.EventOnAnimatorIK?.Invoke(layerIndex);
        }

        public void OnAnimatorMove()
        {
            this.Animator.applyRootMotion = true;

            this.RootMotionDeltaPosition = this.Animator.deltaPosition;
            this.RootMotionDeltaRotation = this.Animator.deltaRotation;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public virtual void OnDrawGizmos(Character character)
        { }
    }
}