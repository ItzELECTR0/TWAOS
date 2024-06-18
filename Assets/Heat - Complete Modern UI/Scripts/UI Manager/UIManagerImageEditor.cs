#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(UIManagerImage))]
    public class UIManagerImageEditor : Editor
    {
        private UIManagerImage uimiTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            uimiTarget = (UIManagerImage)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var colorType = serializedObject.FindProperty("colorType");
            var useCustomColor = serializedObject.FindProperty("useCustomColor");
            var useCustomAlpha = serializedObject.FindProperty("useCustomAlpha");

            HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            HeatUIEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");

            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 10);

            if (uimiTarget.UIManagerAsset != null)
            {
                HeatUIEditorHandler.DrawProperty(colorType, customSkin, "Color Type");
                useCustomColor.boolValue = HeatUIEditorHandler.DrawToggle(useCustomColor.boolValue, customSkin, "Use Custom Color");
                if (useCustomColor.boolValue == true) { GUI.enabled = false; }
                useCustomAlpha.boolValue = HeatUIEditorHandler.DrawToggle(useCustomAlpha.boolValue, customSkin, "Use Custom Alpha");
            }

            else { EditorGUILayout.HelpBox("UI Manager should be assigned.", MessageType.Error); }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif