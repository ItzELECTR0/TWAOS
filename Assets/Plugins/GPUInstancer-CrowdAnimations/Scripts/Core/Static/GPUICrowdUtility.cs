#if GPU_INSTANCER
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Jobs;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer.CrowdAnimations
{
    public static class GPUICrowdUtility
    {
        #region Set Animation Data
        public static void SetAnimationData(GPUICrowdRuntimeData runtimeData)
        {
            GPUICrowdPrototype prototype = runtimeData.prototype as GPUICrowdPrototype;

            if (runtimeData.isChildRuntimeData)
            {
                runtimeData.animationDataBuffer = runtimeData.parentRuntimeData.animationDataBuffer;
                runtimeData.animationBakeBuffer = runtimeData.parentRuntimeData.animationBakeBuffer;
                return;
            }

            if (prototype.animationData != null && prototype.animationData.animationTexture != null)
            {
                if (!runtimeData.animationData.IsCreated || runtimeData.animationData.Length != runtimeData.bufferSize * 2)
                {
                    runtimeData.animationData = GPUInstancerUtility.ResizeNativeArray(runtimeData.animationData, runtimeData.bufferSize * 2, Allocator.Persistent);
                }
                if (runtimeData.animationDataBuffer == null || runtimeData.animationDataBuffer.count != runtimeData.bufferSize * 2)
                {
                    if (runtimeData.animationDataBuffer != null)
                        runtimeData.animationDataBuffer.Release();
                    runtimeData.animationDataBuffer = new ComputeBuffer(runtimeData.bufferSize * 2, GPUInstancerConstants.STRIDE_SIZE_FLOAT4);
                    runtimeData.animationDataBuffer.SetData(runtimeData.animationData);
                }
                if (runtimeData.animationBakeBuffer == null || runtimeData.animationBakeBuffer.count != prototype.animationData.totalBoneCount * runtimeData.bufferSize)
                {
                    if (runtimeData.animationBakeBuffer != null)
                        runtimeData.animationBakeBuffer.Release();
                    runtimeData.animationBakeBuffer = new ComputeBuffer(prototype.animationData.totalBoneCount * runtimeData.bufferSize, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);

                    if (GPUInstancerUtility.matrixHandlingType == GPUIMatrixHandlingType.CopyToTexture)
                    {
                        GPUInstancerUtility.DestroyObject(runtimeData.animationBakeTexture);
                        int elementCount = runtimeData.bufferSize * prototype.animationData.totalBoneCount;
                        int rowCount = Mathf.CeilToInt(elementCount / (float)GPUInstancerConstants.TEXTURE_MAX_SIZE);
                        runtimeData.animationBakeTexture = new RenderTexture(rowCount == 1 ? elementCount : GPUInstancerConstants.TEXTURE_MAX_SIZE, 4 * rowCount, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
                        {
                            isPowerOfTwo = false,
                            enableRandomWrite = true,
                            filterMode = FilterMode.Point,
                            useMipMap = false,
                            autoGenerateMips = false
                        };
                        runtimeData.animationBakeTexture.Create();
                    }
                }

                if (prototype.animationData.useCrowdAnimator)
                {
                    if (runtimeData.crowdAnimatorControllerBuffer == null || runtimeData.crowdAnimatorControllerBuffer.count != runtimeData.bufferSize * 4)
                    {
                        runtimeData.crowdAnimatorControllerData = GPUInstancerUtility.ResizeNativeArray(runtimeData.crowdAnimatorControllerData, runtimeData.bufferSize * 4, Allocator.Persistent);

                        if (runtimeData.crowdAnimatorControllerBuffer != null)
                            runtimeData.crowdAnimatorControllerBuffer.Release();
                        runtimeData.crowdAnimatorControllerBuffer = new ComputeBuffer(runtimeData.bufferSize * 4, GPUInstancerConstants.STRIDE_SIZE_FLOAT4);
                        runtimeData.crowdAnimatorControllerBuffer.SetData(runtimeData.crowdAnimatorControllerData);
                    }

                    if (prototype.animationData.applyBoneUpdates && prototype.animationData.exposedBoneIndexes != null && prototype.animationData.exposedBoneIndexes.Length > 0)
                    {
                        runtimeData.boneTransformAccessArray = GPUInstancerUtility.ResizeTransformAccessArray(runtimeData.boneTransformAccessArray, prototype.animationData.exposedBoneIndexes.Length * runtimeData.bufferSize);

                        if (runtimeData.bindPoses.IsCreated)
                            runtimeData.bindPoses.Dispose();
                        runtimeData.bindPoses = new NativeArray<Matrix4x4>(prototype.animationData.bindPoses, Allocator.Persistent);

                        if (runtimeData.boneUpdateFilter.IsCreated)
                            runtimeData.boneUpdateFilter.Dispose();
                        runtimeData.boneUpdateFilter = new NativeArray<int>(prototype.animationData.exposedBoneIndexes, Allocator.Persistent);

                        if (prototype.animationData.isSynchronousBoneUpdates)
                        {
                            if (!runtimeData.bakedAnimationData.IsCreated)
                            {
                                runtimeData.bakedAnimationData = new NativeArray<Matrix4x4>(prototype.animationData.exposedBoneIndexes.Length * prototype.animationData.totalFrameCount, Allocator.Persistent);
                                ReadBakedBoneDataFromTexture(prototype, runtimeData);
                            }
                        }
                        else
                        {
                            if (runtimeData.asyncBoneUpdateFilterBuffer != null)
                                runtimeData.asyncBoneUpdateFilterBuffer.Release();
                            runtimeData.asyncBoneUpdateFilterBuffer = new ComputeBuffer(prototype.animationData.exposedBoneIndexes.Length, GPUInstancerConstants.STRIDE_SIZE_INT);
                            runtimeData.asyncBoneUpdateFilterBuffer.SetData(prototype.animationData.exposedBoneIndexes);

                            if (runtimeData.asyncBoneUpdateDataBuffer != null)
                                runtimeData.asyncBoneUpdateDataBuffer.Release();
                            runtimeData.asyncBoneUpdateDataBuffer = new ComputeBuffer(prototype.animationData.exposedBoneIndexes.Length * runtimeData.bufferSize, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);
                        }
                    }
                    else
                        prototype.animationData.applyBoneUpdates = false;

                    if (prototype.animationData.applyRootMotion)
                    {
                        if (runtimeData.clipDatas.IsCreated)
                            runtimeData.clipDatas.Dispose();
                        runtimeData.clipDatas = new NativeArray<GPUIAnimationClipData>(prototype.animationData.clipDataList.ToArray(), Allocator.Persistent);

                        if (runtimeData.rootMotions.IsCreated)
                            runtimeData.rootMotions.Dispose();
                        runtimeData.rootMotions = new NativeArray<GPUIRootMotion>(prototype.animationData.rootMotions, Allocator.Persistent);
                    }
                }

                runtimeData.animationClipDataDict = new Dictionary<int, GPUIAnimationClipData>();
                for (int i = 0; i < prototype.animationData.clipDataList.Count; i++)
                    runtimeData.animationClipDataDict.Add(prototype.clipList[i].GetHashCode(), prototype.animationData.clipDataList[i]);
                runtimeData.animatorStateDict = new Dictionary<int, GPUIAnimatorState>();
                foreach (GPUIAnimatorState state in prototype.animationData.states)
                    runtimeData.animatorStateDict.Add(state.hashCode, state);
            }
        }

        public static void SetAppendBuffers(GPUICrowdRuntimeData runtimeData)
        {
            GPUICrowdPrototype prototype = runtimeData.prototype as GPUICrowdPrototype;

            if (prototype.animationData == null || prototype.animationData.animationTexture == null)
            {
                Debug.LogError("Prototype has no baked animations: " + prototype.prefabObject.name);
                return;
            }

            foreach (GPUInstancerPrototypeLOD gpuiLod in runtimeData.instanceLODs)
            {
                foreach (GPUInstancerRenderer renderer in gpuiLod.renderers)
                {
                    GPUISkinnedMeshData smd = prototype.animationData.GetSkinnedMeshDataByName(renderer.rendererRefName);
                    if (prototype.animationData != null && prototype.animationData.animationTexture != null)
                    {
                        if (GPUInstancerUtility.matrixHandlingType == GPUIMatrixHandlingType.CopyToTexture)
                            renderer.mpb.SetTexture(GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER_TEXTURE, runtimeData.animationBakeTexture);
                        else
                            renderer.mpb.SetBuffer(GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER, runtimeData.animationBakeBuffer);
                        renderer.mpb.SetFloat(GPUICrowdConstants.CrowdKernelPoperties.TOTAL_NUMBER_OF_BONES, prototype.animationData.totalBoneCount);
                        if (smd.hasBindPoseOffset)
                        {
                            for (int i = 0; i < renderer.materials.Count; i++)
                            {
                                renderer.materials[i].EnableKeyword(GPUICrowdConstants.KEYWORD_GPUI_CA_BINDPOSEOFFSET);
                            }
                            renderer.mpb.SetMatrix(GPUICrowdConstants.CrowdKernelPoperties.BINDPOSE_OFFSET, smd.bindPoseOffset);
                        }
                    }
                    if (runtimeData.hasShadowCasterBuffer)
                    {
                        if (prototype.animationData != null && prototype.animationData.animationTexture != null)
                        {
                            if (GPUInstancerUtility.matrixHandlingType == GPUIMatrixHandlingType.CopyToTexture)
                                renderer.shadowMPB.SetTexture(GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER_TEXTURE, runtimeData.animationBakeTexture);
                            else
                                renderer.shadowMPB.SetBuffer(GPUICrowdConstants.CrowdKernelPoperties.ANIMATION_BUFFER, runtimeData.animationBakeBuffer);
                            renderer.shadowMPB.SetFloat(GPUICrowdConstants.CrowdKernelPoperties.TOTAL_NUMBER_OF_BONES, prototype.animationData.totalBoneCount);
                            if (smd.hasBindPoseOffset)
                            {
                                renderer.shadowMPB.SetMatrix(GPUICrowdConstants.CrowdKernelPoperties.BINDPOSE_OFFSET, smd.bindPoseOffset);
                            }
                        }
                    }
                }
            }
        }

        public static void SetMeshUVs(GPUICrowdRuntimeData runtimeData)
        {
            GPUICrowdPrototype prototype = runtimeData.prototype as GPUICrowdPrototype;
            if (prototype.animationData == null || prototype.animationData.skinnedMeshDataList == null || runtimeData.isUVsSet)
                return;
            foreach (GPUInstancerPrototypeLOD gpuiLod in runtimeData.instanceLODs)
            {
                for (int i = 0; i < gpuiLod.renderers.Count; i++)
                {
                    if (prototype.animationData.skinnedMeshDataList.Count <= i)
                        break;
                    GPUInstancerRenderer renderer = gpuiLod.renderers[i];
                    GPUISkinnedMeshData smd = prototype.animationData.GetSkinnedMeshDataByName(renderer.rendererRefName);
                    renderer.mesh = GenerateMeshWithUVs(renderer.mesh, smd);
                }
            }
            runtimeData.isUVsSet = true;
        }

        public static Mesh GenerateMeshWithUVs(Mesh originalMesh, GPUISkinnedMeshData smd)
        {
            List<Vector4> boneIndexes = new List<Vector4>();
            List<Vector4> boneWeights = new List<Vector4>();
            Vector4 boneIndexVector = Vector4.zero;
            Vector4 boneWeightVector = Vector4.zero;
            foreach (BoneWeight boneWeight in originalMesh.boneWeights)
            {
                boneIndexVector.x = smd.boneIndexes[boneWeight.boneIndex0];
                boneIndexVector.y = smd.boneIndexes[boneWeight.boneIndex1];
                boneIndexVector.z = smd.boneIndexes[boneWeight.boneIndex2];
                boneIndexVector.w = smd.boneIndexes[boneWeight.boneIndex3];
                boneIndexes.Add(boneIndexVector);

                boneWeightVector.x = boneWeight.weight0;
                boneWeightVector.y = boneWeight.weight1;
                boneWeightVector.z = boneWeight.weight2;
                boneWeightVector.w = boneWeight.weight3;
                boneWeights.Add(boneWeightVector);
            }
            Mesh mesh = new Mesh();
            mesh.name = originalMesh.name + "_GPUI_CA";
            mesh.subMeshCount = originalMesh.subMeshCount;
            mesh.vertices = originalMesh.vertices;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                mesh.SetTriangles(originalMesh.GetTriangles(i), i);
            }

            mesh.normals = originalMesh.normals;
            mesh.tangents = originalMesh.tangents;
            mesh.colors = originalMesh.colors;
            mesh.uv = originalMesh.uv;
            mesh.uv2 = originalMesh.uv2;
            mesh.SetUVs(2, boneIndexes);
            mesh.SetUVs(3, boneWeights);

            return mesh;
        }

        public static void ReadBakedBoneDataFromTexture(GPUICrowdPrototype prototype, GPUICrowdRuntimeData runtimeData)
        {
            Color[] colors = prototype.animationData.animationTexture.GetPixels(0);

            int boneCount = prototype.animationData.exposedBoneIndexes.Length;
            int textureSizeX = prototype.animationData.textureSizeX;
            for (int b = 0; b < boneCount; b++)
            {
                int boneIndex = prototype.animationData.exposedBoneIndexes[b];
                for (int f = 0; f < prototype.animationData.totalFrameCount; f++)
                {
                    int textureIndex = f * prototype.animationData.totalBoneCount + boneIndex;
                    int texIndexX = textureIndex % textureSizeX;
                    int texIndexY = Mathf.FloorToInt(textureIndex / (float)textureSizeX) * 4;

                    int colorIndex = texIndexX + texIndexY * textureSizeX;

                    Matrix4x4 matrix4X4 = new Matrix4x4();
                    matrix4X4.SetColumn(0, colors[colorIndex]);
                    colorIndex += textureSizeX;
                    matrix4X4.SetColumn(1, colors[colorIndex]);
                    colorIndex += textureSizeX;
                    matrix4X4.SetColumn(2, colors[colorIndex]);
                    colorIndex += textureSizeX;
                    matrix4X4.SetColumn(3, colors[colorIndex]);

                    runtimeData.bakedAnimationData[f * boneCount + b] = matrix4X4;
                }
            }
        }
        #endregion Set Animation Data

        #region Create Crowd Prototypes
        public static void SetCrowdPrefabPrototypes(GameObject gameObject, List<GPUInstancerPrototype> prototypeList, List<GameObject> prefabList, bool forceNew)
        {
            if (prefabList == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                Undo.RecordObject(gameObject, "Prefab prototypes changed");

            bool changed = false;
            if (forceNew)
            {
                foreach (GPUICrowdPrototype prototype in prototypeList)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prototype));
                    changed = true;
                }
            }
            else
            {
                foreach (GPUICrowdPrototype prototype in prototypeList)
                {
                    if (!prefabList.Contains(prototype.prefabObject))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prototype));
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif

            foreach (GameObject go in prefabList)
            {
                if (!forceNew && prototypeList.Exists(p => p.prefabObject == go))
                    continue;

                prototypeList.Add(GenerateCrowdPrototype(go, forceNew));
            }

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (!Application.isPlaying)
            {
                GPUICrowdPrefab[] prefabInstances = GameObject.FindObjectsOfType<GPUICrowdPrefab>();
                for (int i = 0; i < prefabInstances.Length; i++)
                {
#if UNITY_2018_2_OR_NEWER
                    UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstances[i].gameObject);
#else
                    UnityEngine.Object prefabRoot = PrefabUtility.GetPrefabParent(prefabInstances[i].gameObject);
#endif
                    if (prefabRoot != null && ((GameObject)prefabRoot).GetComponent<GPUICrowdPrefab>() != null && prefabInstances[i].prefabPrototype != ((GameObject)prefabRoot).GetComponent<GPUICrowdPrefab>().prefabPrototype)
                    {
                        Undo.RecordObject(prefabInstances[i], "Changed GPUInstancer Prefab Prototype " + prefabInstances[i].gameObject + i);
                        prefabInstances[i].prefabPrototype = ((GameObject)prefabRoot).GetComponent<GPUICrowdPrefab>().prefabPrototype;
                    }
                }
            }
#endif
        }

        public static GPUICrowdPrototype GenerateCrowdPrototype(GameObject go, bool forceNew, bool attachScript = true, GPUICrowdPrototype copySettingsFrom = null)
        {
            GPUICrowdPrefab prefabScript = go.GetComponent<GPUICrowdPrefab>();
            if (attachScript && prefabScript == null)
#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
                prefabScript = GPUInstancerUtility.AddComponentToPrefab<GPUICrowdPrefab>(go);
#else
                prefabScript = go.AddComponent<GPUICrowdPrefab>();
#endif
            if (attachScript && prefabScript == null)
                return null;

            GPUICrowdPrototype prototype = null;

            if (prefabScript != null)
                prototype = (GPUICrowdPrototype)prefabScript.prefabPrototype;
            if (prototype == null)
            {
                prototype = copySettingsFrom != null ? ScriptableObject.Instantiate(copySettingsFrom) : ScriptableObject.CreateInstance<GPUICrowdPrototype>();
                if (prefabScript != null)
                    prefabScript.prefabPrototype = prototype;
                prototype.prefabObject = go;
                prototype.name = go.name + "_" + go.GetInstanceID();
                prototype.useOriginalShaderForShadow = true;
                if (go.GetComponent<Rigidbody>() != null)
                {
                    prototype.enableRuntimeModifications = true;
                    prototype.autoUpdateTransformData = true;
                }
                prototype.hasNoAnimator = go.GetComponent<Animator>() == null;

                // if SRP use original shader for shadow
                if (!prototype.useOriginalShaderForShadow)
                {
                    MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer rdr in renderers)
                    {
                        foreach (Material mat in rdr.sharedMaterials)
                        {
                            if (mat.shader.name.Contains("HDRenderPipeline") || mat.shader.name.Contains("LWRenderPipeline") || mat.shader.name.Contains("Lightweight Render Pipeline"))
                            {
                                prototype.useOriginalShaderForShadow = true;
                                break;
                            }
                        }
                        if (prototype.useOriginalShaderForShadow)
                            break;
                    }
                }

                //GPUInstancerUtility.GenerateInstancedShadersForGameObject(prototype, gpuiSettings);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    EditorUtility.SetDirty(go);
#endif
            }
#if UNITY_EDITOR
            if (!Application.isPlaying && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(prototype)))
            {
                string assetPath = GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_CROWD_PATH + prototype.name + ".asset";

                if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_CROWD_PATH))
                {
                    System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_CROWD_PATH);
                }

                AssetDatabase.CreateAsset(prototype, assetPath);
            }

#if UNITY_2018_3_OR_NEWER
            if (!Application.isPlaying && prefabScript != null && prefabScript.prefabPrototype != prototype)
            {
                GameObject prefabContents = GPUInstancerUtility.LoadPrefabContents(go);
                prefabContents.GetComponent<GPUICrowdPrefab>().prefabPrototype = prototype;
                GPUInstancerUtility.UnloadPrefabContents(go, prefabContents);
            }
#endif
#endif
            return prototype;
        }

        public static GPUICrowdAnimationData GenerateCrowdAnimationData(GPUICrowdPrototype prototype)
        {
            if (prototype.animationData == null)
                prototype.animationData = ScriptableObject.CreateInstance<GPUICrowdAnimationData>();

#if UNITY_EDITOR
            if (!Application.isPlaying && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(prototype.animationData)))
            {
                string assetPath = GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_CROWD__ANIMATION_DATA_PATH + prototype.name + "_AnimationData.asset";

                if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_CROWD__ANIMATION_DATA_PATH))
                {
                    System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_CROWD__ANIMATION_DATA_PATH);
                }

                AssetDatabase.CreateAsset(prototype.animationData, assetPath);
            }
#endif
            return prototype.animationData;
        }
        #endregion Create Crowd Prototypes

        #region Shader Functions
        public static bool IsShaderInstanced(Shader shader)
        {
#if UNITY_EDITOR
            string originalAssetPath = AssetDatabase.GetAssetPath(shader);
            string originalShaderText = "";
            try
            {
                originalShaderText = System.IO.File.ReadAllText(originalAssetPath);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(originalShaderText))
                return originalShaderText.Contains("GPUICrowdInclude.cginc") || originalShaderText.Contains("GPUI Crowd Setup") || originalShaderText.Contains("GPUICrowdSkinning.cginc");
#endif
            return false;
        }

        public static void GenerateInstancedShadersForGameObject(GPUInstancerPrototype prototype)
        {
            if (prototype.prefabObject == null)
                return;

            SkinnedMeshRenderer[] skinnedMeshRenderers = prototype.prefabObject.GetComponentsInChildren<SkinnedMeshRenderer>();

#if UNITY_EDITOR
            string warnings = "";
#endif

            foreach (SkinnedMeshRenderer mr in skinnedMeshRenderers)
            {
                Material[] mats = mr.sharedMaterials;

                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null || mats[i].shader == null)
                        continue;
                    if (GPUInstancerConstants.gpuiSettings.shaderBindings.IsShadersInstancedVersionExists(mats[i].shader.name, GPUICrowdConstants.GPUI_EXTENSION_CODE))
                    {
                        GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(mats[i], GPUICrowdConstants.GPUI_EXTENSION_CODE);
                        continue;
                    }

                    if (!Application.isPlaying)
                    {
                        if (IsShaderInstanced(mats[i].shader))
                        {
                            GPUInstancerConstants.gpuiSettings.shaderBindings.AddShaderInstance(mats[i].shader.name, mats[i].shader, true, GPUICrowdConstants.GPUI_EXTENSION_CODE);
                            GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(mats[i], GPUICrowdConstants.GPUI_EXTENSION_CODE);
                        }
#if UNITY_EDITOR
                        else
                        {
                            if (!warnings.Contains(mats[i].shader.name))
                                warnings += "Can not find Crowd Animations setup for shader: " + mats[i].shader.name + ". Please add Crowd Animations setup to the shader by following the Shader Setup Documentation:";
                        }
#endif
                    }
                }
            }

            if (prototype.useGeneratedBillboard && prototype.billboard != null)
            {
                GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(GPUInstancerUtility.GetBillboardShaderName(prototype), GPUICrowdConstants.GPUI_EXTENSION_CODE);
            }

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(warnings))
            {
                if (prototype.warningText != null)
                {
                    prototype.warningText = null;
                    EditorUtility.SetDirty(prototype);
                }
            }
            else
            {
                if (prototype.warningText != warnings)
                {
                    prototype.warningText = warnings;
                    EditorUtility.SetDirty(prototype);
                }
            }
#endif
        }
        #endregion

        #region Transform Extensions
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Transform result = aParent.Find(aName);
            if (result != null)
                return result;
            int childCount = aParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                result = aParent.GetChild(i).FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
        #endregion Transform Extensions
    }
}
#endif //GPU_INSTANCER