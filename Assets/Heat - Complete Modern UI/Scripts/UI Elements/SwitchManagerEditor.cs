#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(SwitchManager))]
    public class SwitchManagerEditor : Editor
    {
        private GUISkin customSkin;
        private SwitchManager switchTarget;
        private int currentTab;

        private void OnEnable()
        {
            switchTarget = (SwitchManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Switch Top Header");

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

            var onValueChanged = serializedObject.FindProperty("onValueChanged");
            var onEvents = serializedObject.FindProperty("onEvents");
            var offEvents = serializedObject.FindProperty("offEvents");

            var switchAnimator = serializedObject.FindProperty("switchAnimator");
            var highlightCG = serializedObject.FindProperty("highlightCG");
            var audioManager = serializedObject.FindProperty("audioManager");

            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");
            var isOn = serializedObject.FindProperty("isOn");
            var isInteractable = serializedObject.FindProperty("isInteractable");
            var invokeAtStart = serializedObject.FindProperty("invokeAtStart");
            var useSounds = serializedObject.FindProperty("useSounds");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var saveValue = serializedObject.FindProperty("saveValue");
            var saveKey = serializedObject.FindProperty("saveKey");

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    isOn.boolValue = HeatUIEditorHandler.DrawToggle(isOn.boolValue, customSkin, "Is On");
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveValue.boolValue = HeatUIEditorHandler.DrawTogglePlain(saveValue.boolValue, customSkin, "Save Value");
                    GUILayout.Space(3);

                    if (saveValue.boolValue == true)
                    {
                        HeatUIEditorHandler.DrawPropertyPlainCW(saveKey, customSkin, "Save Key:", 70);
                        EditorGUILayout.HelpBox("Each switch should has its own unique key.", MessageType.Info);
                    }

                    GUILayout.EndVertical();

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    EditorGUILayout.PropertyField(onEvents, new GUIContent("On Events"), true);
                    EditorGUILayout.PropertyField(offEvents, new GUIContent("Off Events"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(switchAnimator, customSkin, "Switch hAnimator");
                    HeatUIEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    if (useSounds.boolValue == true) { HeatUIEditorHandler.DrawProperty(audioManager, customSkin, "Audio Manager"); }
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    isOn.boolValue = HeatUIEditorHandler.DrawToggle(isOn.boolValue, customSkin, "Is On");
                    isInteractable.boolValue = HeatUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    invokeAtStart.boolValue = HeatUIEditorHandler.DrawToggle(invokeAtStart.boolValue, customSkin, "Invoke At Start", "Process events on awake.");
                    useSounds.boolValue = HeatUIEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Switch Sounds");
                    useUINavigation.boolValue = HeatUIEditorHandler.DrawToggle(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveValue.boolValue = HeatUIEditorHandler.DrawTogglePlain(saveValue.boolValue, customSkin, "Save Value");
                    GUILayout.Space(3);

                    if (saveValue.boolValue == true)
                    {
                        HeatUIEditorHandler.DrawPropertyPlainCW(saveKey, customSkin, "Save Key:", 70);
                        EditorGUILayout.HelpBox("Each switch should has its own unique key.", MessageType.Info);
                    }

                    GUILayout.EndVertical();

                    HeatUIEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif