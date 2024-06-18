#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(LocalizationSettings))]
    [System.Serializable]
    public class LocalizationSettingsEditor : Editor
    {
        private GUISkin customSkin;
        private LocalizationSettings lsTarget;

        private int defaultLanguageIndex;
        private List<string> languageList = new List<string>();

        protected static float foldoutItemSpace = 2;
        protected static float foldoutTopSpace = 5;
        protected static float foldoutBottomSpace = 2;

        private void OnEnable()
        {
            lsTarget = (LocalizationSettings)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }

            // Sort by language name
            lsTarget.languages.Sort(SortByName);

            // Refresh language dropdown
            RefreshLanguageDropdown();
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor variables are missing. You can manually fix this by deleting " +
                    "Reach UI > Resources folder and then re-import the package. \n\nIf you're still seeing this " +
                    "dialog even after the re-import, contact me with this ID: " + UIManager.buildID, MessageType.Error);
                return;
            }

            // Foldout style
            GUIStyle foldoutStyle = customSkin.FindStyle("UIM Foldout");

            // Settings Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 8);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Default Language"), customSkin.FindStyle("Text"), GUILayout.Width(120));

            if (languageList.Count != 0)
            {
                var defaultLanguageIndex = serializedObject.FindProperty("defaultLanguageIndex");
                var defaultLanguageID = serializedObject.FindProperty("defaultLanguageID");

                defaultLanguageIndex.intValue = EditorGUILayout.Popup(defaultLanguageIndex.intValue, languageList.ToArray());
                defaultLanguageID.stringValue = languageList[defaultLanguageIndex.intValue];
            }
            else { EditorGUILayout.HelpBox("There are no available languages.", MessageType.Info); }
            if (GUILayout.Button("Refresh List", customSkin.button)) { RefreshLanguageDropdown(); }

            GUILayout.Space(1);
            GUILayout.EndVertical();

            #region Available Languages
            // Available Languages Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Languages Header", 14);
            GUILayout.Space(-3);

            // Draw languages
            for (int i = 0; i < lsTarget.languages.Count; i++)
            {
                // Draw Action Buttons
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Delete", customSkin.button, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Delete Language", "Are you sure you want to delete the following language: " + lsTarget.languages[i].languageName + " (" + lsTarget.languages[i].languageID + ")"
                        + "\n\nThis action deletes the specified language resources and cannot be undone.", "Yes", "Cancel"))
                    {
                        if (lsTarget.languages[i].localizationLanguage != null) { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(lsTarget.languages[i].localizationLanguage)); }
                        lsTarget.languages.Remove(lsTarget.languages[i]);
                        RefreshLanguageDropdown();
                        EditorUtility.SetDirty(lsTarget);
                        return;
                    }
                }

                GUILayout.Space(5);
                GUILayout.EndHorizontal();

                // Start language item
                GUILayout.Space(-29);
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                if (string.IsNullOrEmpty(lsTarget.languages[i].languageName) == true) { lsTarget.languages[i].isExpanded = EditorGUILayout.Foldout(lsTarget.languages[i].isExpanded, "Language #" + i.ToString(), true, foldoutStyle); }
                else { lsTarget.languages[i].isExpanded = EditorGUILayout.Foldout(lsTarget.languages[i].isExpanded, lsTarget.languages[i].languageName + " (" + lsTarget.languages[i].languageID + ")", true, foldoutStyle); }
                lsTarget.languages[i].isExpanded = GUILayout.Toggle(lsTarget.languages[i].isExpanded, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                GUILayout.EndHorizontal();

                // Start language content
                if (lsTarget.languages[i].isExpanded)
                {
                    lsTarget.languages[i].languageID = EditorGUILayout.TextField("Language ID", lsTarget.languages[i].languageID);
                    lsTarget.languages[i].languageName = EditorGUILayout.TextField("Language Name", lsTarget.languages[i].languageName);
                    lsTarget.languages[i].localizedName = EditorGUILayout.TextField("Localized Name", lsTarget.languages[i].localizedName);
                    lsTarget.languages[i].localizationLanguage = EditorGUILayout.ObjectField("Language Asset", lsTarget.languages[i].localizationLanguage, typeof(LocalizationLanguage), true) as LocalizationLanguage;

                    if (lsTarget.languages[i].localizationLanguage == null && GUILayout.Button("Create Language Asset", customSkin.button))
                    {
                        LocalizationLanguage newLocale = ScriptableObject.CreateInstance<LocalizationLanguage>();

                        // Check for the path
                        string path = AssetDatabase.GetAssetPath(lsTarget);
                        path = path.Replace("/" + lsTarget.name + ".asset", "").Trim();
                        string fullPath = path.Replace("/" + lsTarget.name + ".asset", "").Trim() + "/Languages/";
                        if (!Directory.Exists(fullPath)) { AssetDatabase.CreateFolder(path, "Languages"); }

                        // Create the new asset
                        AssetDatabase.CreateAsset(newLocale, fullPath + lsTarget.languages[i].languageName + ".asset");
                        AssetDatabase.SaveAssets();

                        newLocale.localizationSettings = lsTarget;
                        newLocale.languageID = lsTarget.languages[i].languageID;
                        newLocale.name = lsTarget.languages[i].languageName;
                        newLocale.localizedName = lsTarget.languages[i].localizedName;
                        lsTarget.languages[i].localizationLanguage = newLocale;

                        // Add all available tables
                        for (int x = 0; x < lsTarget.tables.Count; x++)
                        {
                            LocalizationLanguage.TableList newList = new LocalizationLanguage.TableList();
                            newList.table = lsTarget.tables[x].localizationTable;
                            lsTarget.languages[i].localizationLanguage.tableList.Add(newList);
                            EditorUtility.SetDirty(lsTarget.languages[i].localizationLanguage);

                            for (int y = 0; y < lsTarget.tables[x].localizationTable.tableContent.Count; y++)
                            {
                                LocalizationLanguage.TableContent newContent = new LocalizationLanguage.TableContent();
                                newContent.key = "Key #" + y;
                                newList.tableContent.Add(newContent);
                            }
                        }

                        // Change the s.o. file name
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(newLocale), lsTarget.languages[i].languageName + " (" + lsTarget.languages[i].languageID + ")");
                        newLocale.name = lsTarget.languages[i].languageName + " (" + lsTarget.languages[i].languageID + ")";
                        AssetDatabase.Refresh();

                        // Set dirty
                        EditorUtility.FocusProjectWindow();
                        EditorUtility.SetDirty(lsTarget);
                    }

                    else if (lsTarget.languages[i].localizationLanguage != null && GUILayout.Button("Show Language Asset", customSkin.button))
                    {
                        Selection.activeObject = lsTarget.languages[i].localizationLanguage;
                    }
                }

                // Set localization settings for the items
                if (lsTarget.languages[i].localizationLanguage != null)
                {
                    lsTarget.languages[i].localizationLanguage.localizationSettings = lsTarget;
                    lsTarget.languages[i].localizationLanguage.languageID = lsTarget.languages[i].localizationLanguage.localizationSettings.languages[i].languageID;
                    lsTarget.languages[i].localizationLanguage.languageName = lsTarget.languages[i].localizationLanguage.localizationSettings.languages[i].languageName;
                    lsTarget.languages[i].localizationLanguage.localizedName = lsTarget.languages[i].localizationLanguage.localizationSettings.languages[i].localizedName;
                }

                // End language item
                GUILayout.EndVertical();
                GUILayout.Space(-3);
            }

            GUILayout.Space(2);

            if (GUILayout.Button("+ New Language", customSkin.button))
            {
                LocalizationSettings.Language language = new LocalizationSettings.Language();
                language.languageID = "null";
                language.languageName = "New Language";
                language.isExpanded = false;
                lsTarget.languages.Add(language);

                // Refresh language list
                RefreshLanguageDropdown();

                EditorUtility.SetDirty(lsTarget);
            }
            #endregion

            #region Available Tables
            // Available Tables Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Tables Header", 14);
            GUILayout.Space(-3);

            // Draw tables
            for (int i = 0; i < lsTarget.tables.Count; i++)
            {
                // Draw Action Buttons
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (lsTarget.tables[i].localizationTable != null && GUILayout.Button("Edit", customSkin.button, GUILayout.Width(34)))
                {
                    LocalizationTableWindow.ShowWindow(lsTarget, lsTarget.tables[i].localizationTable, i);
                }

                if (GUILayout.Button("Delete", customSkin.button, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Delete Table", "Are you sure you want to delete the following table: " + lsTarget.tables[i].tableID
                       + "\n\nThis action deletes the specified table resources and cannot be undone.", "Yes", "Cancel"))
                    {
                        // Clear empty languages first
                        for (int x = 0; x < lsTarget.languages.Count; x++)
                        {
                            for (int y = 0; y < lsTarget.languages[x].localizationLanguage.tableList.Count; y++)
                            {
                                if (lsTarget.languages[x].localizationLanguage.tableList[y].table == null)
                                {
                                    lsTarget.languages[x].localizationLanguage.tableList.RemoveAt(y);
                                }
                            }
                        }

                        // Delete from all existing langs if available
                        if (lsTarget.tables[i].localizationTable != null)
                        {
                            for (int x = 0; x < lsTarget.languages.Count; x++)
                            {
                                if (lsTarget.languages[x].localizationLanguage == null)
                                    continue;

                                for (int y = 0; y < lsTarget.languages[x].localizationLanguage.tableList.Count; y++)
                                {
                                    if (lsTarget.languages[x].localizationLanguage.tableList[y].table == lsTarget.tables[i].localizationTable)
                                    {
                                        lsTarget.languages[x].localizationLanguage.tableList.RemoveAt(y);
                                        EditorUtility.SetDirty(lsTarget.languages[x].localizationLanguage);
                                    }
                                }
                            }
                        }

                        if (lsTarget.tables[i].localizationTable != null) { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(lsTarget.tables[i].localizationTable)); }
                        lsTarget.tables.Remove(lsTarget.tables[i]);
                        EditorUtility.SetDirty(lsTarget);
                        return;
                    }
                }

                GUILayout.Space(5);
                GUILayout.EndHorizontal();

                // Start table item
                GUILayout.Space(-29);
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                if (string.IsNullOrEmpty(lsTarget.tables[i].tableID) == true) { lsTarget.tables[i].isExpanded = EditorGUILayout.Foldout(lsTarget.tables[i].isExpanded, "Table #" + i.ToString(), true, foldoutStyle); }
                else { lsTarget.tables[i].isExpanded = EditorGUILayout.Foldout(lsTarget.tables[i].isExpanded, lsTarget.tables[i].tableID, true, foldoutStyle); }
                lsTarget.tables[i].isExpanded = GUILayout.Toggle(lsTarget.tables[i].isExpanded, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                GUILayout.EndHorizontal();

                // Start table content
                if (lsTarget.tables[i].isExpanded)
                {
                    lsTarget.tables[i].tableID = EditorGUILayout.TextField("Table ID", lsTarget.tables[i].tableID);
                    lsTarget.tables[i].localizationTable = EditorGUILayout.ObjectField("Localization Table", lsTarget.tables[i].localizationTable, typeof(LocalizationTable), true) as LocalizationTable;

                    if (lsTarget.tables[i].localizationTable != null) { lsTarget.tables[i].localizationTable.tableID = lsTarget.tables[i].tableID; }
                    if (lsTarget.tables[i].localizationTable == null && GUILayout.Button("+ Create Table Asset", customSkin.button))
                    {
                        LocalizationTable newTable = ScriptableObject.CreateInstance<LocalizationTable>();

                        // Check for the path
                        string path = AssetDatabase.GetAssetPath(lsTarget);
                        path = path.Replace("/" + lsTarget.name + ".asset", "").Trim();
                        string fullPath = path.Replace("/" + lsTarget.name + ".asset", "").Trim() + "/Tables/";
                        if (!Directory.Exists(fullPath)) { AssetDatabase.CreateFolder(path, "Tables"); }

                        // Create the new asset
                        AssetDatabase.CreateAsset(newTable, fullPath + lsTarget.tables[i].tableID + ".asset");
                        AssetDatabase.SaveAssets();

                        // Change the s.o. values
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(newTable), lsTarget.tables[i].tableID);
                        newTable.name = lsTarget.tables[i].tableID;
                        newTable.tableID = lsTarget.tables[i].tableID;
                        newTable.localizationSettings = lsTarget;
                        lsTarget.tables[i].localizationTable = newTable;
                        AssetDatabase.Refresh();

                        // Add table to all available languages
                        for (int x = 0; x < lsTarget.languages.Count; x++)
                        {
                            if (lsTarget.languages[x].localizationLanguage == null)
                                continue;

                            LocalizationLanguage.TableList newList = new LocalizationLanguage.TableList();
                            newList.table = newTable;
                            lsTarget.languages[x].localizationLanguage.tableList.Add(newList);
                            EditorUtility.SetDirty(lsTarget.languages[x].localizationLanguage);
                        }

                        // Set dirty
                        EditorUtility.FocusProjectWindow();
                        EditorUtility.SetDirty(lsTarget);
                        EditorUtility.SetDirty(newTable);
                    }

                    else if (lsTarget.tables[i].localizationTable != null && GUILayout.Button("Show Table Asset", customSkin.button))
                    {
                        if (lsTarget.tables[i].localizationTable.localizationSettings == null) { lsTarget.tables[i].localizationTable.localizationSettings = lsTarget; }
                        Selection.activeObject = lsTarget.tables[i].localizationTable;
                    }
                }

                // End table item
                GUILayout.EndVertical();
                GUILayout.Space(-3);
            }

            GUILayout.Space(2);

            if (lsTarget.tables.Count == 0)
            {
                EditorGUILayout.HelpBox("There are no localization tables to show.", MessageType.Info);

                if (GUILayout.Button("+ New Table", customSkin.button))
                {
                    LocalizationSettings.Table table = new LocalizationSettings.Table();
                    table.tableID = "Table #" + lsTarget.tables.Count.ToString();
                    lsTarget.tables.Add(table);
                    EditorUtility.SetDirty(lsTarget);
                }
            }

            else if (lsTarget.tables[lsTarget.tables.Count - 1].localizationTable != null && GUILayout.Button("+ New Table", customSkin.button))
            {
                LocalizationSettings.Table table = new LocalizationSettings.Table();
                table.tableID = "Table #" + lsTarget.tables.Count.ToString();
                lsTarget.tables.Add(table);
                EditorUtility.SetDirty(lsTarget);
            }

            else if (lsTarget.tables[lsTarget.tables.Count - 1].localizationTable == null)
            {
                EditorGUILayout.HelpBox("In order to create a new table, you must first create a table asset for " + lsTarget.tables[lsTarget.tables.Count - 1].tableID + ".", MessageType.Warning);
            }
            #endregion

            #region Settings
            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 14);
            var enableExperimental = serializedObject.FindProperty("enableExperimental");
            enableExperimental.boolValue = HeatUIEditorHandler.DrawToggle(enableExperimental.boolValue, customSkin, "Enable Experimental Features");
            #endregion

            #region Support
            HeatUIEditorHandler.DrawHeader(customSkin, "Support Header", 14);
            EditorGUILayout.HelpBox("Stuck or just getting started? You can check out the documentation page.", MessageType.Info);
            if (GUILayout.Button("Documentation", customSkin.button)) { Docs(); }
            #endregion

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }

        private static int SortByName(LocalizationSettings.Language o1, LocalizationSettings.Language o2)
        {
            // Compare the names and sort by A to Z
            return o1.languageName.CompareTo(o2.languageName);
        }

        private void RefreshLanguageDropdown()
        {
            for (int i = 0; i < lsTarget.languages.Count; i++)
            {
                languageList.Add(lsTarget.languages[i].languageName + " (" + lsTarget.languages[i].languageID + ")");
                if (string.IsNullOrEmpty(lsTarget.defaultLanguageID) == false && languageList[i] == lsTarget.defaultLanguageID) { defaultLanguageIndex = i; }
            }

            if (string.IsNullOrEmpty(lsTarget.defaultLanguageID) == true && languageList.Count != 0) { lsTarget.defaultLanguageID = languageList[0]; }
        }

        void Docs() { Application.OpenURL("https://docs.michsky.com/docs/heat-ui/localization"); }
    }
}
#endif