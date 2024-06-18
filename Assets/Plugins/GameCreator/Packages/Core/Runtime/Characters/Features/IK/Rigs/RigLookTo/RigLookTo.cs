using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Title("Look at Targets")]
    [Category("Look at Targets")]
    [Image(typeof(IconEye), ColorTheme.Type.Green)]
    
    [Description(
        "IK system that allows the Character to naturally look at points of interest using the " +
        "whole upper-body chain of bones. Requires a humanoid character"
    )]
    
    [Serializable]
    public class RigLookTo : TRigAnimatorIK
    {
        private class LookTargets : List<ILookTo>
        {
            public ILookTo Get(Vector3 target)
            {
                float minDistance = Mathf.Infinity;
                ILookTo minLookTo = null;

                foreach (ILookTo lookTo in this)
                {
                    if (lookTo is not { Exists: true }) continue;

                    float distance = Vector3.Distance(target, lookTo.Position);
                    if (distance >= minDistance) continue;
                    
                    minLookTo = lookTo;
                    minDistance = distance;
                }

                return minLookTo;
            }
        }
        
        private class LookLayers : SortedDictionary<int, LookTargets>
        { }
        
        // CONSTANTS: -----------------------------------------------------------------------------

        public const string RIG_NAME = "RigLookTo";

        private const float SMOOTH_TIME = 0.15f;
        private const float HORIZON = 10f;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_TrackSpeed = 270f;
        [SerializeField] private float m_MaxAngle = 120f;

        [SerializeField] private LookSection[] m_Sections =
        {
            new LookSection(HumanBodyBones.Chest, 1f),
            new LookSection(HumanBodyBones.Neck, 3f),
            new LookSection(HumanBodyBones.Head, 3f),
        };

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private AnimFloat m_WeightTarget = new AnimFloat(0f, SMOOTH_TIME);

        [NonSerialized] private Transform m_LookHandle;
        [NonSerialized] private Transform m_LookPoint;

        [NonSerialized] private ILookTo m_LookToTarget;
        [NonSerialized] private readonly LookLayers m_Layers = new LookLayers();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => "Look at Target";
        public override string Name => RIG_NAME;

        public override bool RequiresHuman => true;
        public override bool DisableOnBusy => true;

        public ILookTo LookToTarget => this.m_LookToTarget;

        // PUBLIC METHODS: ------------------------------------------------------------------------

         public void SetTarget<T>(T look) where T : ILookTo
         {
             if (look == null) return;
             if (!this.m_Layers.ContainsKey(look.Layer))
             {
                 this.m_Layers[look.Layer] = new LookTargets();
             }

             if (this.m_Layers[look.Layer].Contains(look)) return;
             
             this.m_Layers[look.Layer].Add(look);
         }

         public void RemoveTarget<T>(T look) where T : ILookTo
         {
             if (look == null) return;
             if (this.m_Layers.TryGetValue(look.Layer, out LookTargets targets))
             {
                 targets.Remove(look);
             }
         }

         public void ClearTargets()
         {
             foreach (KeyValuePair<int, LookTargets> entry in this.m_Layers)
             {
                 if (entry.Key == 0) continue;
                 entry.Value.Clear();
             }
         }
        
        // IMPLEMENT METHODS: ---------------------------------------------------------------------

        protected override void DoEnable(Character character)
        {
            base.DoEnable(character);
            
            if (this.m_LookHandle == null || this.m_LookPoint == null)
            {
                if (this.m_LookHandle != null) UnityEngine.Object.Destroy(this.m_LookHandle.gameObject);
                if (this.m_LookPoint != null) UnityEngine.Object.Destroy(this.m_LookPoint.gameObject);
                
                GameObject handle = new GameObject(RIG_NAME + "Handle");
                GameObject point = new GameObject(RIG_NAME + "Point");
                
                handle.hideFlags = HideFlags.HideAndDontSave;
                point.hideFlags = HideFlags.HideAndDontSave;

                this.m_LookHandle = handle.transform;
                this.m_LookHandle.position = character.Eyes;

                this.m_LookPoint = point.transform;
                this.m_LookPoint.SetParent(this.m_LookHandle);
                this.m_LookPoint.localPosition = Vector3.forward * HORIZON;
            }

            foreach (LookSection section in this.m_Sections)
            {
                if (this.GetBone(section.Bone, out Transform bone))
                {
                    section.Transform = bone;
                }
            }

            this.Character.EventAfterLateUpdate -= this.OnLateUpdate;
            this.Character.EventAfterLateUpdate += this.OnLateUpdate;
        }

        protected override void DoDisable(Character character)
        {
            base.DoDisable(character);
            this.Character.EventAfterLateUpdate -= this.OnLateUpdate;
        }

        protected override void DoUpdate(Character character)
        {
            base.DoUpdate(character);
            
            this.m_LookToTarget = this.GetLookTrackTarget(character);
            
            Vector3 targetDirection;
            
            this.m_LookHandle.position = character.Eyes;
            
            if (this.m_LookToTarget is { Exists: true })
            {
                this.m_WeightTarget.Target = 1f;
                Vector3 targetPosition = this.m_LookToTarget.Position;
                
                Vector3 characterDirection = character.transform.forward;
                targetDirection = targetPosition - character.Eyes;
                
                float angle = Vector3.Angle(characterDirection, targetDirection);
                float distance = Vector2.Distance(
                    character.transform.position.XZ(),
                    targetPosition.XZ()
                );
                
                if (angle > this.m_MaxAngle || distance < character.Motion.Radius)
                {
                    this.m_WeightTarget.Target = 0f;
                    targetDirection = character.transform.forward;
                }
            }
            else
            {
                this.m_WeightTarget.Target = 0f;
                targetDirection = character.transform.forward;
            }

            this.m_WeightTarget.UpdateWithDelta(
                this.m_WeightTarget.Target,
                this.Character.Time.DeltaTime
            );
            
            this.m_LookHandle.rotation = Quaternion.RotateTowards(
                this.m_LookHandle.rotation,
                Quaternion.LookRotation(targetDirection, Vector3.up),
                character.Time.DeltaTime * this.m_TrackSpeed
            );
        }
        
        private void OnLateUpdate()
        {
            float totalWeight = 0f;
            
            foreach (LookSection section in this.m_Sections)
            {
                if (!section.IsValid) continue;
                
                section.Transform.localRotation *= section.Rotation;
                totalWeight = section.Weight;
            }
            
            foreach (LookSection section in this.m_Sections)
            {
                if (!section.IsValid) continue;
                float weightRatio = section.Weight / totalWeight;
            
                Quaternion rotation = Quaternion.LookRotation(
                    this.m_LookPoint.position - section.Transform.position,
                    Vector3.up
                );
                
                section.Transform.rotation = Quaternion.Lerp(
                    section.Transform.rotation,
                    rotation,
                    this.m_WeightTarget.Current * weightRatio
                );
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private bool GetBone(HumanBodyBones boneType, out Transform bone)
        {
            bone = this.Animator.GetBoneTransform(boneType);
            return bone != null;
        }
        
        private ILookTo GetLookTrackTarget(Character character)
        {
            foreach (KeyValuePair<int,LookTargets> entryLayer in this.m_Layers)
            {
                ILookTo target = entryLayer.Value.Get(character.Eyes);
                if (target is { Exists: true }) return target;
            }
            
            return null;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

         protected override void DoDrawGizmos(Character character)
         {
             base.DoDrawGizmos(character);
             Gizmos.color = Color.cyan;
             
             if (this.m_LookPoint == null) return;
             
             Gizmos.DrawWireCube(this.m_LookPoint.position, Vector3.one * 0.1f);
             Gizmos.DrawLine(this.Character.Eyes, this.m_LookPoint.position);
         }
    }
}