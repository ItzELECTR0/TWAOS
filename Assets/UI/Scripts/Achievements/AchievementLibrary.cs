using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [CreateAssetMenu(fileName = "New Achievement Library", menuName = "Heat UI/Achievement Library")]
    public class AchievementLibrary : ScriptableObject
    {
        [Space(10)]
        public List<AchievementItem> achievements = new List<AchievementItem>();

        public enum AchievementType { Common, Rare, Legendary }
        public enum DataBehaviour { Default, Unlocked }

        [System.Serializable]
        public class AchievementItem
        {
            public string title = "New Achievement";
            public Sprite icon;
            [TextArea] public string description;
            public AchievementType type;
            public DataBehaviour dataBehaviour;

            [Header("Hidden")]
            public bool isHidden;
            public string hiddenTitle = "Hidden Achievement";
            public Sprite hiddenIcon;
            [TextArea] public string hiddenDescription = "This is a hidden achievement and must be unlocked to preview.";

            [Header("Localization")]
            [Tooltip("If you're not using localization, you can leave this field blank.")]
            public string titleKey;
            [Tooltip("If you're not using localization, you can leave this field blank.")]
            public string decriptionKey;
            [Tooltip("If you're not using localization, you can leave this field blank.")]
            public string hiddenTitleKey = "AchievementHiddenTitle";
            [Tooltip("If you're not using localization, you can leave this field blank.")]
            public string hiddenDescKey = "AchievementHiddenDesc";
        }
    }
}