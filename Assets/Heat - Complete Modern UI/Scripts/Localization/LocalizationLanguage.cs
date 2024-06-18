using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [CreateAssetMenu(fileName = "New Localization Table", menuName = "Heat UI/Localization/New Language")]
    public class LocalizationLanguage : ScriptableObject
    {
        public LocalizationSettings localizationSettings;
        public string languageID;
        public string languageName;
        public string localizedName;
        public List<TableList> tableList = new List<TableList>();

        [System.Serializable]
        public class TableList
        {
            public LocalizationTable table;
            public List<TableContent> tableContent = new List<TableContent>();
        }

        [System.Serializable]
        public class TableContent
        {
            public string key;
            [TextArea] public string value;
            public AudioClip audioValue;
            public Sprite spriteValue;
        }
    }
}