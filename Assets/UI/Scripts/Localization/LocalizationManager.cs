using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    public class LocalizationManager : MonoBehaviour
    {
        // Resources
        public UIManager UIManagerAsset;
        public HorizontalSelector languageSelector;

        // Settings
        public UpdateMode updateMode;
        public bool setLanguageOnAwake = true;
        public bool updateItemsOnSet = true;
        public bool saveLanguageChanges = true;
        public static bool enableLogs = true;

        // Helpers
        public string currentLanguage;
        public LocalizationLanguage currentLanguageAsset;
        public List<LocalizedObject> localizedItems = new List<LocalizedObject>();

        public enum UpdateMode { Realtime, Baked }

        void Awake()
        {
            if (UIManagerAsset == null) { UIManagerAsset = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0]; }
            if (UIManagerAsset == null || !UIManagerAsset.enableLocalization) { return; }
            if (setLanguageOnAwake) { InitializeLanguage(); }
            if (updateMode == UpdateMode.Realtime)
            {
                localizedItems.Clear();

                foreach (LocalizedObject go in Resources.FindObjectsOfTypeAll(typeof(LocalizedObject)) as LocalizedObject[])
                {
                    if (go.gameObject.scene.name != null) 
                    { 
                        localizedItems.Add(go); 
                    }
                }
            }

            // Populate language selector
            if (languageSelector != null)
            {
                languageSelector.items.Clear();

                for (int i = 0; i < UIManagerAsset.localizationSettings.languages.Count; i++)
                {
                    languageSelector.CreateNewItem(UIManagerAsset.localizationSettings.languages[i].localizedName);

                    string tempID = UIManagerAsset.localizationSettings.languages[i].languageID;
                    languageSelector.items[i].onItemSelect.AddListener(() => SetLanguage(tempID));

                    if (UIManagerAsset.localizationSettings.languages[i].localizationLanguage == currentLanguageAsset) 
                    { 
                        languageSelector.index = i;
                        languageSelector.defaultIndex = i;
                    }
                }

                languageSelector.UpdateUI();
            }
        }

        public void InitializeLanguage()
        {
            if (PlayerPrefs.HasKey(UIManager.localizationSaveKey)) { currentLanguage = PlayerPrefs.GetString(UIManager.localizationSaveKey); }
            else { currentLanguage = UIManagerAsset.localizationSettings.defaultLanguageID; }

            SetLanguage(currentLanguage);
        }

        public void SetLanguageByIndex(int index)
        {
            SetLanguage(UIManagerAsset.localizationSettings.languages[index].languageID);
        }

        public void SetLanguage(string langID)
        {
            if (UIManagerAsset == null || !UIManagerAsset.enableLocalization)
            {
                UIManager.isLocalizationEnabled = false;
                return;
            }

            currentLanguageAsset = null;

            for (int i = 0; i < UIManagerAsset.localizationSettings.languages.Count; i++)
            {
                if (UIManagerAsset.localizationSettings.languages[i].languageID == langID) { currentLanguageAsset = UIManagerAsset.localizationSettings.languages[i].localizationLanguage; break; }
                else if (UIManagerAsset.localizationSettings.languages[i].languageName + " (" + UIManagerAsset.localizationSettings.languages[i].languageID + ")" == langID) { currentLanguageAsset = UIManagerAsset.localizationSettings.languages[i].localizationLanguage; break; }
                else if (UIManagerAsset.localizationSettings.languages[i].languageName == langID + ")") { currentLanguageAsset = UIManagerAsset.localizationSettings.languages[i].localizationLanguage;break; }
            }

            if (currentLanguageAsset == null) { Debug.Log("<b>[Localization Manager]</b> No language named <b>" + langID + "</b> found.", this); return; }
            else { currentLanguage = currentLanguageAsset.languageName + " (" + currentLanguageAsset.languageID + ")"; }
            
            if (updateItemsOnSet)
            {
                for (int i = 0; i < localizedItems.Count; i++)
                {
                    if (localizedItems[i] == null) { localizedItems.RemoveAt(i); }
                    else if (localizedItems[i].gameObject.activeInHierarchy && localizedItems[i].updateMode != LocalizedObject.UpdateMode.OnDemand) { localizedItems[i].UpdateItem(); }
                }
            }

            if (saveLanguageChanges)
            {
                PlayerPrefs.SetString(UIManager.localizationSaveKey, currentLanguageAsset.languageID);
            }

            UIManagerAsset.currentLanguage = currentLanguageAsset;
            UIManager.isLocalizationEnabled = true;
        }

        public static void SetLanguageWithoutNotify(string langID)
        {
            PlayerPrefs.SetString(UIManager.localizationSaveKey, langID);
        }
    }
}