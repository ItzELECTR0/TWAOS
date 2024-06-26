#if GPU_INSTANCER
using UnityEditor;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    [CustomEditor(typeof(GPUICrowdSettings))]
    public class GPUICrowdSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GUILayout.Space(5);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUICrowdEditorConstants.GPUI_EXTENSION_TITLE, GPUInstancerEditorConstants.Styles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Version No", GPUICrowdConstants.gpuiCrowdSettings.extensionVersionNo);
            EditorGUI.EndDisabledGroup();
            GPUICrowdConstants.gpuiCrowdSettings.bakeUpdatePerFrame = EditorGUILayout.Toggle("Update Editor For Animation Baking", GPUICrowdConstants.gpuiCrowdSettings.bakeUpdatePerFrame);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }
    }
}
#endif //GPU_INSTANCER