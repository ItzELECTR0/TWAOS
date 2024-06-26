#if GPU_INSTANCER
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    [Serializable]
    public class GPUICrowdAnimationData : ScriptableObject
    {
        public float bakedCAVersion;
        public List<GPUIAnimationClipData> clipDataList;
        public List<GPUISkinnedMeshData> skinnedMeshDataList;
        public List<GPUIBone> bones;
        public List<GPUIAnimatorState> states;
        public Texture2D animationTexture;
        public int totalFrameCount;
        public int totalBoneCount;
        public int textureSizeX;
        public int textureSizeY;
        public bool optimizeGameObjects = true;

        public bool useCrowdAnimator = false;
        public int crowdAnimatorDefaultClip = 0;

        public bool applyRootMotion;
        public GPUIRootMotion[] rootMotions;

        // Bone readback
        public bool applyBoneUpdates;
        public bool isSynchronousBoneUpdates = true;
        public Matrix4x4[] bindPoses;
        public string[] exposedTransforms;
        public int[] exposedBoneIndexes;
        public int asyncBoneUpdateMaxLatency = 3;

        public GPUICrowdAnimationData()
        {
            clipDataList = new List<GPUIAnimationClipData>();
            skinnedMeshDataList = new List<GPUISkinnedMeshData>();
            bones = new List<GPUIBone>();
            optimizeGameObjects = true;
        }

        public GPUIBone GetBoneByTransform(string boneTransformName)
        {
            if (bones == null)
                bones = new List<GPUIBone>();
            return bones.Find(b => b.boneTransformName == boneTransformName);
        }

        /// <summary>
        /// Returns the bone index on the prototype's bones List
        /// </summary>
        public int GetBoneIndexByTransform(string boneTransformName)
        {
            GPUIBone bone = GetBoneByTransform(boneTransformName);
            if (bone != null)
                return bone.boneIndex;
            else
            {
                Debug.LogError("Can not find bone index for transform: " + boneTransformName);
                return -1;
            }
        }

        public void BoneSetParent(GPUIBone child, GPUIBone parent)
        {
            child.parentBoneIndex = parent.boneIndex;
            parent.AddChild(child.boneIndex);
        }

        public bool IsOptimizeGameObjects()
        {
            return (useCrowdAnimator /*&& !applyBoneUpdates*/) || (!useCrowdAnimator && optimizeGameObjects);
        }

        public GPUISkinnedMeshData GetSkinnedMeshDataByName(string transformName)
        {
            GPUISkinnedMeshData result = null;
            if (skinnedMeshDataList != null && skinnedMeshDataList.Count > 0)
            {
                result = skinnedMeshDataList.Find(smd => smd.transformName.Equals(transformName));
                if (result == null)
                    Debug.LogError("Can not find Skinned Mesh Renderer with name: " + transformName);
            }
            return result;
        }
    }

    [Serializable]
    public struct GPUIAnimationClipData
    {
        // bool values cause issues on IL2CPP, using int instead
        public int clipIndex;
        public int clipStartFrame;
        public int clipFrameCount;
        public int hasRootMotion;
        public float length;
        public int isLoopDisabled;
        public int isClipLooping;

        public bool HasRootMotion()
        {
            return hasRootMotion > 0;
        }
        public bool IsLoopDisabled()
        {
            return isLoopDisabled > 0;
        }
        public bool IsClipLooping()
        {
            return isClipLooping > 0;
        }
    }

    [Serializable]
    public class GPUISkinnedMeshData
    {
        public string transformName;
        public int[] boneIndexes;
        public int rootBoneIndex;
        public bool hasBindPoseOffset;
        public Matrix4x4 bindPoseOffset;
        public bool isOptional;
        [NonSerialized]
        public bool isOptionalPrototypeGenerated;
        [NonSerialized]
        public GPUICrowdPrototype optionalPrototype;

        public int GetArrayIndexOfBone(GPUIBone bone)
        {
            for (int i = 0; i < boneIndexes.Length; i++)
            {
                if (boneIndexes[i] == bone.boneIndex)
                    return i;
            }
            return -1;
        }
    }

    [Serializable]
    public class GPUIBone
    {
        public string boneTransformName;
        public int boneIndex;
        public int parentBoneIndex;
        public List<int> childBoneIndexes;
        public bool dontDestroy;
        public bool isRoot;

        public void AddChild(int childBoneIndex)
        {
            if (childBoneIndexes == null)
                childBoneIndexes = new List<int>();
            if (!childBoneIndexes.Contains(childBoneIndex))
                childBoneIndexes.Add(childBoneIndex);
        }
        public void RemoveChild(int childBoneIndex)
        {
            if (childBoneIndexes != null)
                childBoneIndexes.Remove(childBoneIndex);
        }
    }

    [Serializable]
    public struct GPUIRootMotion
    {
        // bool values cause issues on IL2CPP, using int instead
        public int hasMotion;
        public int isPositionOnly;
        public Matrix4x4 motionMatrix;

        public bool HasMotion()
        {
            return hasMotion > 0;
        }
        public bool IsPositionOnly()
        {
            return isPositionOnly > 0;
        }
    }

    [Serializable]
    public class GPUIAnimatorState
    {
        public string fullPathName;
        public int hashCode;
        public float cycleOffset;
        public bool isBlend;
    }
}
#endif //GPU_INSTANCER