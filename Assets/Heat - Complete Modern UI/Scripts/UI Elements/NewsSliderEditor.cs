#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NewsSlider))]
    public class NewsSliderEditor : Editor
    {
        private NewsSlider nsTarget;
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            nsTarget = (NewsSlider)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "News Slider Top Header");

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

            var items = serializedObject.FindProperty("items");

            var itemPreset = serializedObject.FindProperty("itemPreset");
            var itemParent = serializedObject.FindProperty("itemParent");
            var timerPreset = serializedObject.FindProperty("timerPreset");
            var timerParent = serializedObject.FindProperty("timerParent");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var sliderTimer = serializedObject.FindProperty("sliderTimer");

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(items, new GUIContent("Slider Items"), true);
                    EditorGUI.indentLevel = 0;
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(itemPreset, customSkin, "Item Preset");
                    HeatUIEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    HeatUIEditorHandler.DrawProperty(timerPreset, customSkin, "Timer Preset");
                    HeatUIEditorHandler.DrawProperty(timerParent, customSkin, "Timer Parent");
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    useLocalization.boolValue = HeatUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    HeatUIEditorHandler.DrawProperty(sliderTimer, customSkin, "Slider Timer");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif