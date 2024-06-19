using UnityEngine;

namespace UnityEditor.U2D.Aseprite
{
    internal class ImportSettings
    {
        const string k_BackgroundImportKey = UserSettings.settingsUniqueKey + "ImportSettings.backgroundImport";
        static readonly GUIContent k_BackgroundImportLabel = EditorGUIUtility.TrTextContent("Background import", "Enable asset import when the Unity Editor is in the background.");

        public static bool backgroundImport
        {
            get => EditorPrefs.GetBool(k_BackgroundImportKey, true);
            private set => EditorPrefs.SetBool(k_BackgroundImportKey, value);
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            var c = EditorGUILayout.Toggle(k_BackgroundImportLabel, backgroundImport);
            if (EditorGUI.EndChangeCheck())
                backgroundImport = c;
        }
    }

    internal class UserSettings : SettingsProvider
    {
        public const string settingsUniqueKey = "UnityEditor.U2D.Aseprite/";
        static readonly ImportSettings s_ImportSettings = new ImportSettings();

        UserSettings() : base("Preferences/2D/Aseprite Importer", SettingsScope.User)
        {
            guiHandler = OnGUI;
        }

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            return new UserSettings()
            {
                guiHandler = SettingsGUI
            };
        }

        static void SettingsGUI(string searchContext)
        {
            s_ImportSettings.OnGUI();
        }
    }
}
