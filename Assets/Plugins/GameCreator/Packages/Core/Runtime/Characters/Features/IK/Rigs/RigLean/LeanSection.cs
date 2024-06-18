using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    internal class LeanSection
    {
        private const float UP_SPEED = 0.25f;
        private const float DW_SPEED = 0.50f;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly AnimFloat m_RatioForward = new AnimFloat(0f, DW_SPEED);
        private readonly AnimFloat m_RatioSides = new AnimFloat(0f, DW_SPEED);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] private RigLean Rig { get; }
        [field: NonSerialized] private Transform Bone { get; }
        
        [field: NonSerialized] private float SidesAngle { get; }
        [field: NonSerialized] private float ForwardPositive { get; }
        [field: NonSerialized] private float ForwardNegative { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public LeanSection(RigLean rig, HumanBodyBones bone, float sidesAngle, 
            float forwardNegative, float forwardPositive)
        {
            this.Rig = rig;
            this.Bone = rig.Character.Animim.Animator.GetBoneTransform(bone);

            this.SidesAngle = sidesAngle;
            this.ForwardNegative = forwardNegative;
            this.ForwardPositive = forwardPositive;
            
            rig.Character.EventAfterChangeModel += this.RegisterLateUpdate;
            this.RegisterLateUpdate();
        }

        private void RegisterLateUpdate()
        {
            this.Rig.Character.EventAfterLateUpdate -= this.OnLateUpdate;
            this.Rig.Character.EventAfterLateUpdate += this.OnLateUpdate;
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        private void OnLateUpdate()
        {
            Animator animator = this.Rig.Animator;
            if (animator == null) return;
            
            Quaternion rotationForward = this.m_RatioForward.Current >= 0f
                ? Quaternion.Slerp(
                    Quaternion.identity, 
                    Quaternion.Euler(this.ForwardPositive, 0f, 0f),
                    this.m_RatioForward.Current
                )
                : Quaternion.Slerp(
                    Quaternion.identity, 
                    Quaternion.Euler(this.ForwardNegative, 0f, 0f),
                    -this.m_RatioForward.Current
                );

            Quaternion rotationSides = Quaternion.SlerpUnclamped(
                Quaternion.identity, 
                Quaternion.Euler(0f, 0f, -this.SidesAngle),
                this.m_RatioSides.Current
            );
            
            if (this.Bone == null) return;
            this.Bone.localRotation *= rotationForward * rotationSides;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(float forward, float sides)
        {
            float deltaTime = this.Rig.Character.Time.DeltaTime;
            float scale = this.Rig.IsActive && !this.Rig.Character.Ragdoll.IsRagdoll ? 1f : 0f;

            this.m_RatioForward.UpdateWithDelta(
                forward * scale, 
                forward >= this.m_RatioForward.Current ? UP_SPEED : DW_SPEED, 
                deltaTime
            );
            
            this.m_RatioSides.UpdateWithDelta(
                sides * scale,
                DW_SPEED,
                deltaTime
            );
        }
    }
}