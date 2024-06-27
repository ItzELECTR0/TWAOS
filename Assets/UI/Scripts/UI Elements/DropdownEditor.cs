#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(Dropdown))]
    public class DropdownEditor : Editor
    {
        private GUISkin customSkin;
        private Dropdown dTarget;
        private int currentTab;

        private void OnEnable()
        {
            dTarget = (Dropdown)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }

            if (dTarget.selectedItemIndex > dTarget.items.Count - 1) { dTarget.selectedItemIndex = 0; }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Dropdown Top Header");

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
            var onValueChanged = serializedObject.FindProperty("onValueChanged");

            var triggerObject = serializedObject.FindProperty("triggerObject");
            var headerText = serializedObject.FindProperty("headerText");
            var headerImage = serializedObject.FindProperty("headerImage");
            var itemParent = serializedObject.FindProperty("itemParent");
            var itemPreset = serializedObject.FindProperty("itemPreset");
            var scrollbar = serializedObject.FindProperty("scrollbar");
            var listParent = serializedObject.FindProperty("listParent");
            var listRect = serializedObject.FindProperty("listRect");
            var listCG = serializedObject.FindProperty("listCG");
            var contentCG = serializedObject.FindProperty("contentCG");
            var highlightCG = serializedObject.FindProperty("highlightCG");

            var panelDirection = serializedObject.FindProperty("panelDirection");
            var panelSize = serializedObject.FindProperty("panelSize");
            var curveSpeed = serializedObject.FindProperty("curveSpeed");
            var animationCurve = serializedObject.FindProperty("animationCurve");

            var saveSelected = serializedObject.FindProperty("saveSelected");
            var saveKey = serializedObject.FindProperty("saveKey");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableTrigger = serializedObject.FindProperty("enableTrigger");
            var enableScrollbar = serializedObject.FindProperty("enableScrollbar");
            var startAtBottom = serializedObject.FindProperty("startAtBottom");
            var setHighPriority = serializedObject.FindProperty("setHighPriority");
            var outOnPointerExit = serializedObject.FindProperty("outOnPointerExit");
            var invokeOnAwake = serializedObject.FindProperty("invokeOnAwake");
            var initOnAwake = serializedObject.FindProperty("initOnAwake");
            var selectedItemIndex = serializedObject.FindProperty("selectedItemIndex");
            var useSounds = serializedObject.FindProperty("useSounds");
            var itemSpacing = serializedObject.FindProperty("itemSpacing");
            var itemPaddingLeft = serializedObject.FindProperty("itemPaddingLeft");
            var itemPaddingRight = serializedObject.FindProperty("itemPaddingRight");
            var itemPaddingTop = serializedObject.FindProperty("itemPaddingTop");
            var itemPaddingBottom = serializedObject.FindProperty("itemPaddingBottom");
           
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var navigationMode = serializedObject.FindProperty("navigationMode");
            var selectOnUp = serializedObject.FindProperty("selectOnUp");
            var selectOnDown = serializedObject.FindProperty("selectOnDown");
            var selectOnLeft = serializedObject.FindProperty("selectOnLeft");
            var selectOnRight = serializedObject.FindProperty("selectOnRight");
            var wrapAround = serializedObject.FindProperty("wrapAround");

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (Application.isPlaying == false && dTarget.items.Count != 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();

                        GUI.enabled = false;
                        EditorGUILayout.LabelField(new GUIContent("Selected Item:"), customSkin.FindStyle("Text"), GUILayout.Width(82));
                        GUI.enabled = true;

                        EditorGUILayout.LabelField(new GUIContent(dTarget.items[selectedItemIndex.intValue].itemName), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        selectedItemIndex.intValue = EditorGUILayout.IntSlider(selectedItemIndex.intValue, 0, dTarget.items.Count - 1);

                        GUILayout.EndVertical();
                    }

                    else if (Application.isPlaying == true && dTarget.items.Count != 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        GUI.enabled = false;

                        EditorGUILayout.LabelField(new GUIContent("Current Item:"), customSkin.FindStyle("Text"), GUILayout.Width(74));
                        EditorGUILayout.LabelField(new GUIContent(dTarget.items[dTarget.selectedItemIndex].itemName), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        EditorGUILayout.IntSlider(dTarget.index, 0, dTarget.items.Count - 1);

                        GUI.enabled = true;
                        GUILayout.EndVertical();
                    }

                    else { EditorGUILayout.HelpBox("There is no item in the list.", MessageType.Warning); }

                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(items, new GUIContent("Dropdown Items"), true);          
                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    if (Application.isPlaying == false && dTarget.contentCG != null)
                    {
                        if (dTarget.contentCG.alpha == 0 && GUILayout.Button("Show Content Preview", customSkin.button)) { dTarget.contentCG.alpha = 1; }
                        else if (dTarget.contentCG.alpha == 1 && GUILayout.Button("Disable Content Preview", customSkin.button)) { dTarget.contentCG.alpha = 0; }
                    }

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(triggerObject, customSkin, "Trigger Object");
                    HeatUIEditorHandler.DrawProperty(headerText, customSkin, "Header Text");
                    HeatUIEditorHandler.DrawProperty(headerImage, customSkin, "Header Image");
                    HeatUIEditorHandler.DrawProperty(itemPreset, customSkin, "Item Preset");
                    HeatUIEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    HeatUIEditorHandler.DrawProperty(scrollbar, customSkin, "Scrollbar");
                    HeatUIEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    HeatUIEditorHandler.DrawProperty(contentCG, customSkin, "Content CG");
                    HeatUIEditorHandler.DrawProperty(listCG, customSkin, "List CG");
                    HeatUIEditorHandler.DrawProperty(listRect, customSkin, "List Rect");
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Customization Header", 6);
                    enableIcon.boolValue = HeatUIEditorHandler.DrawToggle(enableIcon.boolValue, customSkin, "Enable Header Icon");

                    if (dTarget.headerImage != null)
                    {
                        if (enableIcon.boolValue == true) { dTarget.headerImage.gameObject.SetActive(true); }
                        else { dTarget.headerImage.gameObject.SetActive(false); }
                    }

                    else if (enableIcon.boolValue == true)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("'Header Image' is missing from the resources.", MessageType.Warning);
                        GUILayout.EndHorizontal();
                    }

                    enableScrollbar.boolValue = HeatUIEditorHandler.DrawToggle(enableScrollbar.boolValue, customSkin, "Enable Scrollbar");

                    if (dTarget.scrollbar != null)
                    {
                        if (enableScrollbar.boolValue == true) { dTarget.scrollbar.gameObject.SetActive(true); }
                        else { dTarget.scrollbar.gameObject.SetActive(false); }
                    }

                    else if (enableScrollbar.boolValue == true)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("'Scrollbar' is missing from the resources.", MessageType.Warning);
                        GUILayout.EndHorizontal();
                    }

                    startAtBottom.boolValue = HeatUIEditorHandler.DrawToggle(startAtBottom.boolValue, customSkin, "Start At Bottom");

                    HeatUIEditorHandler.DrawPropertyCW(itemSpacing, customSkin, "Item Spacing", 90);

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Item Padding"), customSkin.FindStyle("Text"), GUILayout.Width(90));
                    GUILayout.EndHorizontal();
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(itemPaddingTop, new GUIContent("Top"));
                    EditorGUILayout.PropertyField(itemPaddingBottom, new GUIContent("Bottom"));
                    EditorGUILayout.PropertyField(itemPaddingLeft, new GUIContent("Left"));
                    EditorGUILayout.PropertyField(itemPaddingRight, new GUIContent("Right"));
                    dTarget.UpdateItemLayout();

                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    HeatUIEditorHandler.DrawHeader(customSkin, "Animation Header", 10);
                    HeatUIEditorHandler.DrawProperty(panelDirection, customSkin, "Panel Direction");
                    HeatUIEditorHandler.DrawProperty(panelSize, customSkin, "Panel Size");
                    HeatUIEditorHandler.DrawProperty(curveSpeed, customSkin, "Curve Speed");
                    HeatUIEditorHandler.DrawProperty(animationCurve, customSkin, "Animation Curve");

                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 10);
                    initOnAwake.boolValue = HeatUIEditorHandler.DrawToggle(initOnAwake.boolValue, customSkin, "Initialize On Awake");
                    invokeOnAwake.boolValue = HeatUIEditorHandler.DrawToggle(invokeOnAwake.boolValue, customSkin, "Invoke On Awake", "Process events on awake.");

                    enableTrigger.boolValue = HeatUIEditorHandler.DrawToggle(enableTrigger.boolValue, customSkin, "Enable Trigger", "Clicking outside will close the dropdown.");
                    if (enableTrigger.boolValue == true && dTarget.triggerObject == null) { EditorGUILayout.HelpBox("'Trigger Object' is missing from the resources.", MessageType.Warning); }

                    setHighPriority.boolValue = HeatUIEditorHandler.DrawToggle(setHighPriority.boolValue, customSkin, "Set High Priority");
                    if (setHighPriority.boolValue == true) { EditorGUILayout.HelpBox("Set High Priority; renders the content above all objects when the dropdown is open.", MessageType.Info); }

                    outOnPointerExit.boolValue = HeatUIEditorHandler.DrawToggle(outOnPointerExit.boolValue, customSkin, "Out On Pointer Exit", "Close the dropdown on pointer exit.");
                    useSounds.boolValue = HeatUIEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Sounds");
                 
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveSelected.boolValue = HeatUIEditorHandler.DrawTogglePlain(saveSelected.boolValue, customSkin, "Save Selected");                 
                    GUILayout.Space(3);

                    if (saveSelected.boolValue == true)
                    {
                        HeatUIEditorHandler.DrawPropertyPlainCW(saveKey, customSkin, "Save Key:", 66);
                        EditorGUILayout.HelpBox("You must set a unique save key for each dropdown.", MessageType.Info);
                    }

                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    useUINavigation.boolValue = HeatUIEditorHandler.DrawTogglePlain(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

                    GUILayout.Space(4);

                    if (useUINavigation.boolValue == true)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        HeatUIEditorHandler.DrawPropertyPlain(navigationMode, customSkin, "Navigation Mode");

                        if (dTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Horizontal)
                        {
                            EditorGUI.indentLevel = 1;
                            wrapAround.boolValue = HeatUIEditorHandler.DrawToggle(wrapAround.boolValue, customSkin, "Wrap Around");
                            EditorGUI.indentLevel = 0;
                        }

                        else if (dTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Vertical)
                        {
                            wrapAround.boolValue = HeatUIEditorHandler.DrawTogglePlain(wrapAround.boolValue, customSkin, "Wrap Around");
                        }

                        else if (dTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Explicit)
                        {
                            EditorGUI.indentLevel = 1;
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnUp, customSkin, "Select On Up");
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnDown, customSkin, "Select On Down");
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnLeft, customSkin, "Select On Left");
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnRight, customSkin, "Select On Right");
                            EditorGUI.indentLevel = 0;
                        }

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif