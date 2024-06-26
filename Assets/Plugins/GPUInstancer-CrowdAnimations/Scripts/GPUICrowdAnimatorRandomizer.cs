#if GPU_INSTANCER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdAnimatorRandomizer : MonoBehaviour
    {
        public GPUICrowdManager crowdManager;
        [Tooltip("Optional parameter to randomize only the given prototypes")]
        public List<GPUICrowdPrototype> crowdPrototypeFilter;
        public bool randomizeClips;
        public bool randomizeFrame;
        public bool resetAnimations;

        void OnEnable()
        {
            if (crowdManager != null)
                StartCoroutine(RandomizeCrowdAnimators());
        }

        private void Reset()
        {
            if (crowdManager == null)
                crowdManager = FindObjectOfType<GPUICrowdManager>();
        }

        IEnumerator RandomizeCrowdAnimators()
        {
            if (crowdManager != null)
            {
                while (!crowdManager.isInitialized)
                    yield return null;
                RandomizeAnimations();
            }
        }

        public void RandomizeAnimations()
        {
            if (crowdManager != null)
            {
                Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>> registeredPrefabInstances = crowdManager.GetRegisteredPrefabsRuntimeData();
                GPUIAnimationClipData clipData;
                float startTime;
                if (registeredPrefabInstances != null)
                {
                    foreach (GPUICrowdPrototype crowdPrototype in registeredPrefabInstances.Keys)
                    {
                        if (crowdPrototypeFilter != null && crowdPrototypeFilter.Count > 0 && !crowdPrototypeFilter.Contains(crowdPrototype))
                            continue;
                        if (crowdPrototype.animationData != null && crowdPrototype.animationData.useCrowdAnimator)
                        {
                            foreach (GPUICrowdPrefab crowdInstance in registeredPrefabInstances[crowdPrototype])
                            {
                                clipData = resetAnimations ? crowdPrototype.animationData.clipDataList[crowdPrototype.animationData.crowdAnimatorDefaultClip] : crowdInstance.crowdAnimator.currentAnimationClipData[0];
                                if (!resetAnimations && randomizeClips)
                                    clipData = crowdPrototype.animationData.clipDataList[UnityEngine.Random.Range(0, crowdPrototype.animationData.clipDataList.Count)];
                                startTime = resetAnimations ? 0 : -1;
                                if (!resetAnimations && randomizeFrame)
                                    startTime = UnityEngine.Random.Range(0, clipData.length);

                                GPUICrowdAPI.StartAnimation(crowdInstance, clipData, startTime);
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif //GPU_INSTANCER