#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(SliderManager))]
    public class SliderManagerEditor : Editor
    {
        private GUISkin customSkin;
        private SliderManager sTarget;
        private int currentTab;

        private void OnEnable()
        {
            sTarget = (SliderManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Slider Top Header");

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

            var sliderObject = serializedObject.FindProperty("mainSlider");
            var valueText = serializedObject.FindProperty("valueText");
            var highlightCG = serializedObject.FindProperty("highlightCG");

            var saveValue = serializedObject.FindProperty("saveValue");
            var saveKey = serializedObject.FindProperty("saveKey");
            var usePercent = serializedObject.FindProperty("usePercent");
            var useRoundValue = serializedObject.FindProperty("useRoundValue");
            var showValue = serializedObject.FindProperty("showValue");
            var showPopupValue = serializedObject.FindProperty("showPopupValue");
            var invokeOnAwake = serializedObject.FindProperty("invokeOnAwake");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (sTarget.mainSlider != null)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();

                        float tempValue = sTarget.mainSlider.value;
                        EditorGUILayout.LabelField(new GUIContent("Current Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        sTarget.mainSlider.value = EditorGUILayout.Slider(sTarget.mainSlider.value, sTarget.mainSlider.minValue, sTarget.mainSlider.maxValue);
                        if (tempValue != sTarget.mainSlider.value) { Undo.RegisterCompleteObjectUndo(sTarget.mainSlider, sTarget.name); }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        float tempMinValue = sTarget.mainSlider.minValue;

                        if (sTarget.mainSlider.wholeNumbers == false) { tempMinValue = EditorGUILayout.FloatField(new GUIContent("Min Value"), sTarget.mainSlider.minValue); }
                        else { tempMinValue = EditorGUILayout.IntField(new GUIContent("Min Value"), (int)sTarget.mainSlider.minValue); }

                        if (tempMinValue < sTarget.mainSlider.maxValue && tempMinValue != sTarget.mainSlider.minValue) 
                        {
                            sTarget.mainSlider.minValue = tempMinValue;
                            EditorUtility.SetDirty(sTarget.mainSlider);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                        else { tempMinValue = sTarget.mainSlider.minValue; }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        float tempMaxValue = sTarget.mainSlider.maxValue;

                        if (sTarget.mainSlider.wholeNumbers == false) { tempMaxValue = EditorGUILayout.FloatField(new GUIContent("Max Value"), sTarget.mainSlider.maxValue); }
                        else { tempMaxValue = EditorGUILayout.IntField(new GUIContent("Max Value"), (int)sTarget.mainSlider.maxValue); }

                        if (tempMaxValue > sTarget.mainSlider.minValue && tempMaxValue != sTarget.mainSlider.maxValue) 
                        {
                            sTarget.mainSlider.maxValue = tempMaxValue;
                            EditorUtility.SetDirty(sTarget.mainSlider);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                        else { tempMaxValue = sTarget.mainSlider.maxValue; }

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        bool tempWN = sTarget.mainSlider.wholeNumbers;
                        sTarget.mainSlider.wholeNumbers = GUILayout.Toggle(sTarget.mainSlider.wholeNumbers, new GUIContent("Use Whole Numbers"), customSkin.FindStyle("Toggle"));
                        sTarget.mainSlider.wholeNumbers = GUILayout.Toggle(sTarget.mainSlider.wholeNumbers, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                        if (tempWN != sTarget.mainSlider.wholeNumbers) { EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        bool tempInt = sTarget.mainSlider.interactable;
                        sTarget.mainSlider.interactable = GUILayout.Toggle(sTarget.mainSlider.interactable, new GUIContent("Is Interactable"), customSkin.FindStyle("Toggle"));
                        sTarget.mainSlider.interactable = GUILayout.Toggle(sTarget.mainSlider.interactable, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                        if (tempInt != sTarget.mainSlider.interactable) { EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); }

                        GUILayout.EndHorizontal();

                        if (Application.isPlaying == false) { sTarget.UpdateUI(); }
                    }

                    else { sTarget.mainSlider = sTarget.GetComponent<Slider>(); }

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(sliderObject, customSkin, "Slider Source");
                    if (showValue.boolValue == true) { HeatUIEditorHandler.DrawProperty(valueText, customSkin, "Label Text"); }
                    HeatUIEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    usePercent.boolValue = HeatUIEditorHandler.DrawToggle(usePercent.boolValue, customSkin, "Use Percent");
                    showValue.boolValue = HeatUIEditorHandler.DrawToggle(showValue.boolValue, customSkin, "Show Label");
                    showPopupValue.boolValue = HeatUIEditorHandler.DrawToggle(showPopupValue.boolValue, customSkin, "Show Popup Label");
                    useRoundValue.boolValue = HeatUIEditorHandler.DrawToggle(useRoundValue.boolValue, customSkin, "Use Round Value");
                    invokeOnAwake.boolValue = HeatUIEditorHandler.DrawToggle(invokeOnAwake.boolValue, customSkin, "Invoke On Awake", "Process events on awake.");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveValue.boolValue = HeatUIEditorHandler.DrawTogglePlain(saveValue.boolValue, customSkin, "Save Value");
                    GUILayout.Space(3);

                    if (saveValue.boolValue == true)
                    {
                        HeatUIEditorHandler.DrawPropertyCW(saveKey, customSkin, "Save Key:", 66);
                        EditorGUILayout.HelpBox("You must set a unique save key for each slider.", MessageType.Info);
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