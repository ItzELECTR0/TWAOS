#if GPU_INSTANCER
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public abstract class GPUIAnimatorWorkflow
    {
        public GPUIAnimationClipData[] currentAnimationClipData;
        public Vector4 currentAnimationClipDataWeights;
        public int activeClipCount;
    }
}
#endif //GPU_INSTANCER