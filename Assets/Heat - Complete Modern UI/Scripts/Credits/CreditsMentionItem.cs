using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Heat
{
    public class CreditsMentionItem : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI descriptionText;
        public VerticalLayoutGroup listLayout;

        [HideInInspector] public CreditsPreset preset;
        [HideInInspector] public LocalizedObject localizedObject;

        void OnEnable()
        {
            if (localizedObject != null && !string.IsNullOrEmpty(localizedObject.localizationKey))
            {
                SetDescription(localizedObject.GetKeyOutput(localizedObject.localizationKey));
            }
        }

        public void UpdateLayout(int paddingValue, int spacingValue)
        {
            listLayout.padding.top = paddingValue / 2;
            listLayout.padding.bottom = paddingValue / 2;
            listLayout.spacing = spacingValue;
        }

        public void SetIcon(Sprite icon)
        {
            if (icon == null)
            {
                iconImage.gameObject.SetActive(false);
                return;
            }

            iconImage.sprite = icon;
        }

        public void SetDescription(string text)
        {
            if (string.IsNullOrEmpty(text)) 
            {
                descriptionText.gameObject.SetActive(false);
                return;
            }

            descriptionText.text = text;
        }

        public void CheckForLocalization(string key)
        {
            localizedObject = descriptionText.GetComponent<LocalizedObject>();
            if (localizedObject == null || localizedObject.localizationManager != null && localizedObject.localizationManager.UIManagerAsset.enableLocalization == false) { localizedObject = null; }
            else if (!string.IsNullOrEmpty(key)) 
            {
                localizedObject.localizationKey = key;
                SetDescription(localizedObject.GetKeyOutput(localizedObject.localizationKey));
            }
        }
    }
}