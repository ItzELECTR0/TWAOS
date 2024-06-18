#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(UIManagerLogo))]
    public class UIManagerLogoEditor : Editor
    {
        private UIManagerLogo uimlTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            uimlTarget = (UIManagerLogo)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var logoType = serializedObject.FindProperty("logoType");

            HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            HeatUIEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 10);

            if (uimlTarget.UIManagerAsset != null)
            {
                HeatUIEditorHandler.DrawProperty(logoType, customSkin, "Logo Type");
            }

            else { EditorGUILayout.HelpBox("UI Manager should be assigned.", MessageType.Error); }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif