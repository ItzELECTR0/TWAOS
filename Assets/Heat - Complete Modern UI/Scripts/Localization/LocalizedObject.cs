using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Heat UI/Localization/Localized Object")]
    public class LocalizedObject : MonoBehaviour
    {
        // Resources
        public LocalizationManager localizationManager;
        public LocalizationSettings localizationSettings;
        public TextMeshProUGUI textObj;
        public AudioSource audioObj;
        public Image imageObj;

        // Settings
        public int tableIndex = -1;
        public string localizationKey;
        public ObjectType objectType = ObjectType.TextMeshPro;
        public UpdateMode updateMode = UpdateMode.OnEnable;
        public bool rebuildLayoutOnUpdate;
        [SerializeField] private bool forceAddToManager = false;
#if UNITY_EDITOR
        public bool showOutputOnEditor = true;
#endif

        // Events
        public LanguageChangedEvent onLanguageChanged = new LanguageChangedEvent();

        [System.Serializable]
        public class LanguageChangedEvent : UnityEvent<string> { }

        // Helpers
        public bool isInitialized = false;

        public enum UpdateMode { OnEnable, OnDemand }
        public enum ObjectType { TextMeshPro, Custom, ComponentDriven, Audio, Image }

        void Awake()
        {
            if (localizationManager != null && !localizationManager.UIManagerAsset.enableLocalization) { Destroy(this); return; }
            InitializeItem();
        }

        void OnEnable()
        {
            if (localizationManager == null) { Destroy(this); return; }
            if (!isInitialized || localizationManager == null || localizationManager.currentLanguageAsset == null) { return; }
            if (updateMode == UpdateMode.OnEnable) { UpdateItem(); }
        }

        public void InitializeItem()
        {
            if (isInitialized)
                return;

            if (localizationManager == null)
            {
                bool locManagerFound = false;

                foreach (LocalizationManager lm in Resources.FindObjectsOfTypeAll(typeof(LocalizationManager)) as LocalizationManager[])
                {
                    if (lm.gameObject.scene.name != null)
                    {
                        localizationManager = lm;
                        locManagerFound = true;
                        break;
                    }
                }

                if (locManagerFound == false)
                {
                    UIManager tempUIM = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0];
                    if (tempUIM == null || !tempUIM.enableLocalization) { return; }

                    GameObject newLM = new GameObject("Localization Manager [Auto Generated]");
                    localizationManager = newLM.AddComponent<LocalizationManager>();
                }
            }

            if (localizationManager != null && !localizationManager.UIManagerAsset.enableLocalization) { Destroy(this); return; }
            if (localizationManager == null || localizationManager.UIManagerAsset == null || !localizationManager.UIManagerAsset.enableLocalization) { return; }
            if (forceAddToManager && !localizationManager.localizedItems.Contains(this)) { localizationManager.localizedItems.Add(this); }

            if (objectType == ObjectType.TextMeshPro && textObj == null) { textObj = gameObject.GetComponent<TextMeshProUGUI>(); }
            else if (objectType == ObjectType.Audio && audioObj == null) { audioObj = gameObject.GetComponent<AudioSource>(); }
            else if (objectType == ObjectType.Image && imageObj == null) { imageObj = gameObject.GetComponent<Image>(); }

            isInitialized = true;
        }

        public void ReInitializeItem()
        {
            isInitialized = false;
            InitializeItem();
        }

        public void UpdateItem()
        {
            if (!isInitialized || localizationManager == null || localizationManager.currentLanguageAsset == null || localizationManager.currentLanguageAsset.tableList.Count == 0)
                return;

            if (objectType == ObjectType.TextMeshPro && textObj != null)
            {
                for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (localizationKey == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                    {
                        if (string.IsNullOrEmpty(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value))
                        {
                            if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The specified key '" + localizationKey + "' could not be found or the output value is empty for " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                            break;
                        }

                        textObj.text = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value;
                        onLanguageChanged.Invoke(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value);
                        break;
                    }
                }
            }

            else if (objectType == ObjectType.Audio && audioObj != null)
            {
                for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (localizationKey == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                    {
                        if (localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].audioValue == null)
                        {
                            if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The specified key '" + localizationKey + "' could not be found or the output value is empty for " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                            break;
                        }

                        audioObj.clip = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].audioValue;
                        break;
                    }
                }
            }

            else if (objectType == ObjectType.Image && imageObj != null)
            {
                for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (localizationKey == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                    {
                        if (localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].spriteValue == null)
                        {
                            if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The specified key '" + localizationKey + "' could not be found or the output value is empty for " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                            break;
                        }

                        imageObj.sprite = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].spriteValue;
                        break;
                    }
                }
            }

            else if (objectType == ObjectType.Custom || objectType == ObjectType.ComponentDriven)
            {
                for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (localizationKey == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                    {
                        if (string.IsNullOrEmpty(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value))
                            break;

                        onLanguageChanged.Invoke(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value);
                        break;
                    }
                }
            }

            if (rebuildLayoutOnUpdate && gameObject.activeInHierarchy) { StartCoroutine("RebuildLayout"); }
        }

        public bool CheckLocalizationStatus()
        {
            if (!isInitialized) { InitializeItem(); }
            if (localizationManager == null || localizationManager.UIManagerAsset == null || !localizationManager.UIManagerAsset.enableLocalization) { return false; }
            else { return true; }
        }

        public string GetKeyOutput(string key)
        {
            string keyValue = null;
            bool keyFound = false;

            if (localizationManager != null && localizationManager.currentLanguageAsset == null) { localizationManager.InitializeLanguage(); }
            if (!isInitialized || localizationManager == null || localizationManager.currentLanguageAsset == null || localizationManager.currentLanguageAsset.tableList.Count == 0)
                return LocalizationSettings.notInitializedText;

            for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
            {
                if (key == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                {
                    keyValue = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value;
                    keyFound = true;
                    break;
                }
            }

            if (keyFound && string.IsNullOrEmpty(keyValue))
            {
                if (LocalizationManager.enableLogs == true) { Debug.Log("<b>[Localized Object]</b> The output value for '" + key + "' is empty in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return "EMPTY_KEY_IN_" + localizationManager.currentLanguageAsset.languageID + ": " + key;
            }

            else if (!keyFound)
            {
                if (LocalizationManager.enableLogs == true) { Debug.Log("<b>[Localized Object]</b> The specified key '" + key + "' could not be found in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return "MISSING_KEY_IN_" + localizationManager.currentLanguageAsset.languageID + ": " + key;
            }

            return keyValue;
        }

        public AudioClip GetKeyOutputAudio(string key)
        {
            AudioClip keyValue = null;
            bool keyFound = false;

            if (localizationManager != null && localizationManager.currentLanguageAsset == null) { localizationManager.InitializeLanguage(); }
            if (!isInitialized || localizationManager == null || localizationManager.currentLanguageAsset == null || localizationManager.currentLanguageAsset.tableList.Count == 0)
                return null;

            for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
            {
                if (key == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                {
                    keyValue = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].audioValue;
                    keyFound = true;
                    break;
                }
            }

            if (keyFound && keyValue == null)
            {
                if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The output value for '" + key + "' is empty in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return null;
            }

            else if (!keyFound)
            {
                if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The specified key '" + key + "' could not be found in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return null;
            }

            return keyValue;
        }

        public Sprite GetKeyOutputSprite(string key)
        {
            Sprite keyValue = null;
            bool keyFound = false;

            if (localizationManager != null && localizationManager.currentLanguageAsset == null) { localizationManager.InitializeLanguage(); }
            if (!isInitialized || localizationManager == null || localizationManager.currentLanguageAsset == null || localizationManager.currentLanguageAsset.tableList.Count == 0)
                return null;

            for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
            {
                if (key == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                {
                    keyValue = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].spriteValue;
                    keyFound = true;
                    break;
                }
            }

            if (keyFound && keyValue == null)
            {
                if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The output value for '" + key + "' is empty in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return null;
            }

            else if (!keyFound)
            {
                if (LocalizationManager.enableLogs) { Debug.Log("<b>[Localized Object]</b> The specified key '" + key + "' could not be found in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return null;
            }

            return keyValue;
        }

        public static string GetKeyOutput(string tableID, string tableKey)
        {
            UIManager tempUIM = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0];

            if (tempUIM == null || !tempUIM.enableLocalization)
                return null;

            int tableIndex = -1;
            string keyValue = null;

            for (int i = 0; i < tempUIM.currentLanguage.tableList.Count; i++)
            {
                if (tempUIM.currentLanguage.tableList[i].table.tableID == tableID)
                {
                    tableIndex = i;
                    break;
                }
            }

            if (tableIndex == -1) { return null; }
            else
            {
                for (int i = 0; i < tempUIM.currentLanguage.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].key == tableKey)
                    {
                        keyValue = tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].value;
                        break;
                    }
                }
            }

            return keyValue;
        }

        public static AudioClip GetKeyOutputAudio(string tableID, string tableKey)
        {
            UIManager tempUIM = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0];

            if (tempUIM == null || !tempUIM.enableLocalization)
                return null;

            int tableIndex = -1;
            AudioClip keyValue = null;

            for (int i = 0; i < tempUIM.currentLanguage.tableList.Count; i++)
            {
                if (tempUIM.currentLanguage.tableList[i].table.tableID == tableID)
                {
                    tableIndex = i;
                    break;
                }
            }

            if (tableIndex == -1) { return null; }
            else
            {
                for (int i = 0; i < tempUIM.currentLanguage.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].key == tableKey)
                    {
                        keyValue = tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].audioValue;
                        break;
                    }
                }
            }

            return keyValue;
        }

        public static Sprite GetKeyOutputSprite(string tableID, string tableKey)
        {
            UIManager tempUIM = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0];

            if (tempUIM == null || !tempUIM.enableLocalization)
                return null;

            int tableIndex = -1;
            Sprite keyValue = null;

            for (int i = 0; i < tempUIM.currentLanguage.tableList.Count; i++)
            {
                if (tempUIM.currentLanguage.tableList[i].table.tableID == tableID)
                {
                    tableIndex = i;
                    break;
                }
            }

            if (tableIndex == -1) { return null; }
            else
            {
                for (int i = 0; i < tempUIM.currentLanguage.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].key == tableKey)
                    {
                        keyValue = tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].spriteValue;
                        break;
                    }
                }
            }

            return keyValue;
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }
    }
}