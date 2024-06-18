#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(LocalizationLanguage))]
    [System.Serializable]
    public class LocalizationLanguageEditor : Editor
    {
        private LocalizationLanguage lTarget;
        private GUISkin customSkin;
        private LocalizationLanguage.TableList tempTable;

        private void OnEnable()
        {
            lTarget = (LocalizationLanguage)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor variables are missing. You can manually fix this by deleting " +
                    "Heat UI > Resources folder and then re-import the package. \n\nIf you're still seeing this " +
                    "dialog even after the re-import, contact me with this ID: " + UIManager.buildID, MessageType.Error);
                return;
            }

            // Info Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 8);

            var localizationSettings = serializedObject.FindProperty("localizationSettings");
            var languageID = serializedObject.FindProperty("languageID");
            var languageName = serializedObject.FindProperty("languageName");
            var localizedName = serializedObject.FindProperty("localizedName");
            var tableList = serializedObject.FindProperty("tableList");

            GUI.enabled = false;
            HeatUIEditorHandler.DrawProperty(languageID, customSkin, "Language ID");
            HeatUIEditorHandler.DrawProperty(languageName, customSkin, "Language Name");
            HeatUIEditorHandler.DrawProperty(localizedName, customSkin, "Localized Name");
            GUI.enabled = true;

            // Settings Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 14);
            HeatUIEditorHandler.DrawPropertyCW(localizationSettings, customSkin, "Localization Settings", 130);

            if (localizationSettings != null && GUILayout.Button("Show Localization Settings", customSkin.button)) 
            { 
                Selection.activeObject = localizationSettings.objectReferenceValue; 
            }

            // Content Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Tables Header", 14);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import Table", customSkin.button)) { Import(); }
            if (GUILayout.Button("Export Table(s)", customSkin.button)) { Export(); }
            GUILayout.EndHorizontal();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(tableList, new GUIContent("Table List (Debug Only)"), true);

            serializedObject.ApplyModifiedProperties();
        }

        void Import()
        {
            string path = EditorUtility.OpenFilePanel("Select a file to import", "", "");
           
            if (path.Length != 0)
            {
                string tempKey = null;
                bool checkForValue = false;

                foreach (string option in File.ReadLines(path))
                {
                    if (option.Contains("[LanguageID] "))
                    {
                        string tempLangID = option.Replace("[LanguageID] ", "");
                        checkForValue = false;

                        if (tempLangID != lTarget.languageID) 
                        {
                            Debug.LogError("The language ID does not match with the language asset."); 
                            break;
                        }
                    }

                    if (option.Contains("[TableID] "))
                    {
                        string tempTableID = option.Replace("[TableID] ", "");
                        checkForValue = false;

                        for (int i = 0; i < lTarget.tableList.Count; i++)
                        {
                            if (lTarget.tableList[i].table.tableID == tempTableID)
                            {
                                tempTable = lTarget.tableList[i];
                                break;
                            }
                        }
                    }

                    else if (option.Contains("[StringKey] ")) 
                    { 
                        tempKey = option.Replace("[StringKey] ", "");
                        checkForValue = false;
                    }

                    else if (option.Contains("[Value] "))
                    {
                        if (tempTable == null) { Debug.LogError("Can't find the given table ID."); break; }
                        for (int i = 0; i < tempTable.tableContent.Count; i++)
                        {
                            if (tempTable.tableContent[i].key == tempKey)
                            {
                                string tempValue = option.Replace("[Value] ", "");
                                tempTable.tableContent[i].value = tempValue;
                                continue;
                            }
                        }

                        checkForValue = true;
                    }

                    else if (checkForValue == true && !option.Contains("[Value] ") && !string.IsNullOrEmpty(option))
                    {
                        checkForValue = false;

                        if (tempTable == null) { Debug.LogError("Can't find the given table ID."); break; }
                        for (int i = 0; i < tempTable.tableContent.Count; i++)
                        {
                            if (tempTable.tableContent[i].key == tempKey)
                            {
                                tempTable.tableContent[i].value = tempTable.tableContent[i].value + "\n" + option;
                                continue;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tempKey) && tempTable != null) { Debug.Log(tempTable.table.tableID + " (table) has been successfully imported to " + lTarget.languageID); }
            }
        }

        void Export()
        {
            for (int i = 0; i < lTarget.tableList.Count; i++)
            {
                string path = EditorUtility.SaveFilePanel("Export: " + lTarget.tableList[i].table.tableID, "", "Exported_" + lTarget.tableList[i].table.tableID + "(" + lTarget.languageID + ")", "txt");

                if (path.Length != 0)
                {
                    TextWriter tw = new StreamWriter(path, false);
                    tw.WriteLine("[LanguageID] " + lTarget.languageID);
                    tw.WriteLine("[TableID] " + lTarget.tableList[i].table.tableID);
                    tw.WriteLine("\n------------------------------");

                    for (int x = 0; x < lTarget.tableList[i].table.tableContent.Count; x++)
                    {
                        tw.WriteLine("\n[StringKey] " + lTarget.tableList[i].table.tableContent[x].key);
                        tw.Write("[Value] " + lTarget.tableList[i].tableContent[x].value + "\n");
                    }

                    tw.Close();
                    Debug.Log(lTarget.tableList[i].table.tableID + " has been exported to: " + path);
                }
            }
        }
    }
}
#endif