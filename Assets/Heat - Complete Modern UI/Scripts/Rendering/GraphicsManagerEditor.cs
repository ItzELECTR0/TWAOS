#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GraphicsManager))]
    public class GraphicsManagerEditor : Editor
    {
        private GraphicsManager gmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            gmTarget = (GraphicsManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var resolutionDropdown = serializedObject.FindProperty("resolutionDropdown");

            var initializeResolutions = serializedObject.FindProperty("initializeResolutions");

            HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            HeatUIEditorHandler.DrawPropertyCW(resolutionDropdown, customSkin, "Resolution Dropdown", 132);

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            initializeResolutions.boolValue = HeatUIEditorHandler.DrawToggle(initializeResolutions.boolValue, customSkin, "Initialize Resolutions");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif