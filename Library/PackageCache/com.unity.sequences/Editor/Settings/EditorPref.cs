using UnityEditor.SettingsManagement;

namespace UnityEditor.Sequences
{
    sealed class EditorPref<T> : UserSetting<T>
    {
        public EditorPref(string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(EditorSettings.instance, key, value, scope)
        {}

        public EditorPref(Settings settings, string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(settings, key, value, scope) {}

        public static implicit operator T(EditorPref<T> pref)
        {
            return pref.value;
        }
    }
}
