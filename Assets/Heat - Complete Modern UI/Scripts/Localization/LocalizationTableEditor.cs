#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(LocalizationTable))]
    public class LocalizationTableEditor : Editor
    {
        private GUISkin customSkin;
        private LocalizationTable ltTarget;

        private void OnEnable()
        {
            ltTarget = (LocalizationTable)target;

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

            // Settings Header
            HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 8);
            GUI.enabled = false;

            var tableID = serializedObject.FindProperty("tableID");
            HeatUIEditorHandler.DrawProperty(tableID, customSkin, "Table ID");

            var localizationSettings = serializedObject.FindProperty("localizationSettings");
            HeatUIEditorHandler.DrawProperty(localizationSettings, customSkin, "Localization Settings");

            GUI.enabled = true;

            if (ltTarget.localizationSettings != null && ltTarget.localizationSettings.languages.Count != 0 && GUILayout.Button("Edit Table", customSkin.button))
            {
                for (int i = 0; i < ltTarget.localizationSettings.languages[0].localizationLanguage.tableList.Count; i++)
                {
                    if (ltTarget.localizationSettings.languages[0].localizationLanguage.tableList[i].table == ltTarget)
                        LocalizationTableWindow.ShowWindow(ltTarget.localizationSettings, ltTarget, i);
                }
            }
        }
    }
}
#endif