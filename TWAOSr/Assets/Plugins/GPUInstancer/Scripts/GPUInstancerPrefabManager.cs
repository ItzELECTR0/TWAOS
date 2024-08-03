using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.Collections;
using System.Linq;
using UnityEngine.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    [ExecuteInEditMode]
    public class GPUInstancerPrefabManager : GPUInstancerManager
    {
        [SerializeField]
        public List<RegisteredPrefabsData> registeredPrefabs = new List<RegisteredPrefabsData>();
        [SerializeField]
        public List<GameObject> prefabList;
        public bool enableMROnManagerDisable = true;
        public bool enableMROnRemoveInstance = true;

        protected List<GPUInstancerModificationCollider> _modificationColliders;
        protected Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>> _registeredPrefabsRuntimeData;
        protected List<IPrefabVariationData> _variationDataList;

        #region MonoBehavior Methods

        public override void Awake()
        {
            base.Awake();

            if (prefabList == null)
                prefabList = new List<GameObject>();
        }

        public override void Reset()
        {
            base.Reset();

            RegisterPrefabsInScene();
        }

        public override void Update()
        {
            base.Update();

            if (runtimeDataList != null && Application.isPlaying)
            {
                foreach (GPUInstancerRuntimeData runtimeData in runtimeDataList)
                {
                    if (runtimeData.prototype.autoUpdateTransformData && runtimeData.instanceCount > 0 && runtimeData.instanceDataNativeArray.IsCreated && runtimeData.instanceTransformAccessArray.isCreated)
                    {
                        runtimeData.dependentJob.Complete();
                        runtimeData.dependentJob = new AutoUpdateTransformsJob()
                        {
                            instanceDataNativeArray = runtimeData.instanceDataNativeArray
                        }.Schedule(runtimeData.instanceTransformAccessArray);
                        runtimeData.transformDataModified = true;
                    }
                }
            }
        }

        public override void LateUpdate()
        {
            if (runtimeDataList != null && Application.isPlaying)
            {
                foreach (GPUInstancerRuntimeData runtimeData in runtimeDataList)
                {
                    if (runtimeData.transformDataModified)
                    {
                        runtimeData.dependentJob.Complete();
                        if (runtimeData.instanceDataNativeArray.IsCreated && runtimeData.transformationMatrixVisibilityBuffer != null)
                            runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray);
                        runtimeData.transformDataModified = false;
                    }
                }
            }
            base.LateUpdate();
        }
        #endregion MonoBehavior Methods

        public override void ClearInstancingData()
        {
            base.ClearInstancingData();

            if (Application.isPlaying && _registeredPrefabsRuntimeData != null && enableMROnManagerDisable)
            {
                foreach (GPUInstancerPrefabPrototype p in _registeredPrefabsRuntimeData.Keys)
                {
                    if (p.meshRenderersDisabled)
                        continue;
                    foreach (GPUInstancerPrefab prefabInstance in _registeredPrefabsRuntimeData[p])
                    {
                        if (!prefabInstance)
                            continue;
#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
                        if (playModeState != PlayModeStateChange.EnteredEditMode && playModeState != PlayModeStateChange.ExitingPlayMode)
#endif
                            SetRenderersEnabled(prefabInstance, true);
                    }
                }
            }

            if (_variationDataList != null)
            {
                foreach (IPrefabVariationData pvd in _variationDataList)
                    pvd.ReleaseBuffer();
            }
        }

        public override void GeneratePrototypes(bool forceNew = false)
        {
            base.GeneratePrototypes();

            GPUInstancerUtility.SetPrefabInstancePrototypes(gameObject, prototypeList, prefabList, forceNew);
        }

#if UNITY_EDITOR
        public override void CheckPrototypeChanges()
        {
            base.CheckPrototypeChanges();

            if (prefabList == null)
                prefabList = new List<GameObject>();

            prefabList.RemoveAll(p => p == null);
            prefabList.RemoveAll(p => p.GetComponent<GPUInstancerPrefab>() == null);
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
#endif
        public override void InitializeRuntimeDataAndBuffers(bool forceNew = true)
        {
            base.InitializeRuntimeDataAndBuffers(forceNew);

            if (!forceNew && isInitialized)
                return;

            if (_registeredPrefabsRuntimeData == null)
                _registeredPrefabsRuntimeData = new Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>>();

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                if (registeredPrefabs != null && registeredPrefabs.Count > 0)
                {
                    foreach (RegisteredPrefabsData rpd in registeredPrefabs)
                    {
                        if (!_registeredPrefabsRuntimeData.ContainsKey(rpd.prefabPrototype))
                            _registeredPrefabsRuntimeData.Add(rpd.prefabPrototype, rpd.registeredPrefabs);
                        else
                            _registeredPrefabsRuntimeData[rpd.prefabPrototype].AddRange(rpd.registeredPrefabs);
                    }
                    registeredPrefabs.Clear();
                }

                if (_registeredPrefabsRuntimeData.Count != prototypeList.Count)
                {
                    foreach (GPUInstancerPrototype p in prototypeList)
                    {
                        if (!_registeredPrefabsRuntimeData.ContainsKey(p))
                            _registeredPrefabsRuntimeData.Add(p, new List<GPUInstancerPrefab>());
                    }
                }
#if UNITY_EDITOR
            }
#endif

            InitializeRuntimeDataRegisteredPrefabs();
            GPUInstancerUtility.InitializeGPUBuffers(runtimeDataList);
            isInitial = true;
            isInitialized = true;
        }

        public override void DeletePrototype(GPUInstancerPrototype prototype, bool removeSO = true)
        {
            base.DeletePrototype(prototype, removeSO);

            prefabList.Remove(prototype.prefabObject);
            if (removeSO)
            {
#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
                GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefab>(prototype.prefabObject);
                GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefabRuntimeHandler>(prototype.prefabObject);
#else
                DestroyImmediate(prototype.prefabObject.GetComponent<GPUInstancerPrefab>(), true);
                if (prototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>() != null)
                    DestroyImmediate(prototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>(), true);
#endif
#if UNITY_EDITOR
                EditorUtility.SetDirty(prototype.prefabObject);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prototype));
#endif
            }
            GeneratePrototypes(false);
        }

        public virtual void InitializeRuntimeDataRegisteredPrefabs(int additionalBufferSize = 0)
        {
            if (runtimeDataList == null)
                runtimeDataList = new List<GPUInstancerRuntimeData>();
            if (runtimeDataDictionary == null)
                runtimeDataDictionary = new Dictionary<GPUInstancerPrototype, GPUInstancerRuntimeData>();

            foreach (GPUInstancerPrefabPrototype p in prototypeList)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && !p.isTransformsSerialized && !p.meshRenderersDisabled)
                    continue;
#endif
                InitializeRuntimeDataForPrefabPrototype(p, additionalBufferSize);
            }
        }

        public virtual GPUInstancerRuntimeData InitializeRuntimeDataForPrefabPrototype(GPUInstancerPrefabPrototype p, int additionalBufferSize = 0)
        {
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                p.useOriginalShaderForShadow = true;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(p);
            if (runtimeData == null)
            {
                runtimeData = new GPUInstancerRuntimeData(p);
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

                GPUInstancerUtility.AddBillboardToRuntimeData(runtimeData);

                if (p.treeType == GPUInstancerTreeType.SpeedTree || p.treeType == GPUInstancerTreeType.SpeedTree8 || p.treeType == GPUInstancerTreeType.TreeCreatorTree)
                    AddTreeProxy(p, runtimeData);
            }
            runtimeData.dependentJob.Complete();
            int instanceCount = 0;
            List<GPUInstancerPrefab> registeredPrefabsList = null;
            if (p.isTransformsSerialized)
            {
                string matrixStr;
                System.IO.StringReader strReader = new System.IO.StringReader(p.serializedTransformData.text);
                List<Matrix4x4> matrixData = new List<Matrix4x4>();
                while (true)
                {
                    matrixStr = strReader.ReadLine();
                    if (!string.IsNullOrEmpty(matrixStr))
                    {
                        matrixData.Add(GPUInstancerUtility.Matrix4x4FromString(matrixStr));
                    }
                    else
                        break;
                }
                if (runtimeData.instanceDataNativeArray.IsCreated)
                    runtimeData.instanceDataNativeArray.Dispose();
                runtimeData.instanceDataNativeArray = new NativeArray<Matrix4x4>(matrixData.ToArray(), Allocator.Persistent);
                runtimeData.bufferSize = matrixData.Count + (p.enableRuntimeModifications && p.addRemoveInstancesAtRuntime ? p.extraBufferSize : 0) + additionalBufferSize;
                instanceCount = matrixData.Count;
            }
#if UNITY_EDITOR
            else if (!Application.isPlaying && p.meshRenderersDisabled)
            {
                List<GPUInstancerPrefab> prefabInstances = registeredPrefabs.Find(rpd => rpd.prefabPrototype == p).registeredPrefabs;
                runtimeData.ReleaseBuffers();
                runtimeData.bufferSize = prefabInstances.Count;
                runtimeData.instanceDataNativeArray = new NativeArray<Matrix4x4>(runtimeData.bufferSize, Allocator.Persistent);
                instanceCount = runtimeData.bufferSize;
                for (int i = 0; i < instanceCount; i++)
                {
                    runtimeData.instanceDataNativeArray[i] = prefabInstances[i].GetInstanceTransform().localToWorldMatrix;
                }
            }
#endif
            else if (_registeredPrefabsRuntimeData.TryGetValue(p, out registeredPrefabsList))
            {
                runtimeData.ReleaseBuffers();
                runtimeData.bufferSize = registeredPrefabsList.Count + (p.enableRuntimeModifications && p.addRemoveInstancesAtRuntime ? p.extraBufferSize : 0) + additionalBufferSize;
                if (runtimeData.bufferSize > 0)
                {
                    runtimeData.instanceDataNativeArray = new NativeArray<Matrix4x4>(runtimeData.bufferSize, Allocator.Persistent);

                    Matrix4x4 instanceData;
                    bool hasModificationColliders = p.enableRuntimeModifications && _modificationColliders != null && _modificationColliders.Count > 0;
                    bool hasAutoTransformUpdate = p.enableRuntimeModifications && p.autoUpdateTransformData;
                    if (hasAutoTransformUpdate && runtimeData.bufferSize > 0)
                    {
                        TransformAccessArray.Allocate(runtimeData.bufferSize, -1, out runtimeData.instanceTransformAccessArray);
                        runtimeData.instanceTransformAccessArray.SetTransforms(new Transform[runtimeData.bufferSize]);
                    }

                    foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsList)
                    {
                        if (!prefabInstance)
                            continue;

                        Transform instanceTransform = prefabInstance.GetInstanceTransform();
                        instanceData = instanceTransform.localToWorldMatrix;
                        prefabInstance.state = PrefabInstancingState.Instanced;

                        bool disableRenderers = true;

                        if (hasModificationColliders)
                        {
                            bool isInsideCollider = false;
                            foreach (GPUInstancerModificationCollider mc in _modificationColliders)
                            {
                                if (mc.IsInsideCollider(prefabInstance))
                                {
                                    isInsideCollider = true;
                                    mc.AddEnteredInstance(prefabInstance);
                                    instanceData = GPUInstancerConstants.zeroMatrix;
                                    prefabInstance.state = PrefabInstancingState.Disabled;
                                    disableRenderers = false;
                                    break;
                                }
                            }
                            if (!isInsideCollider)
                            {
                                if (p.startWithRigidBody && prefabInstance.GetComponent<Rigidbody>() != null)
                                {
                                    isInsideCollider = true;
                                    _modificationColliders[0].AddEnteredInstance(prefabInstance);
                                    instanceData = GPUInstancerConstants.zeroMatrix;
                                    prefabInstance.state = PrefabInstancingState.Disabled;
                                    disableRenderers = false;
                                }
                            }
                        }

                        if (disableRenderers && !p.meshRenderersDisabled)
                            SetRenderersEnabled(prefabInstance, false);

                        if (hasAutoTransformUpdate)
                            runtimeData.instanceTransformAccessArray[instanceCount] = instanceTransform;
                        runtimeData.instanceDataNativeArray[instanceCount] = instanceData;
                        instanceCount++;
                        prefabInstance.gpuInstancerID = instanceCount;
                    }
                }
            }

            // set instanceCount
            runtimeData.instanceCount = instanceCount;

            // variations
            if (_variationDataList != null)
            {
                foreach (IPrefabVariationData pvd in _variationDataList)
                {
                    if (pvd.GetPrototype() == p)
                    {
                        pvd.InitializeBufferAndArray(runtimeData.bufferSize);
                        if (registeredPrefabsList != null)
                        {
                            foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsList)
                            {
                                pvd.SetInstanceData(prefabInstance);
                            }
                        }
                        pvd.SetBufferData(0, 0, runtimeData.bufferSize);

                        for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                        {
                            for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                            {
                                pvd.SetVariation(runtimeData.instanceLODs[i].renderers[j]);
                            }
                        }
                    }
                }
            }
            return runtimeData;
        }

        public virtual void SetRenderersEnabled(GPUInstancerPrefab prefabInstance, bool enabled)
        {
            if (!prefabInstance || !prefabInstance.prefabPrototype || !prefabInstance.prefabPrototype.prefabObject)
                return;

            MeshRenderer[] meshRenderers = prefabInstance.GetComponentsInChildren<MeshRenderer>(true);
            if (meshRenderers != null && meshRenderers.Length > 0)
                for (int mr = 0; mr < meshRenderers.Length; mr++)
                    if (GPUInstancerUtility.IsInLayer(layerMask, meshRenderers[mr].gameObject.layer))
                        meshRenderers[mr].enabled = enabled;

            BillboardRenderer[] billboardRenderers = prefabInstance.GetComponentsInChildren<BillboardRenderer>(true);
            if (billboardRenderers != null && billboardRenderers.Length > 0)
                for (int mr = 0; mr < billboardRenderers.Length; mr++)
                    if (GPUInstancerUtility.IsInLayer(layerMask, billboardRenderers[mr].gameObject.layer))
                        billboardRenderers[mr].enabled = enabled;

            LODGroup lodGroup = prefabInstance.GetComponent<LODGroup>();
            if (lodGroup != null)
                lodGroup.enabled = enabled;

            Rigidbody rigidbody = prefabInstance.GetComponent<Rigidbody>();

            if (enabled)
            {
                if (rigidbody == null)
                {
                    GPUInstancerPrefabPrototype.RigidbodyData rigidbodyData = prefabInstance.prefabPrototype.rigidbodyData;
                    if (rigidbodyData != null && prefabInstance.prefabPrototype.hasRigidBody)
                    {
                        rigidbody = prefabInstance.gameObject.AddComponent<Rigidbody>();
                        rigidbody.useGravity = rigidbodyData.useGravity;
                        rigidbody.angularDamping = rigidbodyData.angularDrag;
                        rigidbody.mass = rigidbodyData.mass;
                        rigidbody.constraints = rigidbodyData.constraints;
                        rigidbody.detectCollisions = true;
                        rigidbody.linearDamping = rigidbodyData.drag;
                        rigidbody.isKinematic = rigidbodyData.isKinematic;
                        rigidbody.interpolation = rigidbodyData.interpolation;
                    }
                }
            }
            else if (rigidbody != null && !prefabInstance.prefabPrototype.autoUpdateTransformData)
                Destroy(rigidbody);
        }

        #region API Methods

        public static void ExpandBufferSize(GPUInstancerRuntimeData runtimeData, int newBufferSize)
        {
            if (runtimeData.bufferSize < newBufferSize)
            {
                runtimeData.bufferSize = newBufferSize;
                runtimeData.instanceDataNativeArray = GPUInstancerUtility.ResizeNativeArray(runtimeData.instanceDataNativeArray, newBufferSize, Allocator.Persistent);

                if (runtimeData.instanceTransformAccessArray.isCreated)
                {
                    runtimeData.instanceTransformAccessArray = GPUInstancerUtility.ResizeTransformAccessArray(runtimeData.instanceTransformAccessArray, newBufferSize);
                }
                else if (runtimeData.prototype.autoUpdateTransformData)
                {
                    TransformAccessArray.Allocate(newBufferSize, -1, out runtimeData.instanceTransformAccessArray);
                    runtimeData.instanceTransformAccessArray.SetTransforms(new Transform[newBufferSize]);
                }
            }
        }

        public virtual void SetInstanceTransform(GPUInstancerRuntimeData runtimeData, int index, Transform transform)
        {
            runtimeData.instanceDataNativeArray[index] = transform ? transform.localToWorldMatrix : GPUInstancerConstants.zeroMatrix;
            if (runtimeData.instanceTransformAccessArray.isCreated)
                runtimeData.instanceTransformAccessArray[index] = transform;
        }

        public virtual void DisableIntancingForInstance(GPUInstancerPrefab prefabInstance, bool setRenderersEnabled = true)
        {
            if (!prefabInstance)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null && prefabInstance.gpuInstancerID > 0 && prefabInstance.gpuInstancerID <= runtimeData.bufferSize)
            {
                runtimeData.dependentJob.Complete();
                prefabInstance.state = PrefabInstancingState.Disabled;
                SetInstanceTransform(runtimeData, prefabInstance.gpuInstancerID - 1, null);

                runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                if (setRenderersEnabled)
                    SetRenderersEnabled(prefabInstance, true);
                prefabInstance.SetupPrefabInstance(runtimeData);
            }
            else
            {
                Debug.LogWarning("Can not disable instancing for instance with id: " + prefabInstance.gpuInstancerID);
            }
        }

        public virtual void EnableInstancingForInstance(GPUInstancerPrefab prefabInstance, bool setRenderersDisabled = true)
        {
            if (!prefabInstance)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null && prefabInstance.gpuInstancerID > 0 && prefabInstance.gpuInstancerID <= runtimeData.bufferSize)
            {
                runtimeData.dependentJob.Complete();
                prefabInstance.state = PrefabInstancingState.Instanced;
                SetInstanceTransform(runtimeData, prefabInstance.gpuInstancerID - 1, prefabInstance.GetInstanceTransform());

                runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                if (setRenderersDisabled)
                    SetRenderersEnabled(prefabInstance, false);
                prefabInstance.SetupPrefabInstance(runtimeData);
            }
            else
            {
                Debug.LogWarning("Can not enable instancing for instance with id: " + prefabInstance.gpuInstancerID);
            }
        }

        public virtual void UpdateTransformDataForInstance(GPUInstancerPrefab prefabInstance)
        {
            if (!prefabInstance)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null && prefabInstance.gpuInstancerID > 0 && prefabInstance.gpuInstancerID <= runtimeData.bufferSize)
            {
                runtimeData.dependentJob.Complete();
                runtimeData.instanceDataNativeArray[prefabInstance.gpuInstancerID - 1] = prefabInstance.GetInstanceTransform().localToWorldMatrix;

                // automatically set in Update method
                runtimeData.transformDataModified = true;
                //runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
            }
            else
            {
                Debug.LogWarning("Can not update transform for instance with id: " + prefabInstance.gpuInstancerID);
            }
        }

        public virtual void AddPrefabInstance(GPUInstancerPrefab prefabInstance, bool automaticallyIncreaseBufferSize = false)
        {
            //#if UNITY_EDITOR
            //            UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerPrefabManager.AddPrefabInstance");
            //#endif
            if (!prefabInstance || prefabInstance.state == PrefabInstancingState.Instanced)
                return;

            if (runtimeDataList == null)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null)
            {
                runtimeData.dependentJob.Complete();
                Transform instanceTransform = prefabInstance.GetInstanceTransform();
                if (runtimeData.bufferSize == runtimeData.instanceCount)
                {
                    if (automaticallyIncreaseBufferSize)
                    {
                        ExpandBufferSize(runtimeData, runtimeData.bufferSize + 1024);
                        SetInstanceTransform(runtimeData, runtimeData.instanceCount, instanceTransform);
                        runtimeData.instanceCount++;
                        prefabInstance.gpuInstancerID = runtimeData.instanceCount;
                        _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Add(prefabInstance);
                        if (!prefabInstance.prefabPrototype.meshRenderersDisabled)
                            SetRenderersEnabled(prefabInstance, false);
                        prefabInstance.state = PrefabInstancingState.Instanced;
                        GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
                        prefabInstance.SetupPrefabInstance(runtimeData, true);

                        // variations
                        if (_variationDataList != null)
                        {
                            foreach (IPrefabVariationData pvd in _variationDataList)
                            {
                                if (pvd.GetPrototype() == prefabInstance.prefabPrototype)
                                {
                                    pvd.SetNewBufferSize(runtimeData.bufferSize);
                                    pvd.SetInstanceData(prefabInstance);
                                    pvd.SetBufferData(prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);

                                    for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                                    {
                                        for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                                        {
                                            pvd.SetVariation(runtimeData.instanceLODs[i].renderers[j]);
                                        }
                                    }
                                }
                            }
                        }

                        return;
                    }
                    else
                    {
                        Debug.LogWarning("Can not add instance. Buffer is full.");
                        return;
                    }
                }
                prefabInstance.state = PrefabInstancingState.Instanced;
                SetInstanceTransform(runtimeData, runtimeData.instanceCount, instanceTransform);
                runtimeData.instanceCount++;
                prefabInstance.gpuInstancerID = runtimeData.instanceCount;

                runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                if (!prefabInstance.prefabPrototype.meshRenderersDisabled)
                    SetRenderersEnabled(prefabInstance, false);

                if (!_registeredPrefabsRuntimeData.ContainsKey(prefabInstance.prefabPrototype))
                    _registeredPrefabsRuntimeData.Add(prefabInstance.prefabPrototype, new List<GPUInstancerPrefab>());
                _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Add(prefabInstance);

                // variations
                if (_variationDataList != null)
                {
                    foreach (IPrefabVariationData pvd in _variationDataList)
                    {
                        if (pvd.GetPrototype() == prefabInstance.prefabPrototype)
                        {
                            pvd.SetInstanceData(prefabInstance);
                            pvd.SetBufferData(prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                        }
                    }
                }

                prefabInstance.SetupPrefabInstance(runtimeData, true);
            }
            //#if UNITY_EDITOR
            //            UnityEngine.Profiling.Profiler.EndSample();
            //#endif
        }

        /// <summary>
        /// Adds prefab instances for multiple prototypes
        /// </summary>
        public virtual void AddPrefabInstances(IEnumerable<GPUInstancerPrefab> prefabInstances)
        {
            List<GPUInstancerPrefab>[] instanceLists = new List<GPUInstancerPrefab>[prototypeList.Count];
            Dictionary<GPUInstancerPrototype, int> indexDict = new Dictionary<GPUInstancerPrototype, int>();
            for (int i = 0; i < instanceLists.Length; i++)
            {
                instanceLists[i] = new List<GPUInstancerPrefab>();
                indexDict.Add(prototypeList[i], i);
            }

            foreach (GPUInstancerPrefab prefabInstance in prefabInstances)
            {
                instanceLists[indexDict[prefabInstance.prefabPrototype]].Add(prefabInstance);
            }

            for (int i = 0; i < instanceLists.Length; i++)
            {
                AddPrefabInstances((GPUInstancerPrefabPrototype)prototypeList[i], instanceLists[i]);
            }
        }

        /// <summary>
        /// Adds prefab instances for single prototype
        /// </summary>
        public virtual void AddPrefabInstances(GPUInstancerPrefabPrototype prototype, IEnumerable<GPUInstancerPrefab> prefabInstances)
        {
            if (prefabInstances == null)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return;

            int count = prefabInstances.Count();

            if (count == 0)
                return;

            runtimeData.dependentJob.Complete();
            GPUInstancerPrefab prefabInstance;
            if (runtimeData.instanceCount + count > runtimeData.bufferSize)
            {
                ExpandBufferSize(runtimeData, runtimeData.instanceCount + count);

                for (int i = 0; i < count; i++)
                {
                    prefabInstance = prefabInstances.ElementAt(i);
                    SetInstanceTransform(runtimeData, runtimeData.instanceCount + i, prefabInstance.GetInstanceTransform());
                    prefabInstance.gpuInstancerID = runtimeData.instanceCount + i + 1;
                    if (!prototype.meshRenderersDisabled)
                        SetRenderersEnabled(prefabInstance, false);
                    prefabInstance.state = PrefabInstancingState.Instanced;
                    prefabInstance.SetupPrefabInstance(runtimeData, true);
                }
                _registeredPrefabsRuntimeData[prototype].AddRange(prefabInstances);
                runtimeData.instanceCount = runtimeData.bufferSize;

                GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                prefabInstance = prefabInstances.ElementAt(i);
                SetInstanceTransform(runtimeData, runtimeData.instanceCount + i, prefabInstance.GetInstanceTransform());
                prefabInstance.gpuInstancerID = runtimeData.instanceCount + i + 1;
                if (!prototype.meshRenderersDisabled)
                    SetRenderersEnabled(prefabInstance, false);
                prefabInstance.state = PrefabInstancingState.Instanced;
                prefabInstance.SetupPrefabInstance(runtimeData, true);
            }
            _registeredPrefabsRuntimeData[prototype].AddRange(prefabInstances);
            runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray);
            runtimeData.instanceCount += count;
        }

        public virtual void UpdateInstanceDataArray(GPUInstancerRuntimeData runtimeData, List<GPUInstancerPrefab> prefabList)
        {
            runtimeData.ReleaseBuffers();
            int instanceCount = prefabList.Count;
            int bufferSize = instanceCount + ((GPUInstancerPrefabPrototype)runtimeData.prototype).extraBufferSize;

            runtimeData.instanceDataNativeArray = new NativeArray<Matrix4x4>(bufferSize, Allocator.Persistent);
            if (runtimeData.prototype.autoUpdateTransformData)
                TransformAccessArray.Allocate(bufferSize, -1, out runtimeData.instanceTransformAccessArray);

            for (int i = 0; i < prefabList.Count;)
            {
                GPUInstancerPrefab gPUInstancerPrefab = prefabList[i];
                SetInstanceTransform(runtimeData, i, gPUInstancerPrefab.GetInstanceTransform());
                gPUInstancerPrefab.gpuInstancerID = ++i;
            }
            runtimeData.instanceCount = instanceCount;
            runtimeData.bufferSize = bufferSize;
            GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
        }

        public virtual void RemovePrefabInstance(GPUInstancerPrefab prefabInstance, bool setRenderersEnabled = true)
        {
            //#if UNITY_EDITOR
            //            UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerPrefabManager.RemovePrefabInstance");
            //#endif
            if (!prefabInstance || prefabInstance.state == PrefabInstancingState.None)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype);
            if (runtimeData != null)
            {
                if (prefabInstance.gpuInstancerID > runtimeData.bufferSize || prefabInstance.gpuInstancerID <= 0)
                {
                    Debug.LogWarning("Instance can not be removed.");
                    return;
                }

                runtimeData.dependentJob.Complete();
                List<GPUInstancerPrefab> prefabInstanceList = _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype];

                if (prefabInstance.gpuInstancerID == runtimeData.instanceCount)
                {
                    prefabInstance.state = PrefabInstancingState.None;
                    SetInstanceTransform(runtimeData, prefabInstance.gpuInstancerID - 1, null);
                    runtimeData.instanceCount--;
                    prefabInstanceList.RemoveAt(prefabInstance.gpuInstancerID - 1);
                    if (setRenderersEnabled && enableMROnRemoveInstance && !prefabInstance.prefabPrototype.meshRenderersDisabled)
                        SetRenderersEnabled(prefabInstance, true);
                }
                else
                {
                    GPUInstancerPrefab lastIndexPrefabInstance = null;
                    for (int i = prefabInstanceList.Count - 1; i >= 0; i--)
                    {
                        GPUInstancerPrefab loopPI = prefabInstanceList[i];
                        if (loopPI == null)
                        {
                            prefabInstanceList.RemoveAt(i);
                            if (i < prefabInstanceList.Count - 1)
                                i++;
                        }
                        else if (loopPI.gpuInstancerID == runtimeData.instanceCount)
                        {
                            lastIndexPrefabInstance = loopPI;
                            break;
                        }
                    }
                    if (!lastIndexPrefabInstance)
                    {
                        prefabInstanceList.RemoveAll(pi => pi == null);
                        Debug.LogWarning("Prefab instance was destroyed before being removed from instance list in GPUI Prefab Manager!");
                        return;
                    }

                    prefabInstance.state = PrefabInstancingState.None;

                    // exchange last index with this one
                    SetInstanceTransform(runtimeData, prefabInstance.gpuInstancerID - 1, lastIndexPrefabInstance.GetInstanceTransform());
                    // set last index data to Matrix4x4.zero
                    SetInstanceTransform(runtimeData, lastIndexPrefabInstance.gpuInstancerID - 1, null);
                    runtimeData.instanceCount--;

                    runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataNativeArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);

                    prefabInstanceList.RemoveAt(lastIndexPrefabInstance.gpuInstancerID - 1);
                    lastIndexPrefabInstance.gpuInstancerID = prefabInstance.gpuInstancerID;
                    prefabInstanceList[lastIndexPrefabInstance.gpuInstancerID - 1] = lastIndexPrefabInstance;

                    if (setRenderersEnabled && enableMROnRemoveInstance && !prefabInstance.prefabPrototype.meshRenderersDisabled)
                        SetRenderersEnabled(prefabInstance, true);
                    //Destroy(prefabInstance);

                    // variations
                    if (_variationDataList != null)
                    {
                        foreach (IPrefabVariationData pvd in _variationDataList)
                        {
                            if (pvd.GetPrototype() == lastIndexPrefabInstance.prefabPrototype)
                            {
                                pvd.SetInstanceData(lastIndexPrefabInstance);
                                pvd.SetBufferData(lastIndexPrefabInstance.gpuInstancerID - 1, lastIndexPrefabInstance.gpuInstancerID - 1, 1);
                            }
                        }
                    }

                    lastIndexPrefabInstance.SetupPrefabInstance(runtimeData);
                }
            }

            //#if UNITY_EDITOR
            //            UnityEngine.Profiling.Profiler.EndSample();
            //#endif
        }

        /// <summary>
        /// Removes prefab instances for multiple prototypes
        /// </summary>
        public virtual void RemovePrefabInstances(IEnumerable<GPUInstancerPrefab> prefabInstances)
        {
            List<GPUInstancerPrefab>[] instanceLists = new List<GPUInstancerPrefab>[prototypeList.Count];
            Dictionary<GPUInstancerPrototype, int> indexDict = new Dictionary<GPUInstancerPrototype, int>();
            for (int i = 0; i < instanceLists.Length; i++)
            {
                instanceLists[i] = new List<GPUInstancerPrefab>();
                indexDict.Add(prototypeList[i], i);
            }

            foreach (GPUInstancerPrefab prefabInstance in prefabInstances)
            {
                instanceLists[indexDict[prefabInstance.prefabPrototype]].Add(prefabInstance);
            }
            for (int i = 0; i < instanceLists.Length; i++)
            {
                RemovePrefabInstances((GPUInstancerPrefabPrototype)prototypeList[i], instanceLists[i]);
            }
        }

        /// <summary>
        /// Removes prefab instances for single prototype
        /// </summary>
        public virtual void RemovePrefabInstances(GPUInstancerPrefabPrototype prototype, IEnumerable<GPUInstancerPrefab> prefabInstances)
        {
            if (prefabInstances == null || prefabInstances.Count() == 0)
                return;

            int count = prefabInstances.Count();

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return;

            List<GPUInstancerPrefab> prefabInstanceList = _registeredPrefabsRuntimeData[prototype];
            prefabInstanceList.RemoveRange(prefabInstances.ElementAt(0).gpuInstancerID - 1, count);
            foreach (GPUInstancerPrefab pi in prefabInstances)
            {
                if (enableMROnRemoveInstance && !prototype.meshRenderersDisabled)
                    SetRenderersEnabled(pi, true);
                pi.state = PrefabInstancingState.None;
                pi.gpuInstancerID = 0;
            }

            UpdateInstanceDataArray(runtimeData, prefabInstanceList);
        }

        public virtual void RegisterPrefabsInScene()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Registered prefabs changed");
#endif
            registeredPrefabs.Clear();
            foreach (GPUInstancerPrefabPrototype pp in prototypeList)
                registeredPrefabs.Add(new RegisteredPrefabsData(pp));

            GPUInstancerPrefab[] scenePrefabInstances = FindObjectsOfType<GPUInstancerPrefab>();
            foreach (GPUInstancerPrefab prefabInstance in scenePrefabInstances)
                AddRegisteredPrefab(prefabInstance);
        }

        public virtual void RegisterPrefabInstanceList(IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            if (_registeredPrefabsRuntimeData == null)
                _registeredPrefabsRuntimeData = new Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>>();
            if (_registeredPrefabsRuntimeData.Keys.Count != prototypeList.Count)
            {
                foreach (GPUInstancerPrototype prototype in prototypeList)
                    if (!_registeredPrefabsRuntimeData.ContainsKey(prototype))
                        _registeredPrefabsRuntimeData.Add(prototype, new List<GPUInstancerPrefab>());
            }

            foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
            {
                _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Add(prefabInstance);
            }
        }

        public virtual void UnregisterPrefabInstanceList(IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            if (_registeredPrefabsRuntimeData == null)
                _registeredPrefabsRuntimeData = new Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>>();
            if (_registeredPrefabsRuntimeData.Keys.Count != prototypeList.Count)
            {
                foreach (GPUInstancerPrototype prototype in prototypeList)
                    if (!_registeredPrefabsRuntimeData.ContainsKey(prototype))
                        _registeredPrefabsRuntimeData.Add(prototype, new List<GPUInstancerPrefab>());
            }

            foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
            {
                _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Remove(prefabInstance);
            }
        }

        public virtual void ClearRegisteredPrefabInstances()
        {
            foreach (GPUInstancerPrototype p in _registeredPrefabsRuntimeData.Keys)
            {
                _registeredPrefabsRuntimeData[p].Clear();
            }
        }

        public void ClearRegisteredPrefabInstances(GPUInstancerPrototype p)
        {
            if (_registeredPrefabsRuntimeData.TryGetValue(p, out List<GPUInstancerPrefab> prefabList))
                prefabList.Clear();
        }

        public virtual PrefabVariationData<T> DefinePrototypeVariationBuffer<T>(GPUInstancerPrefabPrototype prototype, string bufferName)
        {
            if (_variationDataList == null)
                _variationDataList = new List<IPrefabVariationData>();
            if (GPUInstancerUtility.matrixHandlingType == GPUIMatrixHandlingType.MatrixAppend)
            {
                Debug.LogError("GPUI can not define material variations in this platform and/or with this rendering settings.");
                return null;
            }
            PrefabVariationData<T> result = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    result = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (result == null)
            {
                result = new PrefabVariationData<T>(prototype, bufferName);
                _variationDataList.Add(result);

                if (isInitialized)
                {
                    GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
                    result.InitializeBufferAndArray(runtimeData.bufferSize);
                    if (_registeredPrefabsRuntimeData != null && _registeredPrefabsRuntimeData.TryGetValue(prototype, out List<GPUInstancerPrefab> prefabList))
                    {
                        foreach (GPUInstancerPrefab prefabInstance in prefabList)
                        {
                            result.SetInstanceData(prefabInstance);
                        }
                    }
                    result.SetBufferData(0, 0, runtimeData.bufferSize);

                    for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                    {
                        for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                        {
                            result.SetVariation(runtimeData.instanceLODs[i].renderers[j]);
                        }
                    }
                }
            }
            return result;
        }

        public virtual void UpdateVariationData<T>(GPUInstancerPrefab prefabInstance, string bufferName, T value)
        {
            if (!prefabInstance || !prefabInstance.prefabPrototype)
                return;
            PrefabVariationData<T> variationData = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prefabInstance.prefabPrototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    variationData = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (variationData != null && variationData.dataArray != null)
            {
                int index = prefabInstance.gpuInstancerID - 1;
                if (index >= 0 && index < variationData.dataArray.Length)
                {
                    variationData.dataArray[index] = value;
#if UNITY_2017_1_OR_NEWER
                    variationData.variationBuffer.SetData(variationData.dataArray, index, index, 1);
#else
                    variationData.variationBuffer.SetData(variationData.dataArray);
#endif
                }
            }
        }

        public virtual PrefabVariationData<T> DefineAndAddVariationFromArray<T>(GPUInstancerPrefabPrototype prototype, string bufferName, T[] variationArray)
        {
            PrefabVariationData<T> variationData = DefinePrototypeVariationBuffer<T>(prototype, bufferName);
            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
            if (runtimeData != null && variationData != null)
            {
                variationData.SetArrayAndInitializeBuffer(variationArray);
                variationData.SetBufferData(0, 0, Math.Min(runtimeData.bufferSize, variationArray.Length));
                for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        variationData.SetVariation(runtimeData.instanceLODs[l].renderers[r]);
                    }
                }
            }

            return variationData;
        }

        public virtual PrefabVariationData<T> DefineAndAddVariationFromArray<T>(GPUInstancerPrefabPrototype prototype, string bufferName, NativeArray<T> variationArray) where T : struct
        {
            PrefabVariationData<T> variationData = DefinePrototypeVariationBuffer<T>(prototype, bufferName);
            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
            if (runtimeData != null && variationData != null)
            {
                variationData.SetArrayAndInitializeBuffer(variationArray);
                for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        variationData.SetVariation(runtimeData.instanceLODs[l].renderers[r]);
                    }
                }
            }

            return variationData;
        }

        public virtual PrefabVariationData<T> UpdateVariationsFromArray<T>(GPUInstancerPrefabPrototype prototype, string bufferName, T[] variationArray,
            int arrayStartIndex = 0, int bufferStartIndex = 0, int count = 0)
        {
            PrefabVariationData<T> variationData = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    variationData = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (variationData != null)
            {
                GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
                if (runtimeData != null)
                {
                    variationData.dataArray = variationArray;
                    if (count > 0)
                        variationData.SetBufferData(arrayStartIndex, bufferStartIndex, count);
                    else
                        variationData.SetBufferData(0, 0, runtimeData.bufferSize);
                    for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                    {
                        for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                        {
                            variationData.SetVariation(runtimeData.instanceLODs[l].renderers[r]);
                        }
                    }
                }
            }

            return variationData;
        }

        public virtual PrefabVariationData<T> UpdateVariationsFromArray<T>(GPUInstancerPrefabPrototype prototype, string bufferName, NativeArray<T> variationArray,
            int arrayStartIndex = 0, int bufferStartIndex = 0, int count = 0) where T : struct
        {
            PrefabVariationData<T> variationData = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    variationData = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (variationData != null)
            {
                GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
                if (runtimeData != null)
                {
                    if (count > 0)
                        variationData.variationBuffer.SetData(variationArray, arrayStartIndex, bufferStartIndex, count);
                    else
                        variationData.variationBuffer.SetData(variationArray);
                    for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                    {
                        for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                        {
                            variationData.SetVariation(runtimeData.instanceLODs[l].renderers[r]);
                        }
                    }
                }
            }

            return variationData;
        }

        public ComputeBuffer GetVariationBuffer(GPUInstancerPrefabPrototype prototype, string bufferName)
        {
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prototype && item.GetBufferName() == bufferName)
                    return item.GetBuffer();
            }
            Debug.LogError(prototype + " does not contain a variation buffer with name: " + bufferName);
            return null;
        }

        public virtual GPUInstancerPrefabPrototype DefineGameObjectAsPrefabPrototypeAtRuntime(GameObject prototypeGameObject, bool attachScript = true)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("DefineGameObjectAsPrefabPrototypeAtRuntime method is designed to use at runtime. Prototype generation canceled.");
                return null;
            }

            if (prefabList == null)
                prefabList = new List<GameObject>();
            GPUInstancerPrefabPrototype prefabPrototype = GPUInstancerUtility.GeneratePrefabPrototype(prototypeGameObject, false, attachScript);
            if (!prototypeList.Contains(prefabPrototype))
                prototypeList.Add(prefabPrototype);
            if (!prefabList.Contains(prototypeGameObject))
                prefabList.Add(prototypeGameObject);
            if (prefabPrototype.minCullingDistance < minCullingDistance)
                prefabPrototype.minCullingDistance = minCullingDistance;

            return prefabPrototype;
        }

        public virtual void AddInstancesToPrefabPrototypeAtRuntime(GPUInstancerPrefabPrototype prefabPrototype, IEnumerable<GameObject> instances)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("AddInstancesToPrefabPrototypeAtRuntime method is designed to use at runtime. Adding instances canceled.");
                return;
            }

            if (isActiveAndEnabled)
            {
                List<GPUInstancerPrefab> instanceList;
                if (!_registeredPrefabsRuntimeData.TryGetValue(prefabPrototype, out instanceList))
                {
                    instanceList = new List<GPUInstancerPrefab>();
                    _registeredPrefabsRuntimeData.Add(prefabPrototype, instanceList);
                }

                GPUInstancerPrefab prefabInstance;
                foreach (GameObject instance in instances)
                {
                    prefabInstance = instance.GetComponent<GPUInstancerPrefab>();
                    if (prefabInstance == null)
                    {
                        prefabInstance = instance.AddComponent<GPUInstancerPrefab>();
                        prefabInstance.prefabPrototype = prefabPrototype;
                    }
                    if (prefabInstance != null && !instanceList.Contains(prefabInstance))
                        instanceList.Add(prefabInstance);
                }

                GPUInstancerRuntimeData runtimeData = InitializeRuntimeDataForPrefabPrototype(prefabPrototype, 0);
                GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
            }
            else
            {
                if (registeredPrefabs == null)
                    registeredPrefabs = new List<RegisteredPrefabsData>();

                RegisteredPrefabsData data = null;
                foreach (RegisteredPrefabsData item in registeredPrefabs)
                {
                    if (item.prefabPrototype == prefabPrototype)
                    {
                        data = item;
                        break;
                    }
                }
                if (data == null)
                {
                    data = new RegisteredPrefabsData(prefabPrototype);
                    registeredPrefabs.Add(data);
                }

                GPUInstancerPrefab prefabInstance;
                foreach (GameObject instance in instances)
                {
                    prefabInstance = instance.GetComponent<GPUInstancerPrefab>();
                    if (prefabInstance == null)
                    {
                        prefabInstance = instance.AddComponent<GPUInstancerPrefab>();
                        prefabInstance.prefabPrototype = prefabPrototype;
                    }
                    if (prefabInstance != null && !data.registeredPrefabs.Contains(prefabInstance))
                        data.registeredPrefabs.Add(prefabInstance);
                }
            }
        }

        public virtual void RemovePrototypeAtRuntime(GPUInstancerPrefabPrototype prefabPrototype)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("RemovePrototypeAtRuntime method is designed to use at runtime. Prototype removal canceled.");
                return;
            }

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabPrototype);
            if (runtimeData != null)
            {
                GPUInstancerUtility.ReleaseInstanceBuffers(runtimeData);
                if (runtimeDataList != null)
                    runtimeDataList.Remove(runtimeData);
                if (runtimeDataDictionary != null)
                    runtimeDataDictionary.Remove(runtimeData.prototype);
            }
            if (isActiveAndEnabled)
            {
                _registeredPrefabsRuntimeData.Remove(prefabPrototype);
            }
            else if (registeredPrefabs != null)
            {
                RegisteredPrefabsData data = null;
                foreach (RegisteredPrefabsData item in registeredPrefabs)
                {
                    if (item.prefabPrototype == prefabPrototype)
                    {
                        data = item;
                        break;
                    }
                }
                if (data != null)
                    registeredPrefabs.Remove(data);
            }
            if (prototypeList.Contains(prefabPrototype))
                prototypeList.Remove(prefabPrototype);
            if (prefabPrototype.prefabObject && prefabList.Contains(prefabPrototype.prefabObject))
                prefabList.Remove(prefabPrototype.prefabObject);
        }
        #endregion API Methods

        public virtual void AddRegisteredPrefab(GPUInstancerPrefab prefabInstance)
        {
            RegisteredPrefabsData data = null;
            foreach (RegisteredPrefabsData item in registeredPrefabs)
            {
                if (item.prefabPrototype == prefabInstance.prefabPrototype)
                {
                    data = item;
                    break;
                }
            }
            if (data != null)
                data.registeredPrefabs.Add(prefabInstance);
        }

        public virtual void AddRuntimeRegisteredPrefab(GPUInstancerPrefab prefabInstance)
        {
            List<GPUInstancerPrefab> list;
            if (!_registeredPrefabsRuntimeData.TryGetValue(prefabInstance.prefabPrototype, out list))
            {
                list = new List<GPUInstancerPrefab>();
                _registeredPrefabsRuntimeData.Add(prefabInstance.prefabPrototype, list);
            }

            if (!list.Contains(prefabInstance))
                list.Add(prefabInstance);
        }

        public virtual void AddModificationCollider(GPUInstancerModificationCollider modificationCollider)
        {
            if (_modificationColliders == null)
                _modificationColliders = new List<GPUInstancerModificationCollider>();

            _modificationColliders.Add(modificationCollider);
        }

        public virtual int GetEnabledPrefabCount()
        {
            int sum = 0;
            if (_modificationColliders != null)
            {
                for (int i = 0; i < _modificationColliders.Count; i++)
                    sum += _modificationColliders[i].GetEnteredInstanceCount();
            }
            return sum;
        }

        public virtual Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>> GetRegisteredPrefabsRuntimeData()
        {
            return _registeredPrefabsRuntimeData;
        }
    }

    [Serializable]
    public class RegisteredPrefabsData
    {
        public GPUInstancerPrefabPrototype prefabPrototype;
        public List<GPUInstancerPrefab> registeredPrefabs;

        public RegisteredPrefabsData(GPUInstancerPrefabPrototype prefabPrototype)
        {
            this.prefabPrototype = prefabPrototype;
            registeredPrefabs = new List<GPUInstancerPrefab>();
        }
    }

    public interface IPrefabVariationData
    {
        void InitializeBufferAndArray(int count, bool setDefaults = true);
        void SetInstanceData(GPUInstancerPrefab prefabInstance);
        void SetBufferData(int managedBufferStartIndex, int computeBufferStartIndex, int count);
        void SetVariation(GPUInstancerRenderer gpuiRenderer);
        void SetNewBufferSize(int newCount);
        GPUInstancerPrefabPrototype GetPrototype();
        string GetBufferName();
        ComputeBuffer GetBuffer();
        void ReleaseBuffer();
    }

    public class PrefabVariationData<T> : IPrefabVariationData
    {
        public GPUInstancerPrefabPrototype prototype;
        public string bufferName;
        public ComputeBuffer variationBuffer;
        public T[] dataArray;
        public T defaultValue;

        public PrefabVariationData(GPUInstancerPrefabPrototype prototype, string bufferName, T defaultValue = default(T))
        {
            this.prototype = prototype;
            this.bufferName = bufferName;
            this.defaultValue = defaultValue;
        }

        public void InitializeBufferAndArray(int count, bool setDefaults = true)
        {
            if (count == 0)
                return;
            dataArray = new T[count];
            if (setDefaults)
            {
                for (int i = 0; i < count; i++)
                {
                    dataArray[i] = defaultValue;
                }
            }
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = new ComputeBuffer(count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
        }

        public void SetArrayAndInitializeBuffer(T[] dataArray)
        {
            if (dataArray == null || dataArray.Length == 0)
                return;
            this.dataArray = dataArray;
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = new ComputeBuffer(dataArray.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
        }

        public void SetArrayAndInitializeBuffer<K>(NativeArray<K> dataArray) where K : struct
        {
            if (dataArray == null || dataArray.Length == 0)
                return;
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = new ComputeBuffer(dataArray.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(K)));
            variationBuffer.SetData(dataArray);
        }

        public void SetInstanceData(GPUInstancerPrefab prefabInstance)
        {
            if (prefabInstance.variationDataList != null && dataArray != null && prefabInstance.variationDataList.ContainsKey(bufferName) && dataArray.Length > prefabInstance.gpuInstancerID - 1)
                dataArray[prefabInstance.gpuInstancerID - 1] = (T)prefabInstance.variationDataList[bufferName];
        }

        public void SetBufferData(int managedBufferStartIndex, int computeBufferStartIndex, int count)
        {
            if (variationBuffer != null && count > 0 && dataArray != null)
            {
#if UNITY_2017_1_OR_NEWER
                variationBuffer.SetData(dataArray, managedBufferStartIndex, computeBufferStartIndex, count);
#else
                variationBuffer.SetData(dataArray);
#endif
            }
        }

        public void SetVariation(GPUInstancerRenderer gpuiRenderer)
        {
            if (variationBuffer != null && gpuiRenderer != null)
            {
                if (gpuiRenderer.mpb != null)
                    gpuiRenderer.mpb.SetBuffer(bufferName, variationBuffer);
                if (gpuiRenderer.shadowMPB != null)
                    gpuiRenderer.shadowMPB.SetBuffer(bufferName, variationBuffer);
            }
        }

        public void SetNewBufferSize(int newCount)
        {
            if (newCount <= 0)
                return;
            int count = 0;
            if (dataArray != null)
            {
                count = dataArray.Length;
                if (count < newCount)
                    Array.Resize<T>(ref dataArray, newCount);
            }
            else
                dataArray = new T[newCount];

            if (count < newCount)
            {
                if (variationBuffer != null)
                    variationBuffer.Release();
                variationBuffer = new ComputeBuffer(newCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
                Array.Resize<T>(ref dataArray, newCount);
                for (int i = count; i < newCount; i++)
                {
                    dataArray[i] = defaultValue;
                }
                variationBuffer.SetData(dataArray);
            }
        }

        public GPUInstancerPrefabPrototype GetPrototype()
        {
            return prototype;
        }

        public ComputeBuffer GetBuffer()
        {
            return variationBuffer;
        }

        public string GetBufferName()
        {
            return bufferName;
        }

        public void ReleaseBuffer()
        {
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = null;
            dataArray = null;
        }
    }
}