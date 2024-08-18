using System;
using System.Collections.Generic;
using System.Globalization;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common.Versions
{
    internal static class VersionsNotifications
    {
        private static readonly CultureInfo CULTURE = CultureInfo.InvariantCulture;
        private const int CHECK_FREQUENCY = 6;
        
        private const string KEY_CHECK_DATE = "gc:versions-check-date";
        private const string KEY_REMIND_UPDATES = "gc:versions-remind-updates";
        private const string KEY_VERSION_SEEN = "gc:versions-{0}-number";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool RemindUpdates
        {
            get => EditorPrefs.GetBool(KEY_REMIND_UPDATES, true);
            set => EditorPrefs.SetBool(KEY_REMIND_UPDATES, value);
        }

        // INITIALIZERS: --------------------------------------------------------------------------
        
        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SettingsWindow.InitRunners.Add(new InitRunner(
                SettingsWindow.INIT_PRIORITY_DEFAULT,
                CanInitializeRemindUpdates,
                InitializeRemindUpdates
            ));
        }
        
        private static bool CanInitializeRemindUpdates()
        {
            if (!RemindUpdates) return false;
            string minDate = DateTime.MinValue.ToString(CULTURE);
            
            DateTime currentDate = DateTime.Now;
            DateTime checkDate = DateTime.Parse(
                EditorPrefs.GetString(KEY_CHECK_DATE, minDate),
                CULTURE
            );

            TimeSpan timeDifference = currentDate - checkDate;
            return timeDifference.TotalHours >= CHECK_FREQUENCY;
        }

        private static void InitializeRemindUpdates()
        {
            DateTime currentDate = DateTime.Now;
            EditorPrefs.SetString(KEY_CHECK_DATE, currentDate.ToString(CULTURE));

            VersionsManager.EventDone -= OnFetchComplete;
            VersionsManager.EventDone += OnFetchComplete;

            VersionsManager.Initialize();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static void OnFetchComplete()
        {
            VersionsManager.EventDone -= OnFetchComplete;
            bool showSettingsUpdates = false;

            foreach (KeyValuePair<string, AssetEntry> entry in VersionsManager.LatestEntries)
            {
                AssetVersion installedVersion = VersionsManager.GetInstalledVersion(entry.Key);
                if (installedVersion.Empty) continue;

                if (installedVersion.IsOlderThan(entry.Value.Version))
                {
                    string keySavedVersion = string.Format(KEY_VERSION_SEEN, entry.Key);
                    string savedVersionString = EditorPrefs.GetString(keySavedVersion);
                    AssetVersion savedVersion = new AssetVersion(savedVersionString);

                    if (entry.Value.Version.IsNewerThan(savedVersion))
                    {
                        showSettingsUpdates = true;
                        EditorPrefs.SetString(keySavedVersion, entry.Value.Version.ToString());
                    }
                }
            }

            if (showSettingsUpdates)
            {
                SettingsWindow.OpenWindow(UpdatesRepository.REPOSITORY_ID);
            }
        }
    }
}