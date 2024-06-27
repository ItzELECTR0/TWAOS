using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [CreateAssetMenu(fileName = "New Credits Preset", menuName = "Heat UI/Panel/New Credits Preset")]
    public class CreditsPreset : ScriptableObject
    {
        [Header("Settings")]
        public Sprite backgroundSprite;
        public int sectionSpacing = 70;
        public int headerSpacing = 30;
        public int nameListSpacing = 50;

        [Space(10)]
        public List<CreditsSection> credits = new List<CreditsSection>();
        public List<MentionSection> mentions = new List<MentionSection>();

        [System.Serializable]
        public class CreditsSection
        {
            public string headerTitle = "Header";
            public string headerTitleKey = "Localization Key";
            public List<string> items = new List<string>();
        }

        [System.Serializable]
        public class MentionSection
        {
            public string ID = "ID";
            public Sprite icon;
            [TextArea] public string description = "Description";
            public string descriptionKey = "Localization Key";
            [Range(0, 100)] public int descriptionSpacing = 30;
            [Range(0, 400)] public int layoutSpacing = 160;
        }
    }
}