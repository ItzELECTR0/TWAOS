#if GPU_INSTANCER
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdAnimationBaker
    {
        private bool _isBakingAnimation;
        private GPUIAnimationBakeData _animationBakeData;
        private ComputeShader _skinnedMeshBakeComputeShader;
        private int _animationToTextureKernelID;

        private AnimatorController _bakerAnimatorController;
        private AnimationClip _bakerAnimationClip;

        class GPUIAnimationBakeData
        {
            public GPUICrowdPrototype crowdPrototype;
            public int[] textureSize;
            public RenderTexture animationRenderTexture;

            public GameObject sampleObject;
            public Animator sampleAnimator;
            public SkinnedMeshRenderer[] sampleSkinnedMeshRenderers;
            public int currentClipIndex;
            public int currentClipFrame;
            public Matrix4x4[] boneMatrices;

            public AnimatorOverrideController animatorOverrideController;

            public bool hasTransformHierarchy;
            public ModelImporter modelImporter;
            public bool optimizeGameObjects;
            public string[] extraExposedTransformPaths;

            public bool isBakeInitialized;
            public List<Transform> boneTransforms;
            public List<Matrix4x4> bindPoses;
            public List<int> bindPosesBoneCount;
        }

        private Queue<GPUInstancerPrototype> _bakeQueue;

        #region SkinnedMesh Bake Methods
        public void SkinnedMeshBakeAnimations(List<GPUInstancerPrototype> crowdPrototypes)
        {
            _bakeQueue = new Queue<GPUInstancerPrototype>(crowdPrototypes);
            SkinnedMeshBakeAnimations((GPUICrowdPrototype)_bakeQueue.Dequeue());
        }

        public void SkinnedMeshBakeAnimations(GPUICrowdPrototype crowdPrototype)
        {
            GPUInstancerUtility.SetPlatformDependentVariables();
            if (crowdPrototype.prefabObject == null || crowdPrototype.prefabObject.GetComponentInChildren<SkinnedMeshRenderer>() == null)
                return;
            _animationBakeData = new GPUIAnimationBakeData();
            _animationBakeData.crowdPrototype = crowdPrototype;

            GPUICrowdPrefab gpuiPrefab = crowdPrototype.prefabObject.GetComponent<GPUICrowdPrefab>();
            Animator prefabAnimator = gpuiPrefab.GetAnimator();

            if (prefabAnimator != null)
                _animationBakeData.hasTransformHierarchy = prefabAnimator.hasTransformHierarchy;
            else
                _animationBakeData.hasTransformHierarchy = true;

            if (!_animationBakeData.hasTransformHierarchy)
            {
                _animationBakeData.modelImporter = (ModelImporter)AssetImporter.GetAtPath(_animationBakeData.crowdPrototype.modelPrefabPath);
                _animationBakeData.optimizeGameObjects = _animationBakeData.modelImporter.optimizeGameObjects;
                _animationBakeData.extraExposedTransformPaths = _animationBakeData.modelImporter.extraExposedTransformPaths;

                if (_animationBakeData.modelImporter.optimizeGameObjects)
                {
                    _animationBakeData.modelImporter.optimizeGameObjects = false;
                    _animationBakeData.modelImporter.SaveAndReimport();
                }
            }
            EditorApplication.update += AnimationBakerInitialize;
        }

        public void AnimationBakerInitialize()
        {
            EditorApplication.update -= AnimationBakerInitialize;

            try
            {
                if (_animationBakeData.crowdPrototype.animationData == null)
                    GPUICrowdUtility.GenerateCrowdAnimationData(_animationBakeData.crowdPrototype);
                _animationBakeData.crowdPrototype.animationData.bakedCAVersion = GPUICrowdEditorConstants.GPUI_CA_VERSION_NO;

                _bakerAnimatorController = Resources.Load<AnimatorController>("Editor/BakerAnimator");
                _bakerAnimationClip = Resources.Load<AnimationClip>("Editor/BakerAnimation");

                _animationBakeData.sampleObject = GetSampleGameObject(_animationBakeData.crowdPrototype, !_animationBakeData.hasTransformHierarchy);
                _animationBakeData.sampleObject.transform.localScale = Vector3.one;
                _animationBakeData.sampleObject.name += "(Baker)";
                _animationBakeData.sampleObject.hideFlags = HideFlags.DontSave;

                _animationBakeData.sampleObject.transform.position = Vector3.zero;
                _animationBakeData.sampleObject.transform.rotation = Quaternion.identity;
                _animationBakeData.sampleSkinnedMeshRenderers = _animationBakeData.sampleObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                _animationBakeData.sampleAnimator = _animationBakeData.crowdPrototype.hasNoAnimator ?
                    _animationBakeData.sampleObject.AddComponent<Animator>()
                    : _animationBakeData.sampleObject.GetComponent<Animator>();
                _animationBakeData.sampleAnimator.runtimeAnimatorController = _animationBakeData.crowdPrototype.hasNoAnimator ?
                    _bakerAnimatorController
                    : _animationBakeData.crowdPrototype.prefabObject.GetComponent<Animator>().runtimeAnimatorController;
                _animationBakeData.sampleAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                //_animationBakeData.crowdPrototype.animationData.applyRootMotion = _animationBakeData.sampleAnimator.applyRootMotion;
                _animationBakeData.sampleAnimator.applyRootMotion = true;

                _animationBakeData.isBakeInitialized = false;
                _animationBakeData.boneTransforms = new List<Transform>();
                _animationBakeData.bindPoses = new List<Matrix4x4>();
                _animationBakeData.bindPosesBoneCount = new List<int>();

                if (_animationBakeData.crowdPrototype.hasNoAnimator && (_animationBakeData.crowdPrototype.clipList == null || _animationBakeData.crowdPrototype.clipList.Count == 0))
                    throw new Exception("No animation clips found. Please add animation clips to the Clip List.");

            }
            catch (Exception e)
            {
                ClearBakingData();
                Debug.LogError(e);
                return;
            }

            AnimationBakerStart();
        }

        public void AnimationBakerStart()
        {
            try
            {
                if (EditorUtility.DisplayCancelableProgressBar("GPU Instancer Animation Baker", "Baking animations for GPUI...", 0.1f))
                {
                    ClearBakingData();
                    return;
                }

                if (_animationBakeData.sampleAnimator.runtimeAnimatorController == null)
                    throw new Exception("Can not find Animator Controller on prefab:" + _animationBakeData.crowdPrototype.prefabObject.name);

                GPUICrowdAnimationData animationData = _animationBakeData.crowdPrototype.animationData;
                if (_animationBakeData.crowdPrototype.clipList == null) 
                    _animationBakeData.crowdPrototype.clipList = new List<AnimationClip>();
                _animationBakeData.crowdPrototype.clipList.RemoveAll(c => c == null);
                List<AnimationClip> previousClipList = _animationBakeData.crowdPrototype.clipList;
                if (!_animationBakeData.crowdPrototype.hasNoAnimator)
                {
                    _animationBakeData.crowdPrototype.clipList.Clear();
                    AnimationClip[] animationClips = _animationBakeData.sampleAnimator.runtimeAnimatorController.animationClips;
                    for (int i = 0; i < animationClips.Length; i++)
                    {
                        if (!_animationBakeData.crowdPrototype.clipList.Contains(animationClips[i]))
                            _animationBakeData.crowdPrototype.clipList.Add(animationClips[i]);
                    }
                }
                
                AnimatorController animatorController = null;

                if (_animationBakeData.sampleAnimator.runtimeAnimatorController is AnimatorOverrideController)
                    animatorController = (AnimatorController)((AnimatorOverrideController)_animationBakeData.sampleAnimator.runtimeAnimatorController).runtimeAnimatorController;
                else
                    animatorController = (AnimatorController)_animationBakeData.sampleAnimator.runtimeAnimatorController;

                animationData.states = new List<GPUIAnimatorState>();
                AddChildStatesFromStateMachine(animationData, animatorController.layers[0].stateMachine, animatorController.layers[0].name);

                animationData.totalFrameCount = 0;

                #region Collect Skinned Mesh Renderer Data
                List<string> optionalSkinnedMeshes = new List<string>();
                if (animationData.skinnedMeshDataList == null)
                    animationData.skinnedMeshDataList = new List<GPUISkinnedMeshData>();
                else
                {
                    foreach (GPUISkinnedMeshData smd in animationData.skinnedMeshDataList)
                    {
                        if (smd.isOptional)
                            optionalSkinnedMeshes.Add(smd.transformName);
                    }
                    animationData.skinnedMeshDataList.Clear();
                }

                List<string> dontDestroyBoneNames = new List<string>();
                if (animationData.bones == null)
                    animationData.bones = new List<GPUIBone>();
                else
                {
                    foreach (GPUIBone bone in animationData.bones)
                    {
                        if (bone.dontDestroy)
                            dontDestroyBoneNames.Add(bone.boneTransformName);
                    }
                    animationData.bones.Clear();
                }

                // collect bone data from SkinnedMeshRenderers
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in _animationBakeData.sampleSkinnedMeshRenderers)
                {
                    if (skinnedMeshRenderer.sharedMesh == null)
                    {
                        throw new Exception("Mesh reference is not set for SkinnedMeshRenderer: " + skinnedMeshRenderer.gameObject.name);
                    }

                    GPUISkinnedMeshData smd = new GPUISkinnedMeshData
                    {
                        transformName = skinnedMeshRenderer.gameObject.name,
                        boneIndexes = new int[skinnedMeshRenderer.bones.Length],
                        hasBindPoseOffset = false,
                        bindPoseOffset = Matrix4x4.identity,
                        isOptional = optionalSkinnedMeshes.Contains(skinnedMeshRenderer.gameObject.name)
                    };

                    for (int b = 0; b < skinnedMeshRenderer.bones.Length; b++)
                    {
                        Transform boneTransform = skinnedMeshRenderer.bones[b];
                        GPUIBone bone = animationData.GetBoneByTransform(boneTransform.name);
                        if (bone == null)
                        {
                            bone = new GPUIBone
                            {
                                boneTransformName = boneTransform.name,
                                boneIndex = animationData.bones.Count,
                                dontDestroy = dontDestroyBoneNames.Contains(boneTransform.name)
                            };
                            animationData.bones.Add(bone);
                            _animationBakeData.boneTransforms.Add(boneTransform);
                            _animationBakeData.bindPoses.Add(skinnedMeshRenderer.sharedMesh.bindposes[b]);
                            _animationBakeData.bindPosesBoneCount.Add(skinnedMeshRenderer.bones.Length);
                        }
                        smd.boneIndexes[b] = bone.boneIndex;
                        if (boneTransform == skinnedMeshRenderer.rootBone)
                            smd.rootBoneIndex = bone.boneIndex;
                        if (_animationBakeData.bindPoses[bone.boneIndex] != skinnedMeshRenderer.sharedMesh.bindposes[b])
                        {
                            if (_animationBakeData.bindPosesBoneCount[bone.boneIndex] < skinnedMeshRenderer.bones.Length)
                            {
                                _animationBakeData.bindPoses[bone.boneIndex] = skinnedMeshRenderer.sharedMesh.bindposes[b];
                                _animationBakeData.bindPosesBoneCount[bone.boneIndex] = skinnedMeshRenderer.bones.Length;
                            }
                        }
                    }

                    animationData.skinnedMeshDataList.Add(smd);
                }

                animationData.bindPoses = _animationBakeData.bindPoses.ToArray();

                foreach (SkinnedMeshRenderer skinnedMeshRenderer in _animationBakeData.sampleSkinnedMeshRenderers)
                {
                    GPUISkinnedMeshData smd = animationData.GetSkinnedMeshDataByName(skinnedMeshRenderer.gameObject.name);
                    int indx = animationData.GetBoneIndexByTransform(skinnedMeshRenderer.bones[0].name);
                    if (_animationBakeData.bindPoses[indx] != skinnedMeshRenderer.sharedMesh.bindposes[0])
                    {
                        smd.hasBindPoseOffset = true;
                        smd.bindPoseOffset = _animationBakeData.bindPoses[indx].inverse * skinnedMeshRenderer.sharedMesh.bindposes[0];
                    }
                }

                // Set bone parent-child relations
                int smdIndex = 0;
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in _animationBakeData.sampleSkinnedMeshRenderers)
                {
                    GPUISkinnedMeshData smd = animationData.skinnedMeshDataList[smdIndex];
                    for (int i = 0; i < smd.boneIndexes.Length; i++)
                    {
                        GPUIBone bone = animationData.GetBoneByTransform(skinnedMeshRenderer.bones[i].name);

                        Transform parentTransform = skinnedMeshRenderer.bones[i].parent;
                        bool foundParent = false;

                        while (parentTransform != null)
                        {
                            GPUIBone parentBone = animationData.GetBoneByTransform(parentTransform.name);
                            if (parentBone != null)
                            {
                                foundParent = true;
                                animationData.BoneSetParent(bone, parentBone);
                                break;
                            }
                            parentTransform = parentTransform.parent;
                        }
                        if (!foundParent)
                        {
                            bone.isRoot = true;
                        }
                    }
                    smdIndex++;
                }
                
                animationData.totalBoneCount = animationData.bones.Count;

                if (animationData.totalBoneCount == 0)
                {
                    Debug.LogError("GPUI can not find any bone data for: " + _animationBakeData.crowdPrototype.prefabObject.name);
                    ClearBakingData();
                    return;
                }
                #endregion Collect Skinned Mesh Renderer Data

                #region Collect Animation Clip Data
                List<GPUIAnimationClipData> previousClipDataList = animationData.clipDataList;
                animationData.clipDataList = new List<GPUIAnimationClipData>();

                foreach (AnimationClip animationClip in _animationBakeData.crowdPrototype.clipList)
                {
                    if (animationData.clipDataList.Exists(cd => cd.GetHashCode() == animationClip.GetHashCode()))
                        continue;

                    int frameCount = Mathf.CeilToInt(animationClip.length * _animationBakeData.crowdPrototype.frameRate + 1);

                    int isLoopDisabled = animationClip.isLooping ? 0 : 1;

                    if (previousClipDataList != null && previousClipList != null)
                    {
                        int previousClipIndex = previousClipList.IndexOf(animationClip);
                        if (previousClipIndex >= 0 && previousClipDataList.Count > previousClipIndex)
                            isLoopDisabled = previousClipDataList[previousClipIndex].isLoopDisabled;
                    }

                    GPUIAnimationClipData acd = new GPUIAnimationClipData
                    {
                        clipIndex = animationData.clipDataList.Count,
                        clipStartFrame = animationData.totalFrameCount,
                        clipFrameCount = frameCount,
                        hasRootMotion = 0,
                        length = (frameCount - 1.0f) / _animationBakeData.crowdPrototype.frameRate,
                        isLoopDisabled = isLoopDisabled,
                        isClipLooping = animationClip.isLooping ? 1 : 0
                    };

                    animationData.totalFrameCount += acd.clipFrameCount;
                    animationData.clipDataList.Add(acd);
                }
                #endregion Collect Animation Clip Data

                #region Calculate Texture Size
                int totalData = animationData.totalBoneCount * animationData.totalFrameCount * 4;
                animationData.textureSizeX =
                    Mathf.NextPowerOfTwo(Mathf.CeilToInt(Mathf.Sqrt(totalData)));
                animationData.textureSizeY =
                    Mathf.NextPowerOfTwo(Mathf.CeilToInt(totalData / (float)animationData.textureSizeX));
                _animationBakeData.textureSize = new int[2] { animationData.textureSizeX, animationData.textureSizeY };

                if (animationData.textureSizeX > 8192 || animationData.textureSizeY > 8192)
                {
                    throw new Exception("Bone and animation frame amount is too high.");
                }
                #endregion Calculate Texture Size

                #region Create Render Texture
                _animationBakeData.animationRenderTexture = new RenderTexture(animationData.textureSizeX, animationData.textureSizeY, 0, RenderTextureFormat.ARGBHalf)
                {
                    filterMode = FilterMode.Point,
                    enableRandomWrite = true,
                    useMipMap = false,
                    autoGenerateMips = false
                };
                _animationBakeData.animationRenderTexture.Create();
                #endregion Crete Render Texture

                _isBakingAnimation = true;
                _animationBakeData.currentClipIndex = 0;
                _animationBakeData.currentClipFrame = 0;
                _animationBakeData.boneMatrices = new Matrix4x4[animationData.totalBoneCount * animationData.totalFrameCount];
                animationData.rootMotions = new GPUIRootMotion[animationData.totalFrameCount];

                _animationBakeData.sampleAnimator.runtimeAnimatorController = _bakerAnimatorController;

                EditorApplication.update += AnimationBakerUpdate;
            }
            catch (Exception e)
            {
                ClearBakingData();
                Debug.LogError(e);
            }
        }

        public void OverrideAnimator(AnimationClip animationClip)
        {
            if (_animationBakeData.animatorOverrideController == null)
                _animationBakeData.animatorOverrideController = new AnimatorOverrideController(_animationBakeData.sampleAnimator.runtimeAnimatorController);

            _animationBakeData.animatorOverrideController[_bakerAnimationClip] = animationClip;

            _animationBakeData.sampleAnimator.runtimeAnimatorController = _animationBakeData.animatorOverrideController;
        }

        public void AnimationBakerUpdate()
        {
            if (!_isBakingAnimation)
            {
                ClearBakingData();
                return;
            }

            try
            {
                GPUICrowdAnimationData animationData = _animationBakeData.crowdPrototype.animationData;

                if (animationData.clipDataList.Count == 0)
                {
                    throw new Exception("No animation clips found.");
                }
                do
                {
                    GPUIAnimationClipData animationClipData = animationData.clipDataList[_animationBakeData.currentClipIndex];

                    if (EditorUtility.DisplayCancelableProgressBar("GPU Instancer Animation Baker", "Frame No: " + _animationBakeData.currentClipFrame + "/" + animationClipData.clipFrameCount
                        + " Clip: " + (_animationBakeData.currentClipIndex + 1) + "/" + _animationBakeData.crowdPrototype.animationData.clipDataList.Count + " (" + _animationBakeData.crowdPrototype.clipList[animationClipData.clipIndex].name + ")",
                        0.1f + ((_animationBakeData.currentClipFrame + animationClipData.clipStartFrame) / (float)_animationBakeData.crowdPrototype.animationData.totalFrameCount) * 0.8f))
                    {
                        ClearBakingData();
                        return;
                    }

                    #region Initialize Sample Object
                    if (!_animationBakeData.isBakeInitialized)
                    {
                        _animationBakeData.isBakeInitialized = true;

                        OverrideAnimator(_animationBakeData.crowdPrototype.clipList[animationClipData.clipIndex]);
                        _animationBakeData.sampleAnimator.Update(0);
                        //animationClipData.animationClip.SampleAnimation(_animationBakeData.sampleObject, 0);
                        return;
                    }
                    #endregion Initialize Sample Object

                    Matrix4x4 motionMatrix = _animationBakeData.sampleObject.transform.localToWorldMatrix;
                    GPUIRootMotion rootMotion = new GPUIRootMotion()
                    {
                        hasMotion = motionMatrix != Matrix4x4.identity ? 1 : 0,
                        isPositionOnly = _animationBakeData.sampleObject.transform.rotation == Quaternion.identity ? 1 : 0,
                        motionMatrix = motionMatrix
                    };
                    animationData.rootMotions[_animationBakeData.currentClipFrame + animationClipData.clipStartFrame] = rootMotion;
                    if (rootMotion.HasMotion() && !animationClipData.HasRootMotion())
                    {
                        animationClipData.hasRootMotion = 1;
                        animationData.clipDataList[_animationBakeData.currentClipIndex] = animationClipData;
                    }
                    _animationBakeData.sampleObject.transform.position = Vector3.zero;
                    _animationBakeData.sampleObject.transform.rotation = Quaternion.identity;

                    foreach (Transform boneTransform in _animationBakeData.boneTransforms)
                    {
                        int boneIndex = animationData.GetBoneIndexByTransform(boneTransform.name);

                        // ordered by frames (all bone data of first frame then all bone data of second frame)
                        int index = (_animationBakeData.currentClipFrame + animationClipData.clipStartFrame) * animationData.totalBoneCount
                            + boneIndex;

                        _animationBakeData.boneMatrices[index] = boneTransform.localToWorldMatrix * _animationBakeData.bindPoses[boneIndex];
                    }

                    #region Move To Next Frame
                    _animationBakeData.currentClipFrame++;
                    if (animationClipData.clipFrameCount == _animationBakeData.currentClipFrame)
                    {
                        _animationBakeData.currentClipIndex++;
                        _animationBakeData.currentClipFrame = 0;
                    }

                    if (_animationBakeData.currentClipIndex == _animationBakeData.crowdPrototype.animationData.clipDataList.Count)
                    {
                        EditorApplication.update -= AnimationBakerUpdate;
                        AnimationBakerFinish();
                        return;
                    }

                    GPUIAnimationClipData nextAnimationClip = animationData.clipDataList[_animationBakeData.currentClipIndex];
                    if (animationClipData.clipIndex != nextAnimationClip.clipIndex)
                    {
                        OverrideAnimator(_animationBakeData.crowdPrototype.clipList[nextAnimationClip.clipIndex]);
                        _animationBakeData.sampleAnimator.Play(_animationBakeData.sampleAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0);
                        _animationBakeData.sampleAnimator.Update(0);
                        return;
                    }
                    _animationBakeData.sampleAnimator.Update(nextAnimationClip.length / nextAnimationClip.clipFrameCount);
                    #endregion Move To Next Frame
                }
                while (!GPUICrowdConstants.gpuiCrowdSettings.bakeUpdatePerFrame);
            }
            catch (Exception e)
            {
                ClearBakingData();
                Debug.LogError(e);
            }
        }

        public void AnimationBakerFinish()
        {
            try
            {
                #region Write To Render Texture With Compute Shader
                if (_skinnedMeshBakeComputeShader == null)
                {
                    _skinnedMeshBakeComputeShader = (ComputeShader)Resources.Load(GPUICrowdConstants.COMPUTE_SKINNED_MESH_BAKE_PATH);
                    _animationToTextureKernelID = _skinnedMeshBakeComputeShader.FindKernel(GPUICrowdConstants.COMPUTE_ANIMATION_TO_TEXTURE_KERNEL);
                }
                ComputeBuffer boneBuffer = new ComputeBuffer(_animationBakeData.boneMatrices.Length, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);
                boneBuffer.SetData(_animationBakeData.boneMatrices);

                _skinnedMeshBakeComputeShader.SetTexture(_animationToTextureKernelID, "outputTexture", _animationBakeData.animationRenderTexture);
                _skinnedMeshBakeComputeShader.SetBuffer(_animationToTextureKernelID, "boneData", boneBuffer);
                _skinnedMeshBakeComputeShader.SetInt("dataCount", _animationBakeData.boneMatrices.Length);
                _skinnedMeshBakeComputeShader.SetInts("textureSize", _animationBakeData.textureSize);
                _skinnedMeshBakeComputeShader.Dispatch(_animationToTextureKernelID,
                    Mathf.CeilToInt(_animationBakeData.boneMatrices.Length / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT), 1, 1);

                boneBuffer.Release();
                #endregion Write To Render Texture With Compute Shader

                #region Write Render Texture To Texture2D
                if(EditorUtility.DisplayCancelableProgressBar("GPU Instancer Animaton Baker", "Writing to Texture...", 0.91f))
                {
                    ClearBakingData();
                    return;
                }

                string texturePath = null;
                if (_animationBakeData.crowdPrototype.animationData.animationTexture != null)
                    texturePath = AssetDatabase.GetAssetPath(_animationBakeData.crowdPrototype.animationData.animationTexture);

                _animationBakeData.crowdPrototype.animationData.animationTexture = new Texture2D(
                    _animationBakeData.crowdPrototype.animationData.textureSizeX,
                    _animationBakeData.crowdPrototype.animationData.textureSizeY,
                    TextureFormat.RGBAHalf, false);
                Rect readingRect = new Rect(0, 0,
                    _animationBakeData.crowdPrototype.animationData.textureSizeX,
                    _animationBakeData.crowdPrototype.animationData.textureSizeY);
                RenderTexture.active = _animationBakeData.animationRenderTexture;
                _animationBakeData.crowdPrototype.animationData.animationTexture.ReadPixels(readingRect, 0, 0);
                _animationBakeData.crowdPrototype.animationData.animationTexture.Apply();
                RenderTexture.active = null;

                _animationBakeData.animationRenderTexture.Release();

                //string texturePath = "Assets/" + prefabPrototype.prefabObject.name + "_AnimationData.png";
                //byte[] bytes = prefabPrototype.animationData.animationTexture.EncodeToPNG();
                //System.IO.File.WriteAllBytes(texturePath, bytes);

                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                //// allow for larger textures if necessary:
                //TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
                //importer.maxTextureSize = prefabPrototype.animationData.textureSizeX;
                //AssetDatabase.ImportAsset(texturePath);

                if (string.IsNullOrEmpty(texturePath))
                {
                    texturePath = GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_ANIMATION_TEXTURES_PATH +
                        _animationBakeData.crowdPrototype.name + "_AnimationData.asset";

                    if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_ANIMATION_TEXTURES_PATH))
                        System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUICrowdConstants.PROTOTYPES_ANIMATION_TEXTURES_PATH);
                }

                AssetDatabase.CreateAsset(_animationBakeData.crowdPrototype.animationData.animationTexture, texturePath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                _animationBakeData.crowdPrototype.animationData.animationTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                EditorUtility.SetDirty(_animationBakeData.crowdPrototype.animationData);
                EditorUtility.SetDirty(_animationBakeData.crowdPrototype);

                EditorUtility.ClearProgressBar();
                #endregion Write Render Texture To Texture2D
            }
            catch (Exception e)
            {
                ClearBakingData();
                Debug.LogError(e);
            }
            ClearBakingData();

            if (_bakeQueue != null)
            {
                if (_bakeQueue.Count > 0)
                    SkinnedMeshBakeAnimations((GPUICrowdPrototype)_bakeQueue.Dequeue());
                else
                    _bakeQueue = null;
            }
        }

        public void ClearBakingData()
        {
            EditorApplication.update -= AnimationBakerUpdate;
            EditorUtility.ClearProgressBar();
            if (_animationBakeData != null)
            {
                if (_animationBakeData.sampleObject)
                    GameObject.DestroyImmediate(_animationBakeData.sampleObject);

                if (!_animationBakeData.hasTransformHierarchy && _animationBakeData.optimizeGameObjects)
                {
                    _animationBakeData.modelImporter.optimizeGameObjects = true;
                    _animationBakeData.modelImporter.extraExposedTransformPaths = _animationBakeData.extraExposedTransformPaths;
                    _animationBakeData.modelImporter.SaveAndReimport();
                }
            }

            _isBakingAnimation = false;
            _animationBakeData = null;
        }

        public GameObject GetSampleGameObject(GPUICrowdPrototype crowdPrototype, bool fromModel = false, int lodIndex = 0)
        {
            if (fromModel)
            {
                return GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(crowdPrototype.modelPrefabPath));
            }
            //else
            //{
            //    LODGroup lodGroup = crowdPrototype.prefabObject.GetComponent<LODGroup>();
            //    if (lodGroup != null)
            //    {
            //        LOD[] lods = lodGroup.GetLODs();
            //        if (lods.Length > lodIndex)
            //        {
            //            GameObject lodGO = lods[lodIndex].renderers.FirstOrDefault(r => r != null && r is SkinnedMeshRenderer).gameObject;
            //            while (lodGO.transform.parent.gameObject != crowdPrototype.prefabObject)
            //            {
            //                lodGO = lodGO.transform.parent.gameObject;
            //                if (lodGO == null)
            //                    return null;
            //            }
            //            return GameObject.Instantiate(lodGO);
            //        }
            //        return null;
            //    }
            else
            {
                return GameObject.Instantiate(crowdPrototype.prefabObject);
            }
            //}
        }

        private void AddChildStatesFromStateMachine(GPUICrowdAnimationData animationData, AnimatorStateMachine stateMachine, string parentPath)
        {
            foreach (ChildAnimatorState childAnimatorState in stateMachine.states)
            {
                GPUIAnimatorState state = new GPUIAnimatorState();
                state.fullPathName = parentPath + "." + childAnimatorState.state.name;
                state.hashCode = Animator.StringToHash(state.fullPathName);
                state.isBlend = childAnimatorState.state.motion is BlendTree;
                state.cycleOffset = childAnimatorState.state.cycleOffset;

                animationData.states.Add(state);
            }
            
            foreach (ChildAnimatorStateMachine casm in stateMachine.stateMachines)
            {
                AddChildStatesFromStateMachine(animationData, casm.stateMachine, parentPath + "." + casm.stateMachine.name);
            }
        }
        #endregion SkinnedMesh Bake Methods

        public void GenerateTestInstance(GPUICrowdPrototype prototype)
        {
            GameObject generatedGO = new GameObject(prototype.prefabObject.name + "(Debugger)");
            generatedGO.hideFlags = HideFlags.DontSave;
            GameObject renderersGO = new GameObject("Renderers");
            renderersGO.hideFlags = HideFlags.DontSave;
            renderersGO.transform.SetParent(generatedGO.transform);

            SkinnedMeshRenderer[] skinnedMeshRenderers = prototype.prefabObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            LODGroup lodGroup = prototype.prefabObject.GetComponent<LODGroup>();
            if (lodGroup != null)
            {
                Renderer[] lod0Renderers = lodGroup.GetLODs()[0].renderers;
                List<SkinnedMeshRenderer> lod0SkinnedMeshRenderers = new List<SkinnedMeshRenderer>();
                foreach (Renderer renderer in lod0Renderers)
                {
                    if (renderer is SkinnedMeshRenderer)
                        lod0SkinnedMeshRenderers.Add((SkinnedMeshRenderer)renderer);
                }
                skinnedMeshRenderers = lod0SkinnedMeshRenderers.ToArray();
            }
            List<Material> replacementMaterials = new List<Material>();
            Material originalMaterial;
            int s = 0;
            List<Mesh> generatedMeshList = new List<Mesh>();
            foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
            {
                GPUISkinnedMeshData smd = prototype.animationData.skinnedMeshDataList[s];
                Material[] newMaterials = new Material[smr.sharedMaterials.Length];
                for (int i = 0; i < smr.sharedMaterials.Length; i++)
                {
                    originalMaterial = smr.sharedMaterials[i];

                    Material newMaterial = new Material(GPUInstancerConstants.gpuiSettings.shaderBindings.GetInstancedShader(originalMaterial.shader.name, GPUICrowdConstants.GPUI_EXTENSION_CODE));
                    newMaterial.CopyPropertiesFromMaterial(originalMaterial);
                    newMaterial.name = originalMaterial.name + "_Test";

                    newMaterial.SetTexture("gpuiAnimationTexture", prototype.animationData.animationTexture);
                    newMaterial.SetVector("animationTextureSize", new Vector2(prototype.animationData.textureSizeX, prototype.animationData.textureSizeY));
                    newMaterial.SetFloat("totalNumberOfBones", prototype.animationData.totalBoneCount);
                    newMaterial.SetMatrix("bindPoseOffset", smd.hasBindPoseOffset ? smd.bindPoseOffset : Matrix4x4.identity);
                    newMaterial.EnableKeyword("GPUI_CA_TEXTURE");

                    replacementMaterials.Add(newMaterial);
                    newMaterials[i] = newMaterial;
                    newMaterial.hideFlags = HideFlags.DontSave;
                }
                GameObject meshRendererGO = new GameObject(smr.gameObject.name);
                meshRendererGO.hideFlags = HideFlags.DontSave;
                meshRendererGO.transform.SetParent(renderersGO.transform);

                MeshRenderer mr = meshRendererGO.AddComponent<MeshRenderer>();
                MeshFilter mf = meshRendererGO.AddComponent<MeshFilter>();
                mr.sharedMaterials = newMaterials;
                Mesh mesh = GPUICrowdUtility.GenerateMeshWithUVs(smr.sharedMesh, smd);
                mf.sharedMesh = mesh;
                generatedMeshList.Add(mesh);
                mr.gameObject.transform.position = Vector3.zero;
                mr.gameObject.transform.rotation = Quaternion.identity;
                mr.gameObject.transform.localScale = Vector3.one;
                s++;
            }
            GPUICrowdPrefabDebugger debugger = generatedGO.AddComponent<GPUICrowdPrefabDebugger>();
            debugger.crowdPrototype = prototype;
            debugger.testMaterials = replacementMaterials;
            debugger.frameIndex = 0;

            Selection.activeGameObject = generatedGO;
            SceneView.lastActiveSceneView.FrameSelected();

            foreach (Mesh m in generatedMeshList)
            {
                // scaling mesh bounds so that they are not frustum culled
                m.bounds = new Bounds(m.bounds.center, m.bounds.size * 20);
            }
        }
    }
}
#endif //GPU_INSTANCER