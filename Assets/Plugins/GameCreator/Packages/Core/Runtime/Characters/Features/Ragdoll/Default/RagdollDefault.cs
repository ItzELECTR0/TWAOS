using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Default Ragdoll")]
    [Category("Default Ragdoll")]
    
    [Image(typeof(IconSkeleton), ColorTheme.Type.Yellow)]
    [Description("Default Ragdoll System")]
    
    [Serializable]
    public class RagdollDefault : TRagdollSystem
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private BoneRack m_BoneRack = new BoneRack();
        [SerializeField] private float m_TransitionDuration = 0.2f;
        
        [SerializeField] private AnimationClip m_RecoverFaceDown;
        [SerializeField] private AnimationClip m_RecoverFaceUp;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private int m_CurrentModelID = -1;
        
        [NonSerialized] private GameObject[] m_Bones = Array.Empty<GameObject>();
        
        [NonSerialized] private BoneSnapshot m_RootSnapshot;
        [NonSerialized] private BoneSnapshot[] m_BonesSnapshots;
        
        [NonSerialized] private bool m_IsRecovering;
        [NonSerialized] private float m_RecoverStartTime;
        
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        protected internal override void OnStartup(Character character)
        {
            if (this.m_BoneRack == null) return;
            this.m_BoneRack.EventChangeSkeleton += this.OnChangeSkeleton;
        }

        protected internal override void OnDispose(Character character)
        {
            if (this.m_BoneRack == null) return;
            this.m_BoneRack.EventChangeSkeleton -= this.OnChangeSkeleton;
        }

        protected internal override void OnEnable(Character character)
        { }

        protected internal override void OnDisable(Character character)
        { }
        
        // UPDATE METHODS: ------------------------------------------------------------------------
        
        protected internal override void OnUpdate(Character character)
        { }

        protected internal override void OnLateUpdate(Character character)
        {
            if (!character.Ragdoll.IsRagdoll) return;
            
            Animator animator = character.Animim.Animator;
            if (animator == null) return;

            if (!this.m_IsRecovering) return;
            this.UpdateRagdollRecover(character, animator.transform);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        protected internal override Task StartRagdoll(Character character)
        {
            this.m_IsRecovering = false;
            this.RequireInitialize(character, false);
            
            Vector3 direction = character.Driver.WorldMoveDirection;
            
            character.Gestures.Stop(0f, 0.1f);
            Animator animator = character.Animim.Animator;
            
            animator.transform.SetParent(null);

            foreach (GameObject bone in this.m_Bones)
            {
                bone.Get<Collider>().enabled = true;
                bone.Get<Rigidbody>().isKinematic = false;
                bone.Get<Rigidbody>().linearVelocity = direction;
            }
            
            animator.enabled = false;
            return Task.CompletedTask;
        }

        protected internal override Task StopRagdoll(Character character)
        {
            Animator animator = character.Animim.Animator;
            this.RequireInitialize(character, false);

            foreach (GameObject bone in this.m_Bones)
            {
                bone.Get<Collider>().enabled = false;
                bone.Get<Rigidbody>().linearVelocity = Vector3.zero;
                bone.Get<Rigidbody>().isKinematic = true;
            }

            Transform model = animator.transform;
            if (animator.isHuman)
            {
                Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
                Transform head = animator.GetBoneTransform(HumanBodyBones.Head);

                Ray ray = new Ray(
                    hips.position,
                    Vector3.down * (character.Motion.Height * 0.5f)
                );

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 pointDifference = model.position - hit.point;
                    model.position -= pointDifference;
                    hips.position += pointDifference;
                }
                else
                {
                    Vector3 hipsDifference = model.position - hips.position;
                    model.position -= hipsDifference;
                    hips.position += hipsDifference;
                }

                Vector3 skeletonForward = Vector3.ProjectOnPlane(
                    head.position - hips.position,
                    Vector3.up
                );

                Quaternion hipsRotation = hips.rotation;
                Vector3 hipsForward = hips.TransformDirection(Vector3.forward);

                model.rotation = Quaternion.LookRotation(skeletonForward, Vector3.up) *
                                 Quaternion.Euler(0f, hipsForward.y > 0f ? 180f : 0f, 0f);

                hips.rotation = hipsRotation;
            }

            character.Driver.SetPosition(model.position);
            character.Driver.SetRotation(model.rotation);

            Transform parent = character.Animim.Mannequin != null && character.Animim.Mannequin != model
                ? character.Animim.Mannequin
                : character.transform;
            
            model.SetParent(parent, true);
            animator.enabled = true;
            
            return Task.CompletedTask;
        }

        protected internal override async Task RecoverRagdoll(Character character)
        {
            Animator animator = character.Animim.Animator;
            AnimationClip standClip;
            
            if (animator.isHuman)
            {
                Vector3 hipsForward = animator
                    .GetBoneTransform(HumanBodyBones.Hips)
                    .TransformDirection(Vector3.forward);

                standClip = hipsForward.y > 0f
                    ? this.m_RecoverFaceUp
                    : this.m_RecoverFaceDown;
            }
            else
            {
                standClip = animator.transform.TransformDirection(Vector3.forward).y > 0f
                    ? this.m_RecoverFaceUp
                    : this.m_RecoverFaceDown;
            }
            
            if (standClip != null)
            {
                const BlendMode mode = BlendMode.Blend;
                ConfigGesture config = new ConfigGesture(
                    0f, standClip.length, 1f, true,
                    0f, character.Animim.SmoothTime
                );

                this.m_IsRecovering = true;
                this.m_RecoverStartTime = character.Time.Time;
                this.RefreshSnapshots(character);

                await character.Gestures.CrossFade(standClip, null, mode, config, true);
            }
        }


        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void RequireInitialize(Character character, bool force)
        {
            if (character == null) return;
            if (character.Animim.Animator == null) return;

            Skeleton skeleton = this.m_BoneRack.Skeleton;
            if (skeleton == null) return;

            int modelID = character.Animim.Animator.gameObject.GetInstanceID();
            if (modelID == this.m_CurrentModelID && !force) return;

            this.m_Bones = skeleton.Refresh(character);
            this.m_CurrentModelID = modelID;
        }
        
        private void RefreshSnapshots(Character character)
        {
            Animator animator = character.Animim.Animator;
            if (animator == null) return;

            Transform[] children = animator.GetComponentsInChildren<Transform>();
            
            this.m_RootSnapshot = new BoneSnapshot(animator.transform);
            List<BoneSnapshot> candidates = new List<BoneSnapshot>();
            
            foreach (Transform child in children)
            {
                if (child == animator.transform) continue;
                candidates.Add(new BoneSnapshot(child));
            }
            
            this.m_BonesSnapshots = candidates.ToArray();
        }
        
        private void UpdateRagdollRecover(Character character, Transform mannequin)
        {
            float elapsed = character.Time.Time - this.m_RecoverStartTime;
            float t = Easing.QuadInOut(0f, 1f, elapsed / this.m_TransitionDuration);

            this.m_RootSnapshot.Value.localPosition = Vector3.Lerp(
                this.m_RootSnapshot.LocalPosition,
                this.m_RootSnapshot.Value.localPosition,
                t
            );
            
            this.m_RootSnapshot.Value.localRotation = Quaternion.Lerp(
                this.m_RootSnapshot.LocalRotation,
                this.m_RootSnapshot.Value.localRotation,
                t
            );
            
            foreach (BoneSnapshot boneSnapshot in this.m_BonesSnapshots)
            {
                if (boneSnapshot.Value == null) continue;
                
                if (boneSnapshot.Value.parent == mannequin)
                {
                    boneSnapshot.Value.position = Vector3.Lerp(
                        boneSnapshot.WorldPosition,
                        boneSnapshot.Value.position,
                        t
                    );
                }
        
                if (boneSnapshot.LocalRotation != boneSnapshot.Value.localRotation)
                {
                    boneSnapshot.Value.rotation = Quaternion.Lerp(
                        boneSnapshot.WorldRotation,
                        boneSnapshot.Value.rotation,
                        t
                    );
                }
            }
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        private void OnChangeSkeleton()
        {
            this.m_CurrentModelID = -1;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        protected internal override void OnDrawGizmos(Character character)
        {
            if (character.Animim.Animator == null) return;
            this.m_BoneRack.DrawGizmos(character.Animim.Animator);
        }
    }
}