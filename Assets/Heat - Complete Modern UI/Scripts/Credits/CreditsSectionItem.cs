using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    public class CreditsSectionItem : MonoBehaviour
    {
        [Header("Resources")]
        public HorizontalLayoutGroup headerLayout;
        public VerticalLayoutGroup listLayout;
        [SerializeField] private TextMeshProUGUI headerText;
        public GameObject namePreset;

        [HideInInspector] public CreditsPreset preset;
        [HideInInspector] public LocalizedObject localizedObject;

        void OnEnable()
        {
            if (localizedObject != null && !string.IsNullOrEmpty(localizedObject.localizationKey)) 
            {
                SetHeader(localizedObject.GetKeyOutput(localizedObject.localizationKey)); 
            }
        }

        public void UpdateLayout()
        {
            headerLayout.spacing = preset.headerSpacing;
            listLayout.spacing = preset.nameListSpacing;
        }

        public void AddNameToList(string name)
        {
            GameObject go = Instantiate(namePreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(listLayout.transform, false);
            go.name = name;

            TextMeshProUGUI goText = go.GetComponent<TextMeshProUGUI>();
            goText.text = name;
        }

        public void SetHeader(string text)
        {
            headerText.text = text;
        }

        public void CheckForLocalization(string key)
        {
            localizedObject = headerText.GetComponent<LocalizedObject>();

            if (localizedObject == null || localizedObject.CheckLocalizationStatus() == false) { localizedObject = null; }
            else if (!string.IsNullOrEmpty(key)) 
            {
                localizedObject.localizationKey = key;
                SetHeader(localizedObject.GetKeyOutput(localizedObject.localizationKey));
            }
        }
    }
}