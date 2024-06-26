#if GPU_INSTANCER
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    [Serializable]
    public class GPUICrowdPrototype : GPUInstancerPrefabPrototype
    {
        public GPUICrowdAnimationData animationData;
        public int frameRate = 60;
        public AnimatorCullingMode animatorCullingMode;
        public string modelPrefabPath;
        public bool hasNoAnimator;
        public List<AnimationClip> clipList;

        // For optional skinned mesh renderers
        public bool hasOptionalRenderers;
        [NonSerialized]
        public bool isChildPrototype;
        [NonSerialized]
        public GPUICrowdPrototype parentPrototype;

        public bool IsBakeRequired()
        {
            return animationData == null || animationData.animationTexture == null || animationData.bakedCAVersion < GPUICrowdConstants.LATEST_REBAKE_VERSION;
        }
    }
}
#endif //GPU_INSTANCER