#if GPU_INSTANCER
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdPrefabDebugger : MonoBehaviour
    {
        [HideInInspector]
        public GPUICrowdPrototype crowdPrototype;
        [HideInInspector]
        public List<Material> testMaterials;
        [HideInInspector]
        public float frameIndex;

        public void OnFrameIndexChanged()
        {
            if (frameIndex < 0 || frameIndex > crowdPrototype.animationData.totalFrameCount)
                frameIndex = 0;

            foreach (Material mat in testMaterials)
            {
                mat.SetFloat("frameIndex", frameIndex);
            }
        }
    }
}
#endif //GPU_INSTANCER