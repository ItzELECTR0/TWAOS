#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PanelButton))]
    public class PanelButtonEditor : Editor
    {
        private PanelButton buttonTarget;
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            buttonTarget = (PanelButton)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Panel Button Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = HeatUIEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var buttonIcon = serializedObject.FindProperty("buttonIcon");
            var buttonText = serializedObject.FindProperty("buttonText");

            var disabledCG = serializedObject.FindProperty("disabledCG");
            var normalCG = serializedObject.FindProperty("normalCG");
            var highlightCG = serializedObject.FindProperty("highlightCG");
            var selectCG = serializedObject.FindProperty("selectCG");
            var audioManager = serializedObject.FindProperty("audioManager");
            var disabledTextObj = serializedObject.FindProperty("disabledTextObj");
            var normalTextObj = serializedObject.FindProperty("normalTextObj");
            var highlightTextObj = serializedObject.FindProperty("highlightTextObj");
            var selectTextObj = serializedObject.FindProperty("selectTextObj");
            var disabledImageObj = serializedObject.FindProperty("disabledImageObj");
            var normalImageObj = serializedObject.FindProperty("normalImageObj");
            var highlightImageObj = serializedObject.FindProperty("highlightImageObj");
            var selectedImageObj = serializedObject.FindProperty("selectedImageObj");
            var seperator = serializedObject.FindProperty("seperator");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var isSelected = serializedObject.FindProperty("isSelected");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useCustomText = serializedObject.FindProperty("useCustomText");
            var useSeperator = serializedObject.FindProperty("useSeperator");
            var useSounds = serializedObject.FindProperty("useSounds");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");

            var onClick = serializedObject.FindProperty("onClick");
            var onHover = serializedObject.FindProperty("onHover");
            var onLeave = serializedObject.FindProperty("onLeave");
            var onSelect = serializedObject.FindProperty("onSelect");

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    HeatUIEditorHandler.DrawPropertyCW(buttonIcon, customSkin, "Button Icon", 80);
                    if (useCustomText.boolValue == false) { HeatUIEditorHandler.DrawPropertyCW(buttonText, customSkin, "Button Text", 80); }
                    if (buttonTarget.buttonIcon != null || useCustomText.boolValue == false) { buttonTarget.UpdateUI(); }

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
                    EditorGUILayout.PropertyField(onHover, new GUIContent("On Hover"), true);
                    EditorGUILayout.PropertyField(onLeave, new GUIContent("On Leave"), true);
                    EditorGUILayout.PropertyField(onSelect, new GUIContent("On Select"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(disabledCG, customSkin, "Disabled CG");
                    HeatUIEditorHandler.DrawProperty(normalCG, customSkin, "Normal CG");
                    HeatUIEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    HeatUIEditorHandler.DrawProperty(selectCG, customSkin, "Select CG");
                    HeatUIEditorHandler.DrawProperty(disabledTextObj, customSkin, "Disabled Text");
                    HeatUIEditorHandler.DrawProperty(normalTextObj, customSkin, "Normal Text");
                    HeatUIEditorHandler.DrawProperty(highlightTextObj, customSkin, "Highlight Text");
                    HeatUIEditorHandler.DrawProperty(selectTextObj, customSkin, "Select Text");
                    HeatUIEditorHandler.DrawProperty(disabledImageObj, customSkin, "Disabled Image");
                    HeatUIEditorHandler.DrawProperty(normalImageObj, customSkin, "Normal Image");
                    HeatUIEditorHandler.DrawProperty(highlightImageObj, customSkin, "Highlight Image");
                    HeatUIEditorHandler.DrawProperty(selectedImageObj, customSkin, "Select Image");
                    HeatUIEditorHandler.DrawProperty(seperator, customSkin, "Seperator");
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    isInteractable.boolValue = HeatUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    isSelected.boolValue = HeatUIEditorHandler.DrawToggle(isSelected.boolValue, customSkin, "Is Selected");
                    useLocalization.boolValue = HeatUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    useCustomText.boolValue = HeatUIEditorHandler.DrawToggle(useCustomText.boolValue, customSkin, "Use Custom Text", "Bypasses inspector values and allows manual editing.");
                    useSeperator.boolValue = HeatUIEditorHandler.DrawToggle(useSeperator.boolValue, customSkin, "Use Seperator");
                    useUINavigation.boolValue = HeatUIEditorHandler.DrawToggle(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-2);
                    GUILayout.BeginHorizontal();
                    useSounds.boolValue = GUILayout.Toggle(useSounds.boolValue, new GUIContent("Use Button Sounds"), customSkin.FindStyle("Toggle"));
                    useSounds.boolValue = GUILayout.Toggle(useSounds.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                    if (useSounds.boolValue == true) { HeatUIEditorHandler.DrawProperty(audioManager, customSkin, "Audio Manager"); }
                    GUILayout.EndVertical();

                    HeatUIEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier", "Set the animation fade multiplier.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif