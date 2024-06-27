#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GradientFilter))]
    public class GradientFilterEditor : Editor
    {
        private GradientFilter gfTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            gfTarget = (GradientFilter)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var selectedFilter = serializedObject.FindProperty("selectedFilter");
            var opacity = serializedObject.FindProperty("opacity");

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
            HeatUIEditorHandler.DrawProperty(selectedFilter, customSkin, "Selected Filter");
            HeatUIEditorHandler.DrawProperty(opacity, customSkin, "Opacity");

            gfTarget.UpdateFilter();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif