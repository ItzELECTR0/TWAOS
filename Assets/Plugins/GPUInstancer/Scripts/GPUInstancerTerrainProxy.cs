using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GPUInstancer
{
    [ExecuteInEditMode]
    public class GPUInstancerTerrainProxy : MonoBehaviour
    {
        public GPUInstancerDetailManager detailManager;
        public GPUInstancerTreeManager treeManager;
        public bool beingDestroyed;
        public int terrainSelectedToolIndex = -1;

        private void OnDestroy()
        {
            beingDestroyed = true;
        }

#if UNITY_EDITOR
        private long _lastDetailChangeTicks;
        private static readonly long _waitForTicks = 4000000;
        private bool _isBeingModified;

        private void Update()
        {
            if (!_isBeingModified && detailManager != null && detailManager.gpuiSimulator != null && detailManager.keepSimulationLive && !detailManager.gpuiSimulator.simulateAtEditor)
                detailManager.gpuiSimulator.StartSimulation();
        }

        private void OnTerrainChanged(TerrainChangedFlags flags)
        {
            if (Application.isPlaying)
                return;
            if ((flags & TerrainChangedFlags.RemoveDirtyDetailsImmediately) != 0 || (flags & TerrainChangedFlags.Heightmap) != 0 || (flags & TerrainChangedFlags.FlushEverythingImmediately) != 0)
            {
                _lastDetailChangeTicks = DateTime.Now.Ticks;
                if (detailManager != null && detailManager.gpuiSimulator != null && detailManager.gpuiSimulator.simulateAtEditor && detailManager.keepSimulationLive && detailManager.updateSimulation)
                {
                    _isBeingModified = true;
                    EditorApplication.update -= RestartDetailSimulation;
                    EditorApplication.update += RestartDetailSimulation;
                }
            }
        }

        private void RestartDetailSimulation()
        {
            if (Application.isPlaying || detailManager == null || detailManager.gpuiSimulator == null)
            {
                EditorApplication.update -= RestartDetailSimulation;
                _isBeingModified = false;
                return;
            }
            if (DateTime.Now.Ticks - _lastDetailChangeTicks < _waitForTicks)
                return;
            detailManager.gpuiSimulator.StopSimulation();
            detailManager.terrain.detailObjectDistance = 0;
            detailManager.gpuiSimulator.StartSimulation();
            _isBeingModified = false;
            EditorApplication.update -= RestartDetailSimulation;
        }
#endif
    }
}
