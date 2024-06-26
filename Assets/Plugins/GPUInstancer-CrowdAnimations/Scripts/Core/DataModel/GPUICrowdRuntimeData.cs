#if GPU_INSTANCER
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdRuntimeData : GPUInstancerRuntimeData
    {
        public ComputeBuffer animationDataBuffer;
        public ComputeBuffer animationBakeBuffer;
        public RenderTexture animationBakeTexture;
        /// <summary>
        /// <para>index: 0 x -> frameNo1, y -> frameNo2, z -> frameNo3, w -> frameNo4</para> 
        /// <para>index: 1 x -> weight1, y -> weight2, z -> weight3, w -> weight4</para> 
        /// </summary>
        public NativeArray<Vector4> animationData;
        public Dictionary<int, GPUIAnimationClipData> animationClipDataDict;
        public Dictionary<int, GPUIAnimatorState> animatorStateDict;

        /// <summary>
        /// 0 to 4: x ->  minFrame, y -> maxFrame (negative if not looping), z -> speed, w -> startTime
        /// </summary>
        public NativeArray<Vector4> crowdAnimatorControllerData;
        public ComputeBuffer crowdAnimatorControllerBuffer;

        public bool disableFrameLerp;
        public List<GPUICrowdAnimator> transitioningAnimators;

        public bool isUVsSet;

        public bool hasEvents;
        public Dictionary<int, List<GPUIAnimationEvent>> eventDict;

        public TransformAccessArray boneTransformAccessArray;
        public NativeArray<Matrix4x4> bindPoses;
        public bool waitingBoneDataRequest;
        public JobHandle boneTransformUpdateJobHandle;
        public ComputeBuffer asyncBoneUpdateFilterBuffer;
        public ComputeBuffer asyncBoneUpdateDataBuffer;
        public NativeArray<int> boneUpdateFilter;
        private System.Action<AsyncGPUReadbackRequest> _boneDataRequestCallback;
        private AsyncGPUReadbackRequest _boneDataRequest;
        private int _boneDataRequestFrame;
        public NativeArray<Matrix4x4> bakedAnimationData;
        private bool _isSynchronousBoneUpdate;

        public NativeArray<GPUIAnimationClipData> clipDatas;
        public NativeArray<GPUIRootMotion> rootMotions;

        public bool animationDataModified;
        public bool crowdAnimatorDataModified;

        public bool isChildRuntimeData;
        public GPUICrowdRuntimeData parentRuntimeData;

        public GPUICrowdRuntimeData(GPUInstancerPrototype prototype) : base(prototype)
        {
            GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)prototype;
            disableFrameLerp = true;
            transitioningAnimators = new List<GPUICrowdAnimator>();
            isUVsSet = false;
            _boneDataRequestCallback = BoneDataRequestCompeleted;
            if (crowdPrototype.animationData != null)
                _isSynchronousBoneUpdate = crowdPrototype.animationData.isSynchronousBoneUpdates;
        }

        public override void InitializeData()
        {
            base.InitializeData();
            GPUICrowdUtility.SetAnimationData(this);
            GPUICrowdUtility.SetAppendBuffers(this);
            GPUICrowdUtility.SetMeshUVs(this);
        }

        public override void ReleaseBuffers()
        {
            base.ReleaseBuffers();
            if (animationDataBuffer != null)
                animationDataBuffer.Release();
            animationDataBuffer = null;
            if (animationBakeBuffer != null)
                animationBakeBuffer.Release();
            animationBakeBuffer = null;
            if (crowdAnimatorControllerBuffer != null)
                crowdAnimatorControllerBuffer.Release();
            crowdAnimatorControllerBuffer = null;
            if (boneTransformAccessArray.isCreated)
                boneTransformAccessArray.Dispose();
            if (bindPoses.IsCreated)
                bindPoses.Dispose();
            if (animationData.IsCreated)
                animationData.Dispose();
            if (crowdAnimatorControllerData.IsCreated)
                crowdAnimatorControllerData.Dispose();
            if (clipDatas.IsCreated)
                clipDatas.Dispose();
            if (rootMotions.IsCreated)
                rootMotions.Dispose();
            if (asyncBoneUpdateFilterBuffer != null)
                asyncBoneUpdateFilterBuffer.Release();
            if (asyncBoneUpdateDataBuffer != null)
                asyncBoneUpdateDataBuffer.Release();
            if (boneUpdateFilter.IsCreated)
                boneUpdateFilter.Dispose();
            if (bakedAnimationData.IsCreated)
                bakedAnimationData.Dispose();
        }

        public override bool GenerateLODsFromLODGroup(GPUInstancerPrototype prototype)
        {
            GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)prototype;

            if (!prototype.prefabObject)
            {
                Debug.LogError("Can't create renderer(s): reference GameObject is null");
                return false;
            }

            if (crowdPrototype.IsBakeRequired())
            {
                Debug.LogError(prototype.prefabObject.name + " requires to Bake Animations.");
                return false;
            }

            LODGroup lodGroup = prototype.prefabObject.GetComponent<LODGroup>();

            if (instanceLODs == null)
                instanceLODs = new List<GPUInstancerPrototypeLOD>();
            else
                instanceLODs.Clear();

            for (int lod = 0; lod < lodGroup.GetLODs().Length; lod++)
            {
                List<SkinnedMeshRenderer> lodRenderers = new List<SkinnedMeshRenderer>();
                if (lodGroup.GetLODs()[lod].renderers != null)
                {
                    foreach (Renderer renderer in lodGroup.GetLODs()[lod].renderers)
                    {
                        if (renderer != null && renderer is SkinnedMeshRenderer)
                        {
                            lodRenderers.Add((SkinnedMeshRenderer)renderer);
                        }
                    }
                }

                if (lodRenderers.Count == 0)
                {
                    Debug.LogWarning("LODGroup has no mesh renderers. Prefab: " + lodGroup.gameObject.name + " LODIndex: " + lod);
                    continue;
                }

                AddLod(lodGroup.GetLODs()[lod].screenRelativeTransitionHeight);

                for (int r = 0; r < lodRenderers.Count; r++)
                {
                    List<Material> instanceMaterials = new List<Material>();
                    for (int m = 0; m < lodRenderers[r].sharedMaterials.Length; m++)
                    {
                        instanceMaterials.Add(GPUInstancerConstants.gpuiSettings.shaderBindings.GetInstancedMaterial(lodRenderers[r].sharedMaterials[m], GPUICrowdConstants.GPUI_EXTENSION_CODE));
                        if (prototype.isLODCrossFade)
                            instanceMaterials[m].EnableKeyword("LOD_FADE_CROSSFADE");
                    }

                    Matrix4x4 transformOffset = Matrix4x4.identity;
                    //Transform currentTransform = lodRenderers[r].gameObject.transform;
                    //while (currentTransform != lodGroup.gameObject.transform)
                    //{
                    //    transformOffset = Matrix4x4.TRS(currentTransform.localPosition, currentTransform.localRotation, currentTransform.localScale) * transformOffset;
                    //    currentTransform = currentTransform.parent;
                    //}

                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    lodRenderers[r].GetPropertyBlock(mpb);
                    MaterialPropertyBlock shadowMPB = null;
                    if (prototype.isShadowCasting)
                    {
                        shadowMPB = new MaterialPropertyBlock();
                        lodRenderers[r].GetPropertyBlock(shadowMPB);
                    }
                    AddRenderer(lod, lodRenderers[r].sharedMesh, instanceMaterials, transformOffset, mpb,
                        lodRenderers[r].shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off, lodRenderers[r].gameObject.layer, shadowMPB, lodRenderers[r]);
                }
            }
            return true;
        }

        public override bool CreateRenderersFromMeshRenderers(int lod, GPUInstancerPrototype prototype)
        {
            GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)prototype;

            if (instanceLODs == null || instanceLODs.Count <= lod || instanceLODs[lod] == null)
            {
                Debug.LogError("Can't create renderer(s): Invalid LOD");
                return false;
            }

            if (!prototype.prefabObject)
            {
                Debug.LogError("Can't create renderer(s): reference GameObject is null");
                return false;
            }

            if (crowdPrototype.IsBakeRequired())
            {
                Debug.LogError(prototype.prefabObject.name + " requires to Bake Animations.");
                return false;
            }

            List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
            GetSkinnedMeshRenderersOfTransform(prototype.prefabObject.transform, skinnedMeshRenderers);

            if (skinnedMeshRenderers == null || skinnedMeshRenderers.Count == 0)
            {
                Debug.LogError("Can't create renderer(s): no SkinnedMeshRenderers found in the reference GameObject <" + prototype.prefabObject.name +
                        "> or any of its children");
                return false;
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (skinnedMeshRenderer.sharedMesh == null)
                {
                    Debug.LogWarning("SkinnedMeshRenderer with no Mesh found on GameObject <" + prototype.prefabObject.name +
                        "> (Child: <" + skinnedMeshRenderer.gameObject + ">). Are you missing a mesh reference?");
                    continue;
                }
                if (skinnedMeshRenderer.gameObject == prototype.prefabObject && !crowdPrototype.isChildPrototype)
                {
                    Debug.LogWarning(prototype.prefabObject.name + " prefab has SkinnedMeshRenderer at the parent GameObject which can cause issues because of renaming. Please add the SkinnedMeshRenderer components as a child GameObject and re-bake animations.", prototype.prefabObject);
                }

                if (instanceLODs[lod].renderers == null)
                {
                    instanceLODs[lod].renderers = new List<GPUInstancerRenderer>();
                }
                if (crowdPrototype.hasOptionalRenderers && crowdPrototype.animationData.GetSkinnedMeshDataByName(skinnedMeshRenderer.gameObject.name).isOptional)
                {
                    continue;
                }

                List<Material> instanceMaterials = new List<Material>();

                for (int m = 0; m < skinnedMeshRenderer.sharedMaterials.Length; m++)
                {
                    instanceMaterials.Add(GPUInstancerConstants.gpuiSettings.shaderBindings.GetInstancedMaterial(skinnedMeshRenderer.sharedMaterials[m], GPUICrowdConstants.GPUI_EXTENSION_CODE));
                }

                Matrix4x4 transformOffset = Matrix4x4.identity;
                //Transform currentTransform = skinnedMeshRenderer.gameObject.transform;
                //while (currentTransform != prototype.prefabObject.transform)
                //{
                //    transformOffset = Matrix4x4.TRS(currentTransform.localPosition, currentTransform.localRotation, currentTransform.localScale) * transformOffset;
                //    currentTransform = currentTransform.parent;
                //}

                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                skinnedMeshRenderer.GetPropertyBlock(mpb);
                MaterialPropertyBlock shadowMPB = null;
                if (prototype.isShadowCasting)
                {
                    shadowMPB = new MaterialPropertyBlock();
                    skinnedMeshRenderer.GetPropertyBlock(shadowMPB);
                }
                AddRenderer(lod, skinnedMeshRenderer.sharedMesh, instanceMaterials, transformOffset, mpb,
                    skinnedMeshRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off, skinnedMeshRenderer.gameObject.layer, shadowMPB,
                    skinnedMeshRenderer);
            }

            return true;
        }

        public override void CalculateBounds()
        {
            if (instanceLODs == null || instanceLODs.Count == 0 || instanceLODs[0].renderers == null ||
                instanceLODs[0].renderers.Count == 0)
                return;

            Bounds rendererBounds;
            GameObject tmpGO = new GameObject("TempGO");
            BoxCollider tmpCollider = tmpGO.AddComponent<BoxCollider>();
            for (int lod = 0; lod < instanceLODs.Count; lod++)
            {
                if (instanceLODs[lod].excludeBounds)
                    continue;

                for (int r = 0; r < instanceLODs[lod].renderers.Count; r++)
                {
                    SkinnedMeshRenderer smr = (SkinnedMeshRenderer)instanceLODs[lod].renderers[r].rendererRef;
                    if (smr.rootBone != null)
                    {
                        tmpGO.transform.position = smr.rootBone.position - prototype.prefabObject.transform.position;
                        tmpGO.transform.rotation = smr.rootBone.rotation * Quaternion.Inverse(prototype.prefabObject.transform.rotation);
                        tmpGO.transform.localScale = smr.rootBone.lossyScale;

                        tmpCollider.center = smr.localBounds.center;
                        tmpCollider.size = smr.localBounds.size;

                        rendererBounds = tmpCollider.bounds;
                    }
                    else
                        rendererBounds = smr.bounds;

                    if (lod == 0 && r == 0)
                    {
                        instanceBounds = rendererBounds;
                        continue;
                    }
                    instanceBounds.Encapsulate(rendererBounds);
                }
            }
            GameObject.Destroy(tmpGO);
            instanceBounds.size += prototype.boundsOffset;
        }

        public void GetSkinnedMeshRenderersOfTransform(Transform objectTransform, List<SkinnedMeshRenderer> skinnedMeshRenderers)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = objectTransform.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
                skinnedMeshRenderers.Add(skinnedMeshRenderer);

            Transform childTransform;
            for (int i = 0; i < objectTransform.childCount; i++)
            {
                childTransform = objectTransform.GetChild(i);
                if (childTransform.GetComponent<GPUICrowdPrefab>() != null)
                    continue;
                GetSkinnedMeshRenderersOfTransform(childTransform, skinnedMeshRenderers);
            }
        }

        public void MakeBoneDataRequest()
        {
            if (_isSynchronousBoneUpdate)
            {
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample("GPUI Crowd Animator Sync Bone Data Update");
#endif
                dependentJob.Complete();
                dependentJob = new ApplyBoneUpdatesJob()
                {
                    bindPoses = bindPoses,
                    boneUpdateFilter = boneUpdateFilter,
                    animationData = animationData,
                    crowdAnimatorControllerData = crowdAnimatorControllerData,
                    bakedAnimationData = bakedAnimationData,
                    currentTime = Time.time,
                    frameRate = ((GPUICrowdPrototype)prototype).frameRate,
                    instanceCount = instanceCount
                }.Schedule(boneTransformAccessArray);

                dependentJob.Complete();
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
            else if (!waitingBoneDataRequest)
            {
                _boneDataRequest = AsyncGPUReadback.Request(asyncBoneUpdateDataBuffer, _boneDataRequestCallback);
                waitingBoneDataRequest = true;
                _boneDataRequestFrame = Time.frameCount;
            }
        }

        public bool CompleteAsyncBoneDataRequest(bool forceComplete, int asyncBoneUpdateMaxLatency)
        {
            if (!_isSynchronousBoneUpdate && waitingBoneDataRequest)
            {
                if (forceComplete || asyncBoneUpdateMaxLatency == 0 || (Time.frameCount - _boneDataRequestFrame >= asyncBoneUpdateMaxLatency))
                {
#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.BeginSample("GPUI Crowd Animator Async Bone Data Request WaitForCompletion");
#endif
                    _boneDataRequest.WaitForCompletion();
#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                    return true;
                }
                return false;
            }
            return true;
        }

        public void BoneDataRequestCompeleted(AsyncGPUReadbackRequest obj)
        {
            if (obj.hasError)
            {
                Debug.LogError("Bone data request has encountered an error. Can not update bone transforms for prototype: " + prototype, prototype);
                return;
            }
            NativeArray<Matrix4x4> boneTransformArray = obj.GetData<Matrix4x4>();
            waitingBoneDataRequest = false;
            if (!boneTransformAccessArray.isCreated)
                return;

            // Having a transform access array with higher length value is normal when adding new instances where buffer size will increase
            //if (boneTransformAccessArray.length > boneTransformArray.Length)
            //{
            //    Debug.LogWarning("Bone data request result does not match the TransformAccessArray size. NativeArray Size: " + boneTransformArray.Length + " TransformAccessArray Size: " + boneTransformAccessArray.length);
            //    return;
            //}

            JobHandle boneTransformUpdateJobHandle = new ApplyBufferDataToTransformsJob()
            {
                boneTransformArray = boneTransformArray,
                bindPoses = bindPoses,
                boneUpdateFilter = boneUpdateFilter,
                instanceCount = instanceCount,
            }.Schedule(boneTransformAccessArray);
            MakeBoneDataRequest();
            boneTransformUpdateJobHandle.Complete();
        }
    }

#if GPUI_BURST
    [Unity.Burst.BurstCompile]
#endif
    struct ApplyBufferDataToTransformsJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Matrix4x4> boneTransformArray;
        [ReadOnly] public NativeArray<Matrix4x4> bindPoses;
        [ReadOnly] public NativeArray<int> boneUpdateFilter;
        [ReadOnly] public int instanceCount;

        public void Execute(int index, TransformAccess transform)
        {
            if (index >= boneTransformArray.Length)
                return;
            int numBones = boneUpdateFilter.Length;
            int instanceIndex = index / numBones;
            if (instanceIndex >= instanceCount)
                return;
            int boneIndex = boneUpdateFilter[index % numBones];
            Matrix4x4 localToWorld = boneTransformArray[index] * bindPoses[boneIndex].inverse;
            if (localToWorld.ValidTRS())
            {
                transform.localPosition = localToWorld.GetColumn(3);
                transform.localRotation = localToWorld.rotation;
                transform.localScale = localToWorld.lossyScale;
            }
        }
    }

#if GPUI_BURST
    [Unity.Burst.BurstCompile]
#endif
    struct ApplyBoneUpdatesJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Matrix4x4> bindPoses;
        [ReadOnly] public NativeArray<int> boneUpdateFilter;
        /// <summary>
        /// <para>index: 0 x -> frameNo1, y -> frameNo2, z -> frameNo3, w -> frameNo4</para> 
        /// <para>index: 1 x -> weight1, y -> weight2, z -> weight3, w -> weight4</para> 
        /// </summary>
        [ReadOnly] public NativeArray<Vector4> animationData;
        /// <summary>
        /// 0 to 4: x ->  minFrame, y -> maxFrame (negative if not looping), z -> speed, w -> startTime
        /// </summary>
        [ReadOnly] public NativeArray<Vector4> crowdAnimatorControllerData;
        [ReadOnly] public NativeArray<Matrix4x4> bakedAnimationData;

        [ReadOnly] public float currentTime;
        [ReadOnly] public int frameRate;
        [ReadOnly] public int instanceCount;

        public void Execute(int index, TransformAccess transform)
        {
            int numBones = boneUpdateFilter.Length;
            int boneFilterIndex = index % numBones;
            int boneIndex = boneUpdateFilter[boneFilterIndex];
            int instanceIndex = index / numBones;
            if (instanceIndex >= instanceCount)
                return;
            int animationDataIndex = instanceIndex * 2;
            int crowdAnimatorIndex = instanceIndex * 4;

            Vector4 weightData = animationData[animationDataIndex + 1];
            Vector4 crowdAnimator = crowdAnimatorControllerData[crowdAnimatorIndex];

            float clipFrame = (currentTime - crowdAnimator.w) * crowdAnimator.z * frameRate;
            float clipTotalFrame = Mathf.Abs(crowdAnimator.y) - crowdAnimator.x;
            float currentFrame = (clipFrame % clipTotalFrame) + crowdAnimator.x;
            if (crowdAnimator.y < 0 && clipFrame > clipTotalFrame)
                currentFrame = Mathf.Abs(crowdAnimator.y);

            int bakedDataIndex = Mathf.FloorToInt(currentFrame) * numBones + boneFilterIndex;
            Matrix4x4 bindPoseInverse = bindPoses[boneIndex].inverse;
            Matrix4x4 localToWorld = bakedAnimationData[bakedDataIndex] * bindPoseInverse;

            if (localToWorld.ValidTRS())
            {
                Vector3 position = localToWorld.GetColumn(3);
                Quaternion rotation = localToWorld.rotation;
                Vector3 scale = localToWorld.lossyScale;

                float progress = currentFrame - Mathf.Floor(currentFrame);
                if (progress > 0f)
                {
                    int nextBakedDataIndex = Mathf.CeilToInt(currentFrame) * numBones + boneFilterIndex;
                    Matrix4x4 nextLocalToWorld = bakedAnimationData[nextBakedDataIndex] * bindPoseInverse;
                    if (nextLocalToWorld.ValidTRS())
                    {
                        position = Vector3.Lerp(position, nextLocalToWorld.GetColumn(3), progress);
                        rotation = Quaternion.Lerp(rotation, nextLocalToWorld.rotation, progress);
                        scale = Vector3.Lerp(scale, nextLocalToWorld.lossyScale, progress);
                    }
                }

                float weight = weightData.x;
                if (weight < 1f) // blending
                {
                    position *= weight;
                    scale *= weight;

                    for (int w = 1; w < 4; w++)
                    {
                        weight = weightData[w];
                        if (weight > 0)
                        {
                            crowdAnimator = crowdAnimatorControllerData[crowdAnimatorIndex + w];
                            clipFrame = (currentTime - crowdAnimator.w) * crowdAnimator.z * frameRate;
                            clipTotalFrame = Mathf.Abs(crowdAnimator.y) - crowdAnimator.x;
                            currentFrame = (clipFrame % clipTotalFrame) + crowdAnimator.x;
                            if (crowdAnimator.y < 0 && clipFrame > clipTotalFrame)
                                currentFrame = Mathf.Abs(crowdAnimator.y);
                            bakedDataIndex = Mathf.FloorToInt(currentFrame) * numBones + boneFilterIndex;

                            localToWorld = bakedAnimationData[bakedDataIndex] * bindPoseInverse;
                            if (localToWorld.ValidTRS())
                            {
                                Vector3 newPosition = localToWorld.GetColumn(3);
                                Quaternion newRotation = localToWorld.rotation;
                                Vector3 newScale = localToWorld.lossyScale;

                                progress = currentFrame - Mathf.Floor(currentFrame);
                                if (progress > 0f)
                                {
                                    int nextBakedDataIndex = Mathf.CeilToInt(currentFrame) * numBones + boneFilterIndex;
                                    Matrix4x4 nextLocalToWorld = bakedAnimationData[nextBakedDataIndex] * bindPoseInverse;
                                    if (nextLocalToWorld.ValidTRS())
                                    {
                                        newPosition = Vector3.Lerp(newPosition, nextLocalToWorld.GetColumn(3), progress);
                                        newRotation = Quaternion.Lerp(newRotation, nextLocalToWorld.rotation, progress);
                                        newScale = Vector3.Lerp(newScale, nextLocalToWorld.lossyScale, progress);
                                    }
                                }

                                position += newPosition * weight;
                                rotation = Quaternion.Lerp(rotation, newRotation, weight);
                                scale += newScale * weight;
                            }
                        }
                    }
                }

                transform.localPosition = position;
                transform.localRotation = rotation;
                transform.localScale = scale;
            }
        }
    }

    public class GPUICrowdTransition
    {
        public int arrayIndex;
        public float startTime;
        public float totalTime;
        public int transitioningClipCount;
        public Vector4 startWeights;
        public Vector4 endWeights;
        public int endActiveClipCount;

        public void SetData(int arrayIndex, float startTime, float totalTime, int transitioningClipCount,
            Vector4 startWeights, Vector4 endWeights, int endActiveClipCount)
        {
            this.arrayIndex = arrayIndex;
            this.startTime = startTime;
            this.totalTime = totalTime;
            this.transitioningClipCount = transitioningClipCount;
            this.startWeights = startWeights;
            this.endWeights = endWeights;
            this.endActiveClipCount = endActiveClipCount;
        }
    }

    [System.Serializable]
    public class GPUIAnimationEvent : UnityEvent<GPUICrowdPrefab, float, int, string>
    {
        public GPUICrowdPrototype prototype;
        public AnimationClip animationClip;
        public int eventFrame;
        public float floatParam;
        public int intParam;
        public string stringParam;

        public GPUIAnimationEvent()
        {
        }

        public GPUIAnimationEvent(GPUICrowdPrototype prototype, AnimationClip animationClip)
        {
            this.prototype = prototype;
            this.animationClip = animationClip;
        }
    }
}
#endif //GPU_INSTANCER