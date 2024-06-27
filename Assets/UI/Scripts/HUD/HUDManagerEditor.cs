#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HUDManager))]
    public class HUDManagerEditor : Editor
    {
        private HUDManager hmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            hmTarget = (HUDManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var HUDPanel = serializedObject.FindProperty("HUDPanel");

            var fadeSpeed = serializedObject.FindProperty("fadeSpeed");
            var defaultBehaviour = serializedObject.FindProperty("defaultBehaviour");

            var onSetVisible = serializedObject.FindProperty("onSetVisible");
            var onSetInvisible = serializedObject.FindProperty("onSetInvisible");

            HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            HeatUIEditorHandler.DrawProperty(HUDPanel, customSkin, "HUD Panel");

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            HeatUIEditorHandler.DrawProperty(fadeSpeed, customSkin, "Fade Speed", "Sets the fade animation speed.");
            HeatUIEditorHandler.DrawProperty(defaultBehaviour, customSkin, "Default Behaviour");

            HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
            EditorGUILayout.PropertyField(onSetVisible, new GUIContent("On Set Visible"), true);
            EditorGUILayout.PropertyField(onSetInvisible, new GUIContent("On Set Invisible"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif