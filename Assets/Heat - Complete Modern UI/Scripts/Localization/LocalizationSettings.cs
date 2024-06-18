using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [CreateAssetMenu(fileName = "New Localization Settings", menuName = "Heat UI/Localization/New Localization Settings")]
    public class LocalizationSettings : ScriptableObject
    {
        public List<Language> languages = new List<Language>();
        public List<Table> tables = new List<Table>();
        public string defaultLanguageID;
        public int defaultLanguageIndex;
        public bool enableExperimental = false;

        // Global Variables
        public static string notInitializedText = "NOT_INITIALIZED";

        [System.Serializable]
        public class Language
        {
            public string languageID = "en-US";
            public string languageName = "English";
            public string localizedName = "English (US)";
            public LocalizationLanguage localizationLanguage;
#if UNITY_EDITOR
            [HideInInspector] public bool isExpanded;
#endif
        }

        [System.Serializable]
        public class Table
        {
            public string tableID;
            public LocalizationTable localizationTable;
#if UNITY_EDITOR
            [HideInInspector] public bool isExpanded;
#endif
        }
    }
}