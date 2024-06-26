using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerFloatingOriginHandler : MonoBehaviour
    {
        public List<GPUInstancerManager> gPUIManagers;
        public bool applyRotationAndScale;

        private Vector3 _previousPosition;
        private Matrix4x4 _previousMatrix;
        private Transform _cachedTransform;

        void OnEnable()
        {
            _cachedTransform = transform;
            _previousPosition = _cachedTransform.position;
            _previousMatrix = _cachedTransform.localToWorldMatrix;
            _cachedTransform.hasChanged = false;
        }

        void Update()
        {
            if (_cachedTransform.hasChanged)
            {
                if (applyRotationAndScale)
                {
                    Matrix4x4 newMatrix = _cachedTransform.localToWorldMatrix;
                    if (newMatrix != _previousMatrix)
                    {
                        foreach (GPUInstancerManager manager in gPUIManagers)
                        {
                            if (manager != null)
                                GPUInstancerAPI.SetGlobalMatrixOffset(manager, newMatrix * _previousMatrix.inverse);
                        }
                        _previousMatrix = _cachedTransform.localToWorldMatrix;
                        _cachedTransform.hasChanged = false;
                    }
                }
                else if (_cachedTransform.position != _previousPosition)
                {
                    foreach (GPUInstancerManager manager in gPUIManagers)
                    {
                        if (manager != null)
                            GPUInstancerAPI.SetGlobalPositionOffset(manager, _cachedTransform.position - _previousPosition);
                    }
                    _previousPosition = _cachedTransform.position;
                    _cachedTransform.hasChanged = false;
                }
            }
        }
    }
}