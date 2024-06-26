#if GPU_INSTANCER
using UnityEditor;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    [CustomEditor(typeof(GPUICrowdPrefab)), CanEditMultipleObjects]
    public class GPUICrowdPrefabEditor : Editor
    {
        private GPUICrowdPrefab[] _prefabScripts;

        private bool _showExposedTransformList;

        protected void OnEnable()
        {
            Object[] monoObjects = targets;
            _prefabScripts = new GPUICrowdPrefab[monoObjects.Length];
            for (int i = 0; i < monoObjects.Length; i++)
            {
                _prefabScripts[i] = monoObjects[i] as GPUICrowdPrefab;
            }
        }

        public override void OnInspectorGUI()
        {
            if(_prefabScripts != null)
            {
                    
                if (_prefabScripts.Length >= 1 && _prefabScripts[0] != null && _prefabScripts[0].prefabPrototype != null)
                {
                    bool isPrefab = _prefabScripts[0].prefabPrototype.prefabObject == _prefabScripts[0].gameObject;

                    if (_prefabScripts.Length == 1)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_prototypeSO, _prefabScripts[0].prefabPrototype, typeof(GPUInstancerPrefabPrototype), false);
                        EditorGUI.EndDisabledGroup();

                        if (_prefabScripts[0].exposedTransforms != null && _prefabScripts[0].exposedTransforms.Length > 0)
                        {
                            _showExposedTransformList = EditorGUILayout.BeginFoldoutHeaderGroup(_showExposedTransformList, "Exposed Transforms");
                            if (_showExposedTransformList)
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                for (int i = 0; i < _prefabScripts[0].exposedTransforms.Length; i++)
                                {
                                    EditorGUILayout.ObjectField(_prefabScripts[0].exposedTransforms[i], typeof(Transform), false);
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            EditorGUILayout.EndFoldoutHeaderGroup();
                        }

                        if (!isPrefab)
                        {
                            if (Application.isPlaying)
                            {
                                if (_prefabScripts[0].state == PrefabInstancingState.Instanced)
                                    GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabInstancingActive + _prefabScripts[0].gpuInstancerID, GPUInstancerEditorConstants.Styles.boldLabel);
                                else if (_prefabScripts[0].state == PrefabInstancingState.Disabled)
                                    GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabInstancingDisabled + _prefabScripts[0].gpuInstancerID, GPUInstancerEditorConstants.Styles.boldLabel);
                                else
                                    GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabInstancingNone, GPUInstancerEditorConstants.Styles.boldLabel);
                            }
                        }
                    }

                    if (isPrefab && !Application.isPlaying)
                    {
                        foreach (GPUInstancerPrefab prefabScript in _prefabScripts)
                        {
                            if (prefabScript != null && prefabScript.prefabPrototype != null)
                            {
                                GPUInstancerPrefabManagerEditor.CheckPrefabRigidbodies(prefabScript.prefabPrototype);
                            }
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (_prefabScripts[0].prefabPrototype.meshRenderersDisabled)
                        {
                            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.enableMeshRenderers, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                                () =>
                                {
                                    foreach (GPUInstancerPrefab prefabScript in _prefabScripts)
                                    {
                                        if (prefabScript != null && prefabScript.prefabPrototype != null)
                                        {
                                            GPUInstancerPrefabManagerEditor.SetRenderersEnabled(prefabScript.prefabPrototype, true);
                                        }
                                    }
                                });
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        static void DrawGizmo(GPUInstancerPrefab instance, GizmoType gizmoType)
        {
            if (EditorApplication.isPlaying || !instance.enabled || instance.prefabPrototype == null || !instance.prefabPrototype.meshRenderersDisabled)
                return;

            MeshRenderer[] render = instance.GetComponentsInChildren<MeshRenderer>();
            MeshFilter[] filter = instance.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i != render.Length; ++i)
            {
                for (int m = 0; m < render[i].sharedMaterials.Length; m++)
                {
                    render[i].sharedMaterials[m].SetPass(0);
                    Graphics.DrawMeshNow(filter[i].sharedMesh, instance.transform.localToWorldMatrix, m);
                }
                //Graphics.DrawMesh(filter[i].sharedMesh, obj.transform.localToWorldMatrix, render[i].sharedMaterial, obj.layer);
                //Gizmos.DrawMesh(filter[i].sharedMesh, obj.transform.position, obj.transform.rotation);
            }
        }
    }
}
#endif //GPU_INSTANCER