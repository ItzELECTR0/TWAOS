using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if GPU_INSTANCER
namespace GPUInstancer.CrowdAnimations
{
    [ExecuteInEditMode]
    public class GPUICrowdManager : GPUInstancerPrefabManager
    {
        protected List<AnimatorClipInfo> _animatorClipInfos = new List<AnimatorClipInfo>();

        private ComputeShader _skinnedMeshAnimateComputeShader;
        private int _animateBonesKernelID;
        private int _animateBonesLerpedKernelID;
        private int _fixWeightsKernelID;

        private ComputeShader _crowdAnimatorComputeShader;
        private int _crowdAnimatorKernelID;

        private ComputeShader _asyncBoneUpdateComputeShader;
        private int _asyncBoneUpdateKernelID;

        private ComputeShader _optionalRendererBufferCopyComputeShader;
        private int _optionalRendererBufferCopyKernelID;

        private ComputeShader _animationBufferToTextureComputeShader;
        private int _animationBufferToTextureKernelID;

        private float _lastRootMotionUpdateTime;
        //private static float _rootMotionFrequency = 1.0f / 90.0f;

        private float _lastTransitionUpdateTime;
        private static float _transitionFrequency = 1.0f / 60.0f;

        private float _lastAnimateTime;

        public List<GPUIAnimationEvent> animationEvents;

#if UNITY_EDITOR
        public int selectedClipIndex;
        public bool showEventsFoldout = true;
#endif

        #region MonoBehavior Methods

        public override void Awake()
        {
            if (GPUICrowdConstants.gpuiCrowdSettings == null)
                GPUICrowdConstants.gpuiCrowdSettings = GPUICrowdSettings.GetDefaultGPUICrowdSettings();

            base.Awake();

            if (!GPUInstancerConstants.gpuiSettings.shaderBindings.HasExtension(GPUICrowdConstants.GPUI_EXTENSION_CODE))
            {
                GPUInstancerConstants.gpuiSettings.shaderBindings.AddExtension(new GPUICrowdShaderBindings());
            }
        }

        public override void Start()
        {
            base.Start();

            _skinnedMeshAnimateComputeShader = (ComputeShader)Resources.Load(GPUICrowdConstants.COMPUTE_SKINNED_MESH_ANIMATE_PATH);
            _animateBonesKernelID = _skinnedMeshAnimateComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_ANIMATE_BONES_KERNEL);
            _animateBonesLerpedKernelID = _skinnedMeshAnimateComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_ANIMATE_BONES_LERPED_KERNEL);
            _fixWeightsKernelID = _skinnedMeshAnimateComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_FIX_WEIGHTS_KERNEL);

            _crowdAnimatorComputeShader = (ComputeShader)Resources.Load(GPUICrowdConstants.COMPUTE_CROWD_ANIMATOR_PATH);
            _crowdAnimatorKernelID = _crowdAnimatorComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_CROWD_ANIMATOR_KERNEL);

            _asyncBoneUpdateComputeShader = (ComputeShader)Resources.Load(GPUICrowdConstants.COMPUTE_ASYNC_BONE_UPDATE);
            _asyncBoneUpdateKernelID = _asyncBoneUpdateComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_ASYNC_BONE_UPDATE_KERNEL);

            _optionalRendererBufferCopyComputeShader = (ComputeShader)Resources.Load(GPUICrowdConstants.COMPUTE_OPTIONAL_RENDERER_BUFFER_COPY);
            _optionalRendererBufferCopyKernelID = _optionalRendererBufferCopyComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_OPTIONAL_RENDERER_BUFFER_COPY_KERNEL);

            _animationBufferToTextureComputeShader = (ComputeShader)Resources.Load(GPUICrowdConstants.COMPUTE_ANIMATION_BUFFER_TO_TEXTURE);
            _animationBufferToTextureKernelID = _animationBufferToTextureComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_ANIMATION_BUFFER_TO_TEXTURE_KERNEL);
        }

        public override void OnEnable()
        {
            if (Application.isPlaying)
            {
                int count = prefabList.Count;
                for (int i = 0; i < count; i++)
                {
                    GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)prototypeList[i];
                    if (crowdPrototype.animationData != null && crowdPrototype.animationData.skinnedMeshDataList != null)
                    {
                        if (crowdPrototype.animationData.applyRootMotion)
                        {
                            crowdPrototype.enableRuntimeModifications = true;
                            crowdPrototype.autoUpdateTransformData = true;
                        }
                        if (crowdPrototype.hasOptionalRenderers)
                        {
                            SkinnedMeshRenderer[] skinnedMeshRenderers = crowdPrototype.prefabObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                            foreach (GPUISkinnedMeshData smd in crowdPrototype.animationData.skinnedMeshDataList)
                            {
                                if (smd.isOptional && !smd.isOptionalPrototypeGenerated)
                                {
                                    foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                                    {
                                        if (skinnedMeshRenderer.gameObject.name.Equals(smd.transformName))
                                        {
                                            GPUICrowdPrototype generatedPrototype = DefineGameObjectAsCrowdPrototypeAtRuntime(skinnedMeshRenderer.gameObject, crowdPrototype.animationData, false,
                                                crowdPrototype);
                                            generatedPrototype.hasOptionalRenderers = false;

                                            generatedPrototype.enableRuntimeModifications = true;
                                            generatedPrototype.addRemoveInstancesAtRuntime = false;
                                            generatedPrototype.isChildPrototype = true;
                                            generatedPrototype.parentPrototype = crowdPrototype;
                                            generatedPrototype.autoUpdateTransformData = false;

                                            smd.isOptionalPrototypeGenerated = true;
                                            smd.optionalPrototype = generatedPrototype;
                                            break;
                                        }
                                    }
                                }
                                else if (smd.isOptional && smd.optionalPrototype != null && !prototypeList.Contains(smd.optionalPrototype))
                                {
                                    prototypeList.Add(smd.optionalPrototype);
                                    prefabList.Add(smd.optionalPrototype.prefabObject);
                                }
                            }
                        }
                    }
                }
            }
            base.OnEnable();
            
            if (Application.isPlaying && animationEvents != null)
            {
                foreach (GPUIAnimationEvent animationEvent in animationEvents)
                {
                    if (animationEvent.prototype.animationData != null && animationEvent.prototype.animationData.useCrowdAnimator)
                        GPUICrowdAPI.AddAnimationEvent(this, animationEvent);
                }
            }
        }

        public override void Update()
        {
            _transitionFrequency = 1.0f / 60.0f * Time.timeScale;

            float currentTime = Time.time;
            float currentDeltaTime = Time.deltaTime;
            bool calculateTransitions = currentTime - _lastTransitionUpdateTime > _transitionFrequency;

            if (runtimeDataList != null)
            {
                foreach (GPUICrowdRuntimeData runtimeData in runtimeDataList)
                {
                    if (runtimeData.isChildRuntimeData)
                        continue;

                    GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)runtimeData.prototype;

                    if (crowdPrototype.animationData == null)
                        continue;

                    // Crowd Animator Root Motion
                    if (crowdPrototype.animationData.useCrowdAnimator && runtimeData.instanceCount > 0)
                    {
                        if (crowdPrototype.animationData.applyRootMotion && runtimeData.instanceTransformAccessArray.isCreated)
                        {
#if UNITY_EDITOR
                            UnityEngine.Profiling.Profiler.BeginSample("GPUI Crowd Animator Root Motion");
#endif
                            runtimeData.dependentJob.Complete();

                            runtimeData.dependentJob = new ApplyCrowdAnimatorRootMotionJob()
                            {
                                currentTime = currentTime,
                                lerpAmount = (currentTime - _lastRootMotionUpdateTime) * crowdPrototype.frameRate,
                                animationData = runtimeData.animationData,
                                crowdAnimatorControllerData = runtimeData.crowdAnimatorControllerData,
                                clipDatas = runtimeData.clipDatas,
                                rootMotions = runtimeData.rootMotions
                            }.Schedule(runtimeData.instanceTransformAccessArray);
#if UNITY_EDITOR
                            UnityEngine.Profiling.Profiler.EndSample();
#endif
                        }
                        if (calculateTransitions)
                        {
#if UNITY_EDITOR
                            UnityEngine.Profiling.Profiler.BeginSample("GPUI Crowd Animator Transition");
#endif
                            int transitionCount = runtimeData.transitioningAnimators.Count;
                            if (transitionCount > 0)
                            {
                                runtimeData.dependentJob.Complete();
                                for (int i = 0; i < transitionCount; i++)
                                {
                                    if (!runtimeData.transitioningAnimators[i].ApplyTransition(runtimeData, currentTime))
                                    {
                                        i--;
                                        transitionCount--;
                                    }
                                }
                                runtimeData.animationDataModified = true;
                            }
#if UNITY_EDITOR
                            UnityEngine.Profiling.Profiler.EndSample();
#endif
                        }
                        if (runtimeData.hasEvents)
                        {
#if UNITY_EDITOR
                            UnityEngine.Profiling.Profiler.BeginSample("GPUI Crowd Animator Events");
#endif
                            runtimeData.dependentJob.Complete();
                            List<GPUInstancerPrefab> prefabInstanceList = _registeredPrefabsRuntimeData[crowdPrototype];
                            for (int i = prefabInstanceList.Count - 1; i >= 0; i--) // switched to for loop to not cause issues when instance is removed in the event
                            {
                                GPUICrowdPrefab prefabInstance = (GPUICrowdPrefab)prefabInstanceList[i];
                                if (prefabInstance.state == PrefabInstancingState.Instanced)
                                {
                                    prefabInstance.crowdAnimator.ApplyAnimationEvents(runtimeData, prefabInstance, currentTime, currentDeltaTime);
                                }
                            }
#if UNITY_EDITOR
                            UnityEngine.Profiling.Profiler.EndSample();
#endif
                        }
                        if (crowdPrototype.animationData.applyBoneUpdates)
                        {
                            runtimeData.MakeBoneDataRequest();
                        }
                    }
                }
            }

            _lastRootMotionUpdateTime = currentTime;

            if (calculateTransitions)
                _lastTransitionUpdateTime = currentTime;

            base.Update();
        }

        public override void LateUpdate()
        {
            if (Application.isPlaying && runtimeDataList != null)
            {
                foreach (GPUICrowdRuntimeData runtimeData in runtimeDataList)
                {
                    runtimeData.dependentJob.Complete();
                    UpdateAnimatorsData(runtimeData); // calculate baked animation
                    // Can inject code here to modify bone matrix buffers
                }
                _lastAnimateTime = Time.time;
            }
            base.LateUpdate(); // GPUI core rendering
        }
        #endregion MonoBehavior Methods

        public override void GeneratePrototypes(bool forceNew = false)
        {
            ClearInstancingData();

            if (forceNew || prototypeList == null)
                prototypeList = new List<GPUInstancerPrototype>();
            else
                prototypeList.RemoveAll(p => p == null);

            GPUInstancerConstants.gpuiSettings.SetDefultBindings();

            GPUICrowdUtility.SetCrowdPrefabPrototypes(gameObject, prototypeList, prefabList, forceNew);
        }

#if UNITY_EDITOR
        public override void CheckPrototypeChanges()
        {
            if (!GPUInstancerConstants.gpuiSettings.shaderBindings.HasExtension(GPUICrowdConstants.GPUI_EXTENSION_CODE))
            {
                GPUInstancerConstants.gpuiSettings.shaderBindings.AddExtension(new GPUICrowdShaderBindings());
            }

            if (prototypeList == null)
                GeneratePrototypes();
            else
                prototypeList.RemoveAll(p => p == null);

            if (GPUInstancerConstants.gpuiSettings != null && GPUInstancerConstants.gpuiSettings.shaderBindings != null)
            {
                GPUInstancerConstants.gpuiSettings.shaderBindings.ClearEmptyShaderInstances();
                foreach (GPUInstancerPrototype prototype in prototypeList)
                {
                    if (prototype.prefabObject != null)
                    {
                        GPUICrowdUtility.GenerateInstancedShadersForGameObject(prototype);
                        if (string.IsNullOrEmpty(prototype.warningText))
                        {
                            if (prototype.prefabObject.GetComponentInChildren<SkinnedMeshRenderer>() == null)
                            {
                                prototype.warningText = "Prefab object does not contain any Skinned Mesh Renderers.";
                            }
                        }
                    }
                }
            }
            if (GPUInstancerConstants.gpuiSettings != null && GPUInstancerConstants.gpuiSettings.billboardAtlasBindings != null)
            {
                GPUInstancerConstants.gpuiSettings.billboardAtlasBindings.ClearEmptyBillboardAtlases();
            }

            if (prefabList == null)
                prefabList = new List<GameObject>();

            prefabList.RemoveAll(p => p == null);
            prefabList.RemoveAll(p => p.GetComponent<GPUICrowdPrefab>() == null);
            prototypeList.RemoveAll(p => p == null);
            prototypeList.RemoveAll(p => !prefabList.Contains(p.prefabObject));

            if (prefabList.Count != prototypeList.Count)
                GeneratePrototypes();

            registeredPrefabs.RemoveAll(rpd => !prototypeList.Contains(rpd.prefabPrototype));
            foreach (GPUInstancerPrefabPrototype prototype in prototypeList)
            {
                if (!registeredPrefabs.Exists(rpd => rpd.prefabPrototype == prototype))
                    registeredPrefabs.Add(new RegisteredPrefabsData(prototype));
            }
        }
#endif // UNITY_EDITOR


        public override void InitializeRuntimeDataAndBuffers(bool forceNew = true)
        {
            base.InitializeRuntimeDataAndBuffers(forceNew);

            foreach (GPUICrowdPrototype prototype in _registeredPrefabsRuntimeData.Keys)
            {
                if (!prototype.animationData)
                    continue;
                GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData)GetRuntimeData(prototype);
                foreach (GPUICrowdPrefab instance in _registeredPrefabsRuntimeData[prototype])
                {
                    if (instance != null)
                    {
                        instance.SetupPrefabInstance(runtimeData, forceNew);

                        if (instance.childCrowdPrefabs != null)
                        {
                            foreach (GPUICrowdPrefab childInstance in instance.childCrowdPrefabs)
                            {
                                childInstance.SetupPrefabInstance(GetRuntimeData(childInstance.prefabPrototype));
                            }
                        }
                    }
                }
            }
        }

        public override GPUInstancerRuntimeData InitializeRuntimeDataForPrefabPrototype(GPUInstancerPrefabPrototype p, int additionalBufferSize = 0)
        {
            GPUICrowdRuntimeData runtimeData = (GPUICrowdRuntimeData) GetRuntimeData(p);
            if (runtimeData == null)
            {
                runtimeData = new GPUICrowdRuntimeData(p);
                if (!runtimeData.CreateRenderersFromGameObject(p))
                    return null;
                runtimeDataList.Add(runtimeData);
                runtimeDataDictionary.Add(p, runtimeData);
                if (p.isShadowCasting)
                {
                    runtimeData.hasShadowCasterBuffer = true;
                    if (!p.useOriginalShaderForShadow)
                    {
                        runtimeData.shadowCasterMaterial = new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_SHADOWS_ONLY));
                    }
                }
            }

            base.InitializeRuntimeDataForPrefabPrototype(p, additionalBufferSize);

            GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)p;
            if (crowdPrototype.isChildPrototype)
            {
                runtimeData.isChildRuntimeData = true;
                runtimeData.parentRuntimeData = (GPUICrowdRuntimeData)GetRuntimeData(crowdPrototype.parentPrototype);
                runtimeData.bufferSize = runtimeData.parentRuntimeData.bufferSize;
                runtimeData.instanceCount = runtimeData.parentRuntimeData.instanceCount;
                if (runtimeData.instanceDataNativeArray.IsCreated)
                    runtimeData.instanceDataNativeArray.Dispose();
                runtimeData.instanceDataNativeArray = new Unity.Collections.NativeArray<Matrix4x4>(runtimeData.parentRuntimeData.bufferSize, Unity.Collections.Allocator.Persistent);
            }

            return runtimeData;
        }

        public override void SetRenderersEnabled(GPUInstancerPrefab prefabInstance, bool enabled)
        {
            if (!prefabInstance || !prefabInstance.prefabPrototype || !prefabInstance.prefabPrototype.prefabObject)
                return;

            GPUICrowdPrototype prototype = (GPUICrowdPrototype)prefabInstance.prefabPrototype;

            if (!prototype.animationData)
                return;

            GPUICrowdPrefab crowdPrefabInstance = (GPUICrowdPrefab)prefabInstance;
            if (crowdPrefabInstance.animatorRef == null)
                crowdPrefabInstance.animatorRef = crowdPrefabInstance.GetAnimator();
            LODGroup lodGroup = prefabInstance.GetComponent<LODGroup>();

            if (lodGroup != null)
                lodGroup.enabled = enabled;

            if (crowdPrefabInstance.animatorRef != null && enabled && prototype.animationData.IsOptimizeGameObjects())
                AnimatorUtility.DeoptimizeTransformHierarchy(crowdPrefabInstance.animatorRef.gameObject);

            if (enabled && lodGroup != null && prototype.animationData.IsOptimizeGameObjects())
            {
                LOD[] lods = lodGroup.GetLODs();
                LOD lod;
                for (int l = 0; l < lodGroup.lodCount; l++)
                {
                    lod = lods[l];
                    for (int r = 0; r < lod.renderers.Length; r++)
                    {
                        if (lod.renderers[r] is SkinnedMeshRenderer)
                        {
                            if (l > 0)
                                ((SkinnedMeshRenderer)lod.renderers[r]).bones = ((SkinnedMeshRenderer)lods[0].renderers[r]).bones;
                            if (GPUInstancerUtility.IsInLayer(layerMask, lod.renderers[r].gameObject.layer))
                                lod.renderers[r].enabled = enabled;
                        }
                    }
                }
            }
            else
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers = prefabInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                if (skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
                {
                    for (int mr = 0; mr < skinnedMeshRenderers.Length; mr++)
                    {
                        SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRenderers[mr];
                        if (GPUInstancerUtility.IsInLayer(layerMask, skinnedMeshRenderer.gameObject.layer))
                            skinnedMeshRenderer.enabled = enabled;
                    }
                }
            }

            if (crowdPrefabInstance.animatorRef != null)
            {
                if (!enabled && prototype.animationData.IsOptimizeGameObjects())
                    AnimatorUtility.OptimizeTransformHierarchy(crowdPrefabInstance.animatorRef.gameObject, prototype.animationData.exposedTransforms);

                if (prototype.animationData.useCrowdAnimator)
                    crowdPrefabInstance.animatorRef.enabled = enabled;
                if (enabled)
                    crowdPrefabInstance.animatorRef.cullingMode = prototype.animatorCullingMode;
            }
        }

        public void UpdateAnimatorsData(GPUICrowdRuntimeData runtimeData)
        {
            if (runtimeData.instanceCount == 0)
                return;

            GPUICrowdPrototype prototype = (GPUICrowdPrototype)runtimeData.prototype;

            if (prototype.animationData != null && runtimeData.animationDataBuffer != null)
            {
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample("GPUI Crowd Manager Set Modified Buffers");
#endif
                if (runtimeData.transformDataModified)
                {
                    if (runtimeData.instanceDataNativeArray.IsCreated)
                        runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray);
                    runtimeData.transformDataModified = false;
                }

                if (prototype.isChildPrototype)
                {
                    _optionalRendererBufferCopyComputeShader.SetBuffer(_optionalRendererBufferCopyKernelID, GPUICrowdConstants.CrowdKernelPoperties.CHILD_INSTANCE_DATA, runtimeData.transformationMatrixVisibilityBuffer);
                    _optionalRendererBufferCopyComputeShader.SetBuffer(_optionalRendererBufferCopyKernelID, GPUICrowdConstants.CrowdKernelPoperties.PARENT_INSTANCE_DATA, runtimeData.parentRuntimeData.transformationMatrixVisibilityBuffer);
                    _optionalRendererBufferCopyComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.INSTANCE_COUNT, runtimeData.instanceCount);

                    _optionalRendererBufferCopyComputeShader.Dispatch(_optionalRendererBufferCopyKernelID,
                        Mathf.CeilToInt(runtimeData.instanceCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT), 1, 1);

#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                    return;
                }

                if (runtimeData.animationDataModified)
                {
                    runtimeData.animationDataBuffer.SetData(runtimeData.animationData);
                    runtimeData.animationDataModified = false;
                }
                if (runtimeData.crowdAnimatorDataModified)
                {
                    runtimeData.crowdAnimatorControllerBuffer.SetData(runtimeData.crowdAnimatorControllerData);
                    runtimeData.crowdAnimatorDataModified = false;
                }
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                #region Crowd Animator
                if (prototype.animationData.useCrowdAnimator)
                {
                    _crowdAnimatorComputeShader.SetBuffer(_crowdAnimatorKernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_DATA, runtimeData.animationDataBuffer);
                    _crowdAnimatorComputeShader.SetBuffer(_crowdAnimatorKernelID, GPUICrowdConstants.CrowdKernelPoperties.CROWD_ANIMATOR_CONTROLLER, runtimeData.crowdAnimatorControllerBuffer);
                    _crowdAnimatorComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.INSTANCE_COUNT, runtimeData.instanceCount);
                    _crowdAnimatorComputeShader.SetFloat(GPUICrowdConstants.CrowdKernelPoperties.CURRENT_TIME, Time.time);
                    _crowdAnimatorComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.FRAME_RATE, prototype.frameRate);

                    _crowdAnimatorComputeShader.Dispatch(_crowdAnimatorKernelID,
                        Mathf.CeilToInt(runtimeData.instanceCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT), 1, 1);
                }
                #endregion Crowd Animator

                #region Mecanim Animator
                else
                {
#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.BeginSample("GPUI UpdateAnimatorsData");
#endif
                    foreach (GPUICrowdPrefab prefabInstance in _registeredPrefabsRuntimeData[runtimeData.prototype])
                    {
                        if (prefabInstance.state == PrefabInstancingState.Instanced)
                        {
                            prefabInstance.mecanimAnimator.UpdateDataFromMecanimAnimator(runtimeData, prefabInstance.gpuInstancerID - 1, prefabInstance.animatorRef, _animatorClipInfos);
                        }
                    }
                    runtimeData.animationDataBuffer.SetData(runtimeData.animationData);
#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                }
                #endregion Mecanim Animator

                #region Fix Weights
                // Fix weights
                _skinnedMeshAnimateComputeShader.SetBuffer(_fixWeightsKernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_DATA, runtimeData.animationDataBuffer);
                _skinnedMeshAnimateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.INSTANCE_COUNT, runtimeData.instanceCount);

                _skinnedMeshAnimateComputeShader.Dispatch(_fixWeightsKernelID,
                    Mathf.CeilToInt(runtimeData.instanceCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT), 1, 1);
                #endregion Fix Weights

                #region Apply Bone Transforms
                int kernelID = _animateBonesLerpedKernelID;
                if (runtimeData.disableFrameLerp)
                {
                    kernelID = _animateBonesKernelID;
                    runtimeData.disableFrameLerp = false;
                }
                // Apply bone transforms
                _skinnedMeshAnimateComputeShader.SetBuffer(kernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_DATA, runtimeData.animationDataBuffer);
                _skinnedMeshAnimateComputeShader.SetBuffer(kernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER, runtimeData.animationBakeBuffer);
                _skinnedMeshAnimateComputeShader.SetTexture(kernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_TEXTURE, prototype.animationData.animationTexture);
                _skinnedMeshAnimateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_TEXTURE_SIZE_X, prototype.animationData.textureSizeX);
                _skinnedMeshAnimateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.TOTAL_NUMBER_OF_FRAMES, prototype.animationData.totalFrameCount);
                _skinnedMeshAnimateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.TOTAL_NUMBER_OF_BONES, prototype.animationData.totalBoneCount);
                _skinnedMeshAnimateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.INSTANCE_COUNT, runtimeData.instanceCount);
                _skinnedMeshAnimateComputeShader.SetFloat(GPUICrowdConstants.CrowdKernelPoperties.DELTA_TIME, Time.time - _lastAnimateTime);
                _skinnedMeshAnimateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.FRAME_RATE, prototype.frameRate);

                _skinnedMeshAnimateComputeShader.Dispatch(kernelID,
                    Mathf.CeilToInt(runtimeData.instanceCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                    Mathf.CeilToInt(prototype.animationData.totalBoneCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                    1);

                if (prototype.animationData.useCrowdAnimator && prototype.animationData.applyBoneUpdates && !prototype.animationData.isSynchronousBoneUpdates)
                {
                    _asyncBoneUpdateComputeShader.SetBuffer(_asyncBoneUpdateKernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER, runtimeData.animationBakeBuffer);
                    _asyncBoneUpdateComputeShader.SetBuffer(_asyncBoneUpdateKernelID, GPUICrowdConstants.CrowdKernelPoperties.ASYNC_BONE_UPDATE_DATA, runtimeData.asyncBoneUpdateDataBuffer);
                    _asyncBoneUpdateComputeShader.SetBuffer(_asyncBoneUpdateKernelID, GPUICrowdConstants.CrowdKernelPoperties.ASYNC_BONE_UPDATE_FILTER, runtimeData.asyncBoneUpdateFilterBuffer);
                    _asyncBoneUpdateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.TOTAL_NUMBER_OF_BONES, prototype.animationData.totalBoneCount);
                    _asyncBoneUpdateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.INSTANCE_COUNT, runtimeData.instanceCount);
                    _asyncBoneUpdateComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.BONE_FILTER_COUNT, runtimeData.asyncBoneUpdateFilterBuffer.count);

                    _asyncBoneUpdateComputeShader.Dispatch(_asyncBoneUpdateKernelID,
                        Mathf.CeilToInt(runtimeData.instanceCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                        Mathf.CeilToInt(runtimeData.asyncBoneUpdateFilterBuffer.count / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                        1);
                    runtimeData.CompleteAsyncBoneDataRequest(false, prototype.animationData.asyncBoneUpdateMaxLatency - 1);
                }

                #endregion Apply Bone Transforms

                #region Copy To Texture
                if (GPUInstancerUtility.matrixHandlingType == GPUIMatrixHandlingType.CopyToTexture)
                {
                    _animationBufferToTextureComputeShader.SetBuffer(_animationBufferToTextureKernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER, runtimeData.animationBakeBuffer);
                    _animationBufferToTextureComputeShader.SetTexture(_animationBufferToTextureKernelID, GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER_TEXTURE, runtimeData.animationBakeTexture);
                    _animationBufferToTextureComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.TOTAL_NUMBER_OF_BONES, prototype.animationData.totalBoneCount);
                    _animationBufferToTextureComputeShader.SetInt(GPUICrowdConstants.CrowdKernelPoperties.INSTANCE_COUNT, runtimeData.instanceCount);
                    _animationBufferToTextureComputeShader.SetInt(GPUInstancerConstants.VisibilityKernelPoperties.MAX_TEXTURE_SIZE, GPUInstancerConstants.TEXTURE_MAX_SIZE);
                    _animationBufferToTextureComputeShader.Dispatch(_animationBufferToTextureKernelID,
                        Mathf.CeilToInt(runtimeData.instanceCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                        Mathf.CeilToInt(prototype.animationData.totalBoneCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                        1);
                }
                #endregion Copy To Texture
            }
        }

        public GPUICrowdPrototype DefineGameObjectAsCrowdPrototypeAtRuntime(GameObject prototypeGameObject, GPUICrowdAnimationData animationData, bool attachScript = true, 
            GPUICrowdPrototype copySettingsFrom = null)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("DefineGameObjectAsCrowdPrototypeAtRuntime method is designed to use at runtime. Prototype generation canceled.");
                return null;
            }

            if (prefabList == null)
                prefabList = new List<GameObject>();
            GPUICrowdPrototype crowdPrototype = GPUICrowdUtility.GenerateCrowdPrototype(prototypeGameObject, false, attachScript, copySettingsFrom);
            if (!prototypeList.Contains(crowdPrototype))
                prototypeList.Add(crowdPrototype);
            if (!prefabList.Contains(prototypeGameObject))
                prefabList.Add(prototypeGameObject);
            if (crowdPrototype.minCullingDistance < minCullingDistance)
                crowdPrototype.minCullingDistance = minCullingDistance;
            crowdPrototype.animationData = animationData;

            return crowdPrototype;
        }

        public override void DeletePrototype(GPUInstancerPrototype prototype, bool removeSO = true)
        {
            GPUICrowdPrototype crowdPrototype = (GPUICrowdPrototype)prototype;
#if UNITY_EDITOR
            if (removeSO && crowdPrototype.animationData != null)
            {
                if (crowdPrototype.animationData.animationTexture != null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(crowdPrototype.animationData.animationTexture));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(crowdPrototype.animationData));
            }
#endif

            base.DeletePrototype(prototype, removeSO);
        }

        public override void AddPrefabInstance(GPUInstancerPrefab prefabInstance, bool automaticallyIncreaseBufferSize = false)
        {
            base.AddPrefabInstance(prefabInstance, automaticallyIncreaseBufferSize);
            GPUICrowdPrefab crowdPrefab = (GPUICrowdPrefab)prefabInstance;
            if (crowdPrefab.hasChildPrefabs)
            {
                foreach (GPUICrowdPrefab childPrefab in crowdPrefab.childCrowdPrefabs)
                {
                    GPUInstancerRuntimeData runtimeData = GetRuntimeData(childPrefab.prefabPrototype);
                    if (runtimeData.bufferSize != crowdPrefab.runtimeData.bufferSize)
                    {
                        ExpandBufferSize(runtimeData, crowdPrefab.runtimeData.bufferSize);
                        GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
                    }
                    runtimeData.instanceCount = crowdPrefab.runtimeData.instanceCount;
                    childPrefab.SetupPrefabInstance(runtimeData);
                }
            }
        }
    }
}
#else //GPU_INSTANCER
namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdManager : MonoBehaviour
    {
    }
}
#endif //GPU_INSTANCER