using System;

namespace Unity.Multiplayer.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ProjectSettingsSectionAttribute : Attribute
    {
        public string Label = null;
        public string SettingsPath = ProjectSettingsProvider.k_SettingsGroupPath;

        public ProjectSettingsSectionAttribute() { }
    }
}
