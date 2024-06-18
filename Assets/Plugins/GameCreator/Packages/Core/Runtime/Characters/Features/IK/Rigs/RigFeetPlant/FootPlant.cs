using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    internal class FootPlant
    {
        private const int RAYCAST_FIXED_SIZE = 10;
        
        private const float COEFFICIENT_RANGE_FEET_UP = 0.5f;
        private const float COEFFICIENT_RANGE_FEET_DOWN = 0.2f;

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly RaycastHit[] m_RaycastHitsBuffer = new RaycastHit[RAYCAST_FIXED_SIZE];

        [NonSerialized] private readonly AnimFloat m_Weight = new AnimFloat(0f, 0f);

        [NonSerialized] private readonly AnimVector3 m_DeltaPosition = new AnimVector3(Vector3.zero, 0f);
        [NonSerialized] private readonly AnimQuaternion m_DeltaRotation = new AnimQuaternion(Quaternion.identity, 0f);
        
        [NonSerialized] private Transform m_BoneTransform;

        [NonSerialized] private bool m_HasHit;
        [NonSerialized] private Vector3 m_HitPoint;
        [NonSerialized] private Vector3 m_HitNormal;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] private HumanBodyBones Bone { get; }
        [field: NonSerialized] private AvatarIKGoal AvatarIK { get; }
        
        [field: NonSerialized] private RigFeetPlant Rig { get; }
        [field: NonSerialized] private int Phase { get; }

        private IUnitDriver Driver => this.Rig.Character.Driver;

        private Transform BoneTransform
        {
            get
            {
                if (this.m_BoneTransform == null)
                {
                    Animator animator = this.Rig.Animator;
                    this.m_BoneTransform = animator.GetBoneTransform(this.Bone);
                }

                return this.m_BoneTransform;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public FootPlant(HumanBodyBones bone, AvatarIKGoal avatarIK, RigFeetPlant rig, int phase)
        {
            this.Bone = bone;
            this.AvatarIK = avatarIK;
            this.Rig = rig;
            this.Phase = phase;

            this.Rig.Character.EventAfterChangeModel += this.RegisterAnimatorIK;
            this.RegisterAnimatorIK();
        }

        private void RegisterAnimatorIK()
        {
            this.Rig.Character.Animim.EventOnAnimatorIK -= this.OnAnimatorIK;
            this.Rig.Character.Animim.EventOnAnimatorIK += this.OnAnimatorIK;
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            this.OnAnimatorUpdateFoot();
            this.OnAnimatorSetFoot();
        }

        private void OnAnimatorUpdateFoot()
        {
            Animator animator = this.Rig.Animator;
            if (animator == null) return;
            
            float feetRangeUp = this.Rig.Character.Motion.Height * COEFFICIENT_RANGE_FEET_UP;
            float feetRangeDown = this.Rig.Character.Motion.Height * COEFFICIENT_RANGE_FEET_DOWN;
            
            Vector3 bonePosition = animator.GetIKPosition(this.AvatarIK);
            // Vector3 parentPosition = this.BoneTransform.parent.position;

            // Vector3 castDirection = (bonePosition - parentPosition).normalized;
            Vector3 castDirection = Vector3.down;
            Vector3 castPosition = bonePosition - castDirection * feetRangeUp;

            int hitCount = Physics.RaycastNonAlloc(
                castPosition,
                castDirection,
                this.m_RaycastHitsBuffer,
                feetRangeUp + feetRangeDown,
                this.Rig.FootMask,
                QueryTriggerInteraction.Ignore
            );
            
            float minDistance = Mathf.Infinity;
            RaycastHit minHit = new RaycastHit();
            
            for (int i = 0; i < hitCount; ++i)
            {
                RaycastHit hit = this.m_RaycastHitsBuffer[i];
                if (hit.distance > minDistance) continue;

                minHit = hit;
                minDistance = hit.distance;
            }

            if (hitCount > 0)
            {
                this.m_HasHit = true;
                this.m_HitPoint = minHit.point;
                this.m_HitNormal = minHit.normal;
            }
            else
            {
                this.m_HasHit = false;
                this.m_HitPoint = this.BoneTransform.position;
                this.m_HitNormal = Vector3.up;
            }
        }

        private void OnAnimatorSetFoot()
        {
            Animator animator = this.Rig.Animator;
            if (animator == null) return;

            if (this.m_HasHit)
            {
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, this.m_HitNormal);
                float angle = Vector3.Angle(Vector3.up, this.m_HitNormal);
                Quaternion targetRotation = Quaternion.AngleAxis(angle, rotationAxis);

                float offset = this.Rig.FootOffset + this.Driver.SkinWidth;
                Vector3 targetPosition = this.m_HitPoint + Vector3.up * offset;
                targetPosition -= animator.GetIKPosition(AvatarIK);

                this.m_DeltaPosition.Target = targetPosition;
                this.m_DeltaRotation.Target = targetRotation;

                this.m_Weight.Target = 1f;
            }
            else
            {
                this.m_DeltaPosition.Target = Vector3.zero;
                this.m_DeltaRotation.Target = Quaternion.identity;

                this.m_Weight.Target = 0f;
            }
            
            float weight = this.Rig.IsActive && this.Driver.IsGrounded
                ? this.Rig.Character.Phases.Get(this.Phase) * this.m_Weight.Current
                : 0f;

            Vector3 position = animator.GetIKPosition(AvatarIK);
            Quaternion rotation =  animator.GetIKRotation(AvatarIK);

            animator.SetIKPositionWeight(this.AvatarIK, weight);
            animator.SetIKPosition(this.AvatarIK, this.m_DeltaPosition.Current + position);
            
            animator.SetIKRotationWeight(this.AvatarIK, weight);
            animator.SetIKRotation(this.AvatarIK, this.m_DeltaRotation.Current * rotation);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update()
        {
            float deltaTime = this.Rig.Character.Time.DeltaTime;
            float smoothTime = this.Rig.SmoothTime;

            this.m_Weight.Smooth = smoothTime;
            this.m_Weight.UpdateWithDelta(deltaTime);
            
            this.m_DeltaPosition.Smooth = Vector3.one * smoothTime;
            this.m_DeltaRotation.Smooth = smoothTime;
            
            this.m_DeltaPosition.UpdateWithDelta(deltaTime);
            this.m_DeltaRotation.UpdateWithDelta(deltaTime);
        }
    }
}