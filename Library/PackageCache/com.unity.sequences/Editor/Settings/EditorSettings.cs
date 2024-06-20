using UnityEditor.SettingsManagement;

namespace UnityEditor.Sequences
{
    static class EditorSettings
    {
        const string k_PackageName = "com.unity.sequences";
        static Settings s_Instance;

        [InitializeOnLoadMethod]
        static void SaveSettingsOnExit()
        {
            EditorApplication.quitting += Save;
        }

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            var provider = new UserSettingsProvider("Project/Sequences",
                instance,
                new[] { typeof(EditorSettings).Assembly },
                SettingsScope.Project);

            return provider;
        }

        internal static Settings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new Settings(k_PackageName);
                }

                return s_Instance;
            }
        }

        public static void Save()
        {
            instance.Save();
        }

        public static void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            instance.Set<T>(key, value, scope);
        }

        public static T Get<T>(string key, SettingsScope scope = SettingsScope.Project, T fallback = default(T))
        {
            return instance.Get<T>(key, scope, fallback);
        }

        public static bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            return instance.ContainsKey<T>(key, scope);
        }

        public static void Delete<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            instance.DeleteKey<T>(key, scope);
        }
    }
}
