#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(LocalizedObject))]
    public class LocalizedObjectEditor : Editor
    {
        private LocalizedObject loTarget;
        private GUISkin customSkin;
        private int currentTab;

        private List<string> tableList = new List<string>();
        private string searchString;
        private string tempValue;
        Vector2 scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            loTarget = (LocalizedObject)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }

            if (loTarget.localizationSettings == null) 
            {
#if UNITY_2023_2_OR_NEWER
                try { loTarget.localizationManager = FindObjectsByType<LocalizationManager>(FindObjectsSortMode.None)[0]; } catch { }
#else
                try { loTarget.localizationManager = (LocalizationManager)GameObject.FindObjectsOfType(typeof(LocalizationManager))[0]; } catch { }
#endif
                if (loTarget.localizationManager != null && loTarget.localizationManager.UIManagerAsset != null && loTarget.localizationManager.UIManagerAsset.localizationSettings != null)
                {
                    loTarget.localizationSettings = loTarget.localizationManager.UIManagerAsset.localizationSettings;
                }
            }

            // Update language settings if it's driven by the manager
            else if (loTarget.localizationManager != null && loTarget.localizationManager.UIManagerAsset != null && loTarget.localizationManager.UIManagerAsset.localizationSettings != null)
            {
                loTarget.localizationSettings = loTarget.localizationManager.UIManagerAsset.localizationSettings;
            }

            RefreshTableDropdown();
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Localization Top Header");

            GUIContent[] toolbarTabs = new GUIContent[2];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Settings");

            currentTab = HeatUIEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 1;

            GUILayout.EndHorizontal();

            var localizationManager = serializedObject.FindProperty("localizationManager");
            var localizationSettings = serializedObject.FindProperty("localizationSettings");
            var tableIndex = serializedObject.FindProperty("tableIndex");
            var objectType = serializedObject.FindProperty("objectType");
            var onLanguageChanged = serializedObject.FindProperty("onLanguageChanged");
            var rebuildLayoutOnUpdate = serializedObject.FindProperty("rebuildLayoutOnUpdate");
            var forceAddToManager = serializedObject.FindProperty("forceAddToManager");
            var updateMode = serializedObject.FindProperty("updateMode");
            var textObj = serializedObject.FindProperty("textObj");
            var localizationKey = serializedObject.FindProperty("localizationKey");

            if (loTarget.localizationManager != null && loTarget.localizationManager.UIManagerAsset != null && loTarget.localizationManager.UIManagerAsset.enableLocalization == false)
            {
                EditorGUILayout.HelpBox("Localization is disabled.", MessageType.Warning);
                return;
            }

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    HeatUIEditorHandler.DrawProperty(localizationManager, customSkin, "Manager");
                    if (loTarget.localizationManager != null) { GUI.enabled = false; }
                    HeatUIEditorHandler.DrawProperty(localizationSettings, customSkin, "Settings");
                    GUI.enabled = true;

                    if (loTarget.localizationSettings == null)
                    {
                        EditorGUILayout.HelpBox("Localization Settings is missing. You can assign a valid 'Localization Manager' " +
                            "or directly assign a localization settings asset.", MessageType.Warning);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    HeatUIEditorHandler.DrawProperty(updateMode, customSkin, "Update Mode");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Object Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                    EditorGUILayout.PropertyField(objectType, new GUIContent(""));
                    GUILayout.EndHorizontal();
                   
                    if (loTarget.objectType == LocalizedObject.ObjectType.TextMeshPro) 
                    {
                        HeatUIEditorHandler.DrawProperty(textObj, customSkin, "Text Object");
                        if (Application.isPlaying == false
                            && loTarget.showOutputOnEditor == true
                            && string.IsNullOrEmpty(tempValue) == false
                            && loTarget.textObj != null 
                            && GUILayout.Button(new GUIContent("Update Text"), customSkin.button)) 
                        { 
                            loTarget.textObj.text = tempValue;
                            loTarget.textObj.enabled = false;
                            loTarget.textObj.enabled = true;
                        }
                    }

                    GUILayout.EndVertical();

                    if (loTarget.localizationSettings.tables.Count != 0 && loTarget.tableIndex != -1)
                    {
                        HeatUIEditorHandler.DrawHeader(customSkin, "Tables Header", 10);

                        // Selected table
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Selected Table"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        tableIndex.intValue = EditorGUILayout.Popup(loTarget.tableIndex, tableList.ToArray());
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button(new GUIContent("Edit Table"), customSkin.button))
                        {
                            LocalizationTableWindow.ShowWindow(loTarget.localizationSettings, loTarget.localizationSettings.tables[loTarget.tableIndex].localizationTable, loTarget.tableIndex);
                        }

                        if (loTarget.objectType != LocalizedObject.ObjectType.ComponentDriven)
                        {
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(new GUIContent("Localization Key"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                            EditorGUILayout.PropertyField(localizationKey, new GUIContent(""));
                            loTarget.showOutputOnEditor = GUILayout.Toggle(loTarget.showOutputOnEditor, new GUIContent("", "See output"), GUILayout.Width(15), GUILayout.Height(18));
                            GUILayout.EndHorizontal();

                            // Search for keys
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(new GUIContent("Search for keys in " + loTarget.localizationSettings.tables[loTarget.tableIndex].tableID), customSkin.FindStyle("Text"));

                            GUILayout.BeginHorizontal();
                            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSearchTextField"));
                            if (!string.IsNullOrEmpty(searchString) && GUILayout.Button(new GUIContent("", "Clear search bar"), GUI.skin.FindStyle("ToolbarSearchCancelButton"))) { searchString = ""; GUI.FocusControl(null); }
                            GUILayout.EndHorizontal();

                            if (!string.IsNullOrEmpty(searchString))
                            {
                                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(132));
                                GUILayout.BeginVertical();

                                for (int i = 0; i < loTarget.localizationSettings.languages[0].localizationLanguage.tableList[loTarget.tableIndex].tableContent.Count; i++)
                                {
                                    if (loTarget.localizationSettings.languages[0].localizationLanguage.tableList[loTarget.tableIndex].tableContent[i].key.ToLower().Contains(searchString.ToLower()))
                                    {
                                        if (GUILayout.Button(new GUIContent(loTarget.localizationSettings.languages[0].localizationLanguage.tableList[loTarget.tableIndex].tableContent[i].key), customSkin.button))
                                        {
                                            loTarget.localizationKey = loTarget.localizationSettings.languages[0].localizationLanguage.tableList[loTarget.tableIndex].tableContent[i].key;
                                            searchString = "";
                                            GUI.FocusControl(null);
                                            EditorUtility.SetDirty(loTarget);
                                        }
                                    }
                                }

                                GUILayout.EndVertical();
                                GUILayout.EndScrollView();
                            }

                            GUILayout.EndVertical();

                            if (loTarget.showOutputOnEditor == true)
                            {
                                GUI.enabled = false;
                                for (int i = 0; i < loTarget.localizationSettings.languages.Count; i++)
                                {
                                    for (int x = 0; x < loTarget.localizationSettings.languages[i].localizationLanguage.tableList[loTarget.tableIndex].tableContent.Count; x++)
                                    {
                                        if (loTarget.localizationSettings.languages[i].localizationLanguage.tableList[loTarget.tableIndex].tableContent[x].key == loTarget.localizationKey)
                                        {
                                            GUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField(new GUIContent("[" + loTarget.localizationSettings.languages[i].languageID + "] " +
                                                loTarget.localizationSettings.languages[i].localizationLanguage.tableList[loTarget.tableIndex].tableContent[x].value), customSkin.FindStyle("Text"));
                                            GUILayout.EndHorizontal();

                                            // Used for Update Text button
                                            tempValue = loTarget.localizationSettings.languages[loTarget.localizationSettings.defaultLanguageIndex].localizationLanguage.tableList[loTarget.tableIndex].tableContent[x].value;
                                        }
                                    }
                                }
                                GUI.enabled = true;
                            }
                        }

                        GUILayout.EndVertical();
                    }
                    else if (loTarget.localizationSettings.tables.Count != 0 && loTarget.tableIndex == -1) { RefreshTableDropdown(); }

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onLanguageChanged, new GUIContent("On Language Changed"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    rebuildLayoutOnUpdate.boolValue = HeatUIEditorHandler.DrawToggle(rebuildLayoutOnUpdate.boolValue, customSkin, "Rebuild Layout On Update", "Force to rebuild layout on item update to prevent visual glitches.");
                    forceAddToManager.boolValue = HeatUIEditorHandler.DrawToggle(forceAddToManager.boolValue, customSkin, "Force Add To Manager", "Force to add this component to the manager on awake.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }

        private void RefreshTableDropdown()
        {
            if (loTarget.localizationSettings == null)
                return;

            for (int i = 0; i < loTarget.localizationSettings.tables.Count; i++)
            {
                if (loTarget.localizationSettings.tables[i].localizationTable != null)
                {
                    tableList.Add(loTarget.localizationSettings.tables[i].tableID);
                }
            }

            if (loTarget.localizationSettings.tables.Count == 0) { loTarget.tableIndex = -1; }
            else if (loTarget.tableIndex > loTarget.localizationSettings.tables.Count - 1) { loTarget.tableIndex = 0; }
            else if (loTarget.tableIndex == -1 && loTarget.localizationSettings.tables.Count != 0) { loTarget.tableIndex = 0; }
        }
    }
}
#endif