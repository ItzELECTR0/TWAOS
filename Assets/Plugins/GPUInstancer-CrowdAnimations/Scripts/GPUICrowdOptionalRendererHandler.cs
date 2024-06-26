using System;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdOptionalRendererHandler : MonoBehaviour
    {
        private GPUICrowdPrefab _crowdPrefab;


        private void Awake()
        {
            _crowdPrefab = GetComponent<GPUICrowdPrefab>();
        }

        private void OnEnable()
        {
            if ((_crowdPrefab.state == PrefabInstancingState.None || _crowdPrefab.state == PrefabInstancingState.Disabled))
            {
                _crowdPrefab.state = PrefabInstancingState.Instanced;
                if (_crowdPrefab.runtimeData != null && _crowdPrefab.runtimeData.instanceDataNativeArray.IsCreated)
                {
                    _crowdPrefab.runtimeData.instanceDataNativeArray[_crowdPrefab.gpuInstancerID - 1] = _crowdPrefab.GetInstanceTransform().localToWorldMatrix;
                    _crowdPrefab.runtimeData.transformDataModified = true;
                }
            }
        }

        private void OnDisable()
        {
            if (_crowdPrefab.state == PrefabInstancingState.Instanced)
            {
                _crowdPrefab.state = PrefabInstancingState.Disabled;
                if (_crowdPrefab.runtimeData != null && _crowdPrefab.runtimeData.instanceDataNativeArray.IsCreated)
                {
                    _crowdPrefab.runtimeData.instanceDataNativeArray[_crowdPrefab.gpuInstancerID - 1] = Matrix4x4.zero;
                    _crowdPrefab.runtimeData.transformDataModified = true;
                }
            }
        }

        internal void SetupOptionalRenderer()
        {
            if (_crowdPrefab.runtimeData.instanceDataNativeArray.IsCreated)
            {
                if (_crowdPrefab.state == PrefabInstancingState.Instanced)
                {
                    _crowdPrefab.runtimeData.instanceDataNativeArray[_crowdPrefab.gpuInstancerID - 1] = _crowdPrefab.GetInstanceTransform().localToWorldMatrix;
                    _crowdPrefab.runtimeData.transformDataModified = true;
                }
                else if (_crowdPrefab.state == PrefabInstancingState.Disabled)
                {
                    _crowdPrefab.runtimeData.instanceDataNativeArray[_crowdPrefab.gpuInstancerID - 1] = Matrix4x4.zero;
                    _crowdPrefab.runtimeData.transformDataModified = true;
                }
            }
        }
    }
}
