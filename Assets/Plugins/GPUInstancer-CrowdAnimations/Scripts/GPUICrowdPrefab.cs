#if GPU_INSTANCER
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdPrefab : GPUInstancerPrefab
    {
        [NonSerialized]
        public GPUICrowdRuntimeData runtimeData;
        [NonSerialized]
        public Animator animatorRef;

        [NonSerialized]
        public GPUIMecanimAnimator mecanimAnimator;
        [NonSerialized]
        public GPUICrowdAnimator crowdAnimator;

        [NonSerialized]
        public bool hasChildPrefabs;
        [NonSerialized]
        public List<GPUICrowdPrefab> childCrowdPrefabs;
        [NonSerialized]
        public GPUICrowdPrefab parentCrowdPrefab;

        public Transform[] exposedTransforms;

        private bool _hasParentPrefab;

        public override void SetupPrefabInstance(GPUInstancerRuntimeData runtimeData, bool forceNew = false)
        {
            this.runtimeData = (GPUICrowdRuntimeData)runtimeData;

            if (_hasParentPrefab)
            {
                GPUICrowdOptionalRendererHandler optionalRendererHandler = gameObject.GetComponent<GPUICrowdOptionalRendererHandler>();
                if (optionalRendererHandler == null)
                    gameObject.AddComponent<GPUICrowdOptionalRendererHandler>();
                else
                    optionalRendererHandler.SetupOptionalRenderer();
                return;
            }

            if (animatorRef == null)
                animatorRef = GetAnimator();

            GPUICrowdPrototype prototype = (GPUICrowdPrototype)prefabPrototype;

            if (prototype.animationData.useCrowdAnimator)
            {
                if (crowdAnimator == null)
                    crowdAnimator = new GPUICrowdAnimator();
                if (crowdAnimator.activeClipCount == 0)
                    crowdAnimator.StartAnimation(this.runtimeData, gpuInstancerID - 1, prototype.animationData.clipDataList[prototype.animationData.crowdAnimatorDefaultClip]);
                else
                    crowdAnimator.UpdateIndex(this.runtimeData, gpuInstancerID - 1);
            }
            else
            {
                mecanimAnimator = new GPUIMecanimAnimator();
                if (animatorRef != null)
                {
                    animatorRef.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    animatorRef.applyRootMotion = prototype.animationData.applyRootMotion;
                }
            }

            Transform transform = GetInstanceTransform(true);
            if (prototype.hasOptionalRenderers)
            {
                foreach (GPUISkinnedMeshData smd in prototype.animationData.skinnedMeshDataList)
                {
                    if (!smd.isOptional)
                        continue;
                    Transform child = transform.FindDeepChild(smd.transformName);
                    if (child != null)
                    {
                        GPUICrowdPrefab childCrowdPrefab = child.gameObject.GetComponent<GPUICrowdPrefab>();
                        if (childCrowdPrefab == null)
                        {
                            hasChildPrefabs = true;
                            childCrowdPrefab = child.gameObject.AddComponent<GPUICrowdPrefab>();
                            if (childCrowdPrefabs == null)
                                childCrowdPrefabs = new List<GPUICrowdPrefab>();
                            childCrowdPrefabs.Add(childCrowdPrefab);
                        }
                        childCrowdPrefab.gpuInstancerID = gpuInstancerID;
                        childCrowdPrefab.prefabPrototype = smd.optionalPrototype;
                        childCrowdPrefab.parentCrowdPrefab = this;
                        childCrowdPrefab._hasParentPrefab = true;
                        if (childCrowdPrefab.runtimeData != null)
                            childCrowdPrefab.SetupPrefabInstance(childCrowdPrefab.runtimeData);
                    }
                }
            }

            if (prototype.animationData.useCrowdAnimator && prototype.animationData.applyBoneUpdates)
            {
                int boneFilterCount = prototype.animationData.exposedBoneIndexes.Length;
                for (int i = 0; i < boneFilterCount; i++)
                {
                    Transform boneTransform = exposedTransforms[i];
                    if (boneTransform == null)
                    {
                        Debug.LogError("Can not find bone transform with name: " + prototype.animationData.bones[prototype.animationData.exposedBoneIndexes[i]].boneTransformName, gameObject);
                    }
                    else
                    {
                        this.runtimeData.boneTransformAccessArray[(gpuInstancerID - 1) * prototype.animationData.exposedBoneIndexes.Length + i] = boneTransform;
                        boneTransform.SetParent(transform);
                    }
                }
            }
        }

        public Animator GetAnimator()
        {
            return GetComponent<Animator>();
        }

        public override Transform GetInstanceTransform(bool forceNew = false)
        {
            if (parentCrowdPrefab != null)
                return parentCrowdPrefab.GetInstanceTransform(forceNew);
            if (!_isTransformSet || forceNew)
            {
                _instanceTransform = transform;
                _instanceTransform.hasChanged = false;
                _isTransformSet = true;
            }
            return _instanceTransform;
        }

        #region Crowd Animator Workflow
        public void StartAnimation(AnimationClip animationClip, float startTime = -1.0f, float speed = 1.0f, float transitionTime = 0)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.StartAnimation(runtimeData, gpuInstancerID - 1, animationClip, startTime, speed, transitionTime);
        }

        public void StartAnimation(GPUIAnimationClipData clipData, float startTime = -1.0f, float speed = 1.0f, float transitionTime = 0)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.StartAnimation(runtimeData, gpuInstancerID - 1, clipData, startTime, speed, transitionTime);
        }

        public void StartBlend(Vector4 animationWeights,
            AnimationClip animationClip1, AnimationClip animationClip2, AnimationClip animationClip3 = null, AnimationClip animationClip4 = null,
            float[] animationTimes = null, float[] animationSpeeds = null, float transitionTime = 0)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.StartBlend(runtimeData, gpuInstancerID - 1, animationWeights, animationClip1, animationClip2, animationClip3, animationClip4, animationTimes, animationSpeeds, transitionTime);
        }

        public void SetAnimationWeights(Vector4 animationWeights)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.SetAnimationWeights(runtimeData, gpuInstancerID - 1, animationWeights);
        }

        public void SetAnimationSpeed(float animationSpeed)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.SetAnimationSpeed(runtimeData, gpuInstancerID - 1, animationSpeed);
        }

        public void SetAnimationSpeeds(float[] animationSpeeds)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.SetAnimationSpeeds(runtimeData, gpuInstancerID - 1, animationSpeeds);
        }

        public float GetAnimationTime(AnimationClip animationClip)
        {
            return crowdAnimator.GetClipTime(runtimeData, animationClip);
        }

        public void SetAnimationTime(AnimationClip animationClip, float time)
        {
            if (_hasParentPrefab)
            {
                Debug.LogError("Can not change animations on a child prototype.");
                return;
            }
            crowdAnimator.SetClipTime(runtimeData, gpuInstancerID - 1, animationClip, time);
        }
        #endregion Crowd Animator Workflow
    }
}
#endif //GPU_INSTANCER