using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerPrefabListRuntimeHandler : MonoBehaviour
    {
        public GPUInstancerPrefabManager prefabManager;
        private IEnumerable<GPUInstancerPrefab> _gpuiPrefabs;
        private bool _isIntancesAdded;

        private void OnEnable()
        {
            if (prefabManager == null)
                return;

            if (!prefabManager.prototypeList.All(p => ((GPUInstancerPrefabPrototype)p).meshRenderersDisabled))
            {
                Debug.LogWarning("GPUInstancerPrefabListRuntimeHandler can not run in Threads while Mesh Renderers are enabled on the prefabs. Disabling threading...");
            }

            _gpuiPrefabs = gameObject.GetComponentsInChildren<GPUInstancerPrefab>(true);

            if (_gpuiPrefabs != null && _gpuiPrefabs.Count() > 0)
            {
                _isIntancesAdded = true;
                prefabManager.AddPrefabInstances(_gpuiPrefabs);
            }
        }

        private void OnDisable()
        {
            _isIntancesAdded = false;
            if (prefabManager == null)
                return;

            if (_gpuiPrefabs != null && _gpuiPrefabs.Count() > 0)
            {
                prefabManager.RemovePrefabInstances(_gpuiPrefabs);
            }
            _gpuiPrefabs = null;
        }

        public void SetManager(GPUInstancerPrefabManager prefabManager)
        {
            if (_isIntancesAdded)
                OnDisable();
            this.prefabManager = prefabManager;
            if (isActiveAndEnabled)
                OnEnable();
        }
    }
}
