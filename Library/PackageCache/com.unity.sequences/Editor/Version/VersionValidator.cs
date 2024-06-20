namespace UnityEditor.Sequences
{
    [InitializeOnLoad]
    internal class VersionValidator
    {
        static EditorPref<SemanticVersion> s_StoredVersionInfo = new EditorPref<SemanticVersion>("about.identifier", new SemanticVersion(), SettingsScope.Project);

        internal static SemanticVersion storedVersion
        {
            get { return s_StoredVersionInfo.value; }
        }

        static VersionValidator()
        {
            // Do not run this if we are in a secondary process such as the Standalone Profiler process.
            if (!PackageUtility.IsRunningInMainEditorInstance())
                return;

            EditorApplication.delayCall += ValidateVersion;
        }

        static void ValidateVersion()
        {
            SemanticVersion currentVersion;

            if (!PackageUtility.GetPackageVersion(out currentVersion))
                return;

            var oldVersion = (SemanticVersion)s_StoredVersionInfo;
            bool isNewVersion = currentVersion != oldVersion;

            if (isNewVersion)
            {
                s_StoredVersionInfo.SetValue(currentVersion, true);

                UpgradeProject(oldVersion, currentVersion);
            }
        }

        static void UpgradeProject(SemanticVersion from, SemanticVersion to)
        {
            // Implement what would be needed.
        }
    }
}
