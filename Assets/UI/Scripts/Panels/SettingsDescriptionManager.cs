using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Heat
{
    public class SettingsDescriptionManager : MonoBehaviour
    {
        [Header("Default Content")]
        [SerializeField] private Sprite cover;
        [SerializeField] private string title = "Settings";
        [SerializeField] [TextArea] private string description = "Description area.";

        [Header("Localization")]
        public string tableID = "UI";
        [SerializeField] private string titleKey;
        [SerializeField] private string descriptionKey;

        [Header("Resources")]
        [SerializeField] private Image coverImage;
        [SerializeField] private TextMeshProUGUI titleObject;
        [SerializeField] private TextMeshProUGUI descriptionObject;

        [Header("Settings")]
        public bool useLocalization = true;

        // Helpers
        [HideInInspector] public LocalizedObject localizedObject;

        void Awake()
        {
            if (useLocalization == true && !string.IsNullOrEmpty(titleKey) && !string.IsNullOrEmpty(descriptionKey))
            { 
                CheckForLocalization(); 
            }
        }

        void OnEnable()
        {
            SetDefault();
        }

        void CheckForLocalization()
        {
            localizedObject = gameObject.GetComponent<LocalizedObject>();

            if (localizedObject == null) 
            { 
                localizedObject = gameObject.AddComponent<LocalizedObject>();
                localizedObject.objectType = LocalizedObject.ObjectType.ComponentDriven; 
                localizedObject.updateMode = LocalizedObject.UpdateMode.OnDemand;
                localizedObject.InitializeItem();

                LocalizationSettings locSettings = localizedObject.localizationManager.UIManagerAsset.localizationSettings;
                foreach (LocalizationSettings.Table table in locSettings.tables) 
                {
                    if (tableID == table.tableID) 
                    { 
                        localizedObject.tableIndex = locSettings.tables.IndexOf(table);
                        break;
                    }
                }
            }

            if (localizedObject.tableIndex == -1 || localizedObject.localizationManager == null || localizedObject.localizationManager.UIManagerAsset.enableLocalization == false) 
            { 
                localizedObject = null;
                useLocalization = false;
            }
        }

        public void UpdateUI(string newTitle, string newDescription, Sprite newCover) 
        {
            if (newCover != null) { coverImage.sprite = newCover; }
            else { coverImage.sprite = cover; }

            titleObject.text = newTitle;
            descriptionObject.text = newDescription;
        }

        public void SetDefault()
        {
            if (localizedObject == null)
            {
                titleObject.text = title;
                descriptionObject.text = description;
            }

            else
            {
                titleObject.text = localizedObject.GetKeyOutput(titleKey);
                descriptionObject.text = localizedObject.GetKeyOutput(descriptionKey);
            }

            coverImage.sprite = cover;
        }
    }
}