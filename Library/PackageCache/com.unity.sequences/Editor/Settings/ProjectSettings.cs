using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    static class ProjectSettings
    {
        static readonly string k_DefaultMasterSequenceRegistryPath =
            $"Assets/{SequencesAssetDatabase.k_SequenceBaseFolder}/MasterSequenceRegistry.asset";

        [UserSetting("Editorial Settings", "Default Master Sequence Registry")]
        static EditorPref<MasterSequenceRegistry> s_DefaultMasterSequenceRegistry =
            new EditorPref<MasterSequenceRegistry>("editor.defaultMasterSequenceRegistry", null, SettingsScope.Project);

        internal static MasterSequenceRegistry defaultMasterSequenceRegistry => s_DefaultMasterSequenceRegistry.value;

        internal static void SaveAssetSetting(Object setting)
        {
            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssetIfDirty(setting);
        }

        /// <summary>
        /// Create a a new MasterSequenceRegistry asset and assign it as the default asset to use.
        /// </summary>
        internal static void CreateDefaultMasterSequenceRegistryIfNeeded()
        {
            if (defaultMasterSequenceRegistry != null)
                return;

            var folderPath = $"Assets/{SequencesAssetDatabase.k_SequenceBaseFolder}";
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets", SequencesAssetDatabase.k_SequenceBaseFolder);

            var registry = ScriptableObject.CreateInstance<MasterSequenceRegistry>();
            AssetDatabase.CreateAsset(registry, k_DefaultMasterSequenceRegistryPath);

            s_DefaultMasterSequenceRegistry.SetValue(registry, true);
        }

#if UNITY_INCLUDE_TESTS
        internal static void SetDefaultMasterSequenceRegistry(MasterSequenceRegistry instance)
        {
            s_DefaultMasterSequenceRegistry.SetValue(instance, true);
        }

#endif
    }
}
