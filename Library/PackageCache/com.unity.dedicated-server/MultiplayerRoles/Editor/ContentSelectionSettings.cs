using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using InternalManager = UnityEditor.Multiplayer.Internal.EditorMultiplayerManager;
using InternalMultiplayerRole = UnityEngine.Multiplayer.Internal.MultiplayerRole;

namespace Unity.Multiplayer.Editor
{
    [FilePath("ProjectSettings/Packages/com.unity.dedicated-server/ContentSelectionSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class ContentSelectionSettings : SyncedSingleton<ContentSelectionSettings>
    {
        private class SaveAssetsProcessor : AssetModificationProcessor
        {
            static string[] OnWillSaveAssets(string[] paths)
            {
                ContentSelectionSettings.instance.SaveIfDirty();
                return paths;
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            InitializeSettings();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            CleanupRestrictedComponents();
        }

        private static void InitializeSettings()
        {
            if (instance.m_SettingsInitialized)
                return;

            instance.m_SettingsInitialized = true;
            InternalManager.enableMultiplayerRoles = true;
            EditorUtility.SetDirty(instance);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // TODO: We have seen cases where the state is ExitingEditMode but isPlaying (or maybe isPlayingOrEnteringPlayMode) is still true.
            //       That needs to be investigated and fixed.
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                instance.SaveToInternalMultiplayerManager();
            }
        }

        private static void CleanupRestrictedComponents()
        {
            var customList = AutomaticSelection.GetCustomComponents();
            var types = customList.Keys;
            var count = types.Count;
            var reasign = false;

            for (var i = 0; i < count; i++)
            {
                var type = types.ElementAt(i);
                if (type == null || type.GetCustomAttribute<MultiplayerRoleRestrictedAttribute>() == null)
                    continue;

                customList.Remove(type);
                reasign = true;
            }

            if (reasign)
                AutomaticSelection.SetCustomComponents(customList);
        }

        [SerializeField] private bool m_SettingsInitialized;
        [SerializeField] private bool m_EnableSafetyChecks = true;
        [SerializeField] private AutomaticSelectionOptions m_AutomaticSelectionOptions;

        public static bool EnableSafetyChecks
        {
            get => instance.m_EnableSafetyChecks;
            set => instance.m_EnableSafetyChecks = value;
        }

        public static ref AutomaticSelectionOptions AutomaticSelection => ref ContentSelectionSettings.instance.m_AutomaticSelectionOptions;

        internal void SaveIfDirty()
        {
            if (EditorUtility.IsDirty(this))
            {
                Save(true);
                SaveToInternalMultiplayerManager();
            }
        }

        private void SaveToInternalMultiplayerManager()
        {
            var strippingComponentsPerRole = new Dictionary<MultiplayerRole, HashSet<Type>>();
            var roleValues = Enum.GetValues(typeof(MultiplayerRole));
            foreach (MultiplayerRole role in roleValues)
                strippingComponentsPerRole[role] = new();

            foreach (var component in AutomaticSelection.CompleteComponentsList)
            {
                foreach (MultiplayerRole role in roleValues)
                {
                    if ((component.Value & (MultiplayerRoleFlags)(1 << (int)role)) == 0)
                        strippingComponentsPerRole[role].Add(component.Key);
                }
            }

            foreach (MultiplayerRole role in roleValues)
            {
                InternalManager.SetStrippingTypesForRole((InternalMultiplayerRole)role, strippingComponentsPerRole[role].ToArray());
            }
        }
    }
}
