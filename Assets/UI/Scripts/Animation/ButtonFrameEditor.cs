#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonFrame))]
    public class ButtonFrameEditor : Editor
    {
        private ButtonFrame bfTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            bfTarget = (ButtonFrame)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var animator = serializedObject.FindProperty("animator");
            var buttonManager = serializedObject.FindProperty("buttonManager");
            var panelButton = serializedObject.FindProperty("panelButton");
            var buttonType = serializedObject.FindProperty("buttonType");

            HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            HeatUIEditorHandler.DrawProperty(animator, customSkin, "Animator");

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            HeatUIEditorHandler.DrawPropertyPlain(buttonType, customSkin, "Animator");
            if (buttonType.enumValueIndex == 0) { HeatUIEditorHandler.DrawProperty(buttonManager, customSkin, "Button Manager"); }
            else if (buttonType.enumValueIndex == 1) { HeatUIEditorHandler.DrawProperty(panelButton, customSkin, "Panel Button"); }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif