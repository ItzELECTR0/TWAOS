using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    public class HorizontalSelector : MonoBehaviour
    {
        // Resources
        public TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI labelHelper;
        public Image labelIcon;
        [SerializeField] private Image labelIconHelper;
        public Transform indicatorParent;
        public GameObject indicatorObject;
        [SerializeField] private Animator selectorAnimator;
        [SerializeField] private HorizontalLayoutGroup contentLayout;
        [SerializeField] private HorizontalLayoutGroup contentLayoutHelper;
        private string newItemTitle;

        // Saving
        public bool saveSelected = false;
        public string saveKey = "Horizontal Selector";

        // Settings
        public bool enableIndicator = true;
        public bool enableIcon = false;
        public bool invokeOnAwake = true;
        public bool invertAnimation;
        public bool loopSelection;
        public bool useLocalization = true;
        [Range(0.25f, 2.5f)] public float iconScale = 1;
        [Range(1, 50)] public int contentSpacing = 15;
        public int defaultIndex = 0;
        [HideInInspector] public int index = 0;

        // Items
        public List<Item> items = new List<Item>();

        // Events
        public HorizontalSelectorEvent onValueChanged = new HorizontalSelectorEvent();

        // Helpers
        LocalizedObject localizedObject;
        float cachedStateLength;

        [System.Serializable]
        public class Item
        {
            public string itemTitle = "Item Title";
            public string localizationKey;
            public Sprite itemIcon;
            public UnityEvent onItemSelect = new UnityEvent();
        }

        [System.Serializable] 
        public class HorizontalSelectorEvent : UnityEvent<int> { }

        void Awake()
        {
            if (selectorAnimator == null) { selectorAnimator = gameObject.GetComponent<Animator>(); }
            if (label == null || labelHelper == null)
            {
                Debug.LogError("<b>[Horizontal Selector]</b> Cannot initalize the object due to missing resources.", this);
                return;
            }

            InitializeSelector();
            UpdateContentLayout();

            if (invokeOnAwake == true)
            {
                items[index].onItemSelect.Invoke();
                onValueChanged.Invoke(index);
            }

            cachedStateLength = HeatUIInternalTools.GetAnimatorClipLength(selectorAnimator, "HorizontalSelector_Next");
        }

        void OnEnable()
        {
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
            if (useLocalization == true && !string.IsNullOrEmpty(items[index].localizationKey) && localizedObject.CheckLocalizationStatus() == true) 
            { 
                label.text = localizedObject.GetKeyOutput(items[index].localizationKey); 
            }
        }

        public void InitializeSelector()
        {
            if (items.Count == 0)
                return;

            if (saveSelected == true)
            {
                if (PlayerPrefs.HasKey("HorizontalSelector_" + saveKey) == true) { defaultIndex = PlayerPrefs.GetInt("HorizontalSelector_" + saveKey); }
                else { PlayerPrefs.SetInt("HorizontalSelector_" + saveKey, defaultIndex); }
            }

            label.text = items[defaultIndex].itemTitle;
            labelHelper.text = label.text;

            if (labelIcon != null && enableIcon == true)
            {
                labelIcon.sprite = items[defaultIndex].itemIcon;
                labelIconHelper.sprite = labelIcon.sprite;
            }

            else if (enableIcon == false)
            {
                if (labelIcon != null) { labelIcon.gameObject.SetActive(false); }
                if (labelIconHelper != null) { labelIconHelper.gameObject.SetActive(false); }
            }

            index = defaultIndex;

            if (enableIndicator == true) { UpdateIndicators(); }
            else { Destroy(indicatorParent.gameObject); }

            if (useLocalization == true)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();

                if (localizedObject == null || localizedObject.CheckLocalizationStatus() == false) { useLocalization = false; }
                else if (useLocalization == true) { localizedObject.onLanguageChanged.AddListener(delegate { UpdateUI(); }); }
            }
        }

        public void PreviousItem()
        {
            if (items.Count == 0) { return; }
            StopCoroutine("DisableAnimator");

            selectorAnimator.enabled = true;

            if (loopSelection == false)
            {
                if (index != 0)
                {
                    // Change the label helper text
                    labelHelper.text = label.text;
                    if (labelIcon != null && enableIcon == true) { labelIconHelper.sprite = labelIcon.sprite; }

                    // Change the index
                    if (index == 0) { index = items.Count - 1; }
                    else { index--; }

                    // Check for localization and change the label
                    if (useLocalization == true && !string.IsNullOrEmpty(items[index].localizationKey)) { label.text = localizedObject.GetKeyOutput(items[index].localizationKey); }
                    else { label.text = items[index].itemTitle; }

                    // Change the label icon
                    if (labelIcon != null && enableIcon == true) { labelIcon.sprite = items[index].itemIcon; }

                    // Invoke events
                    items[index].onItemSelect.Invoke();
                    onValueChanged.Invoke(index);

                    // Reset playback
                    selectorAnimator.Play(null);
                    selectorAnimator.StopPlayback();

                    // Play the animation
                    if (invertAnimation == true) { selectorAnimator.Play("Next"); }
                    else { selectorAnimator.Play("Prev"); }
                }
            }

            else
            {
                // Change the label helper text
                labelHelper.text = label.text;
                if (labelIcon != null && enableIcon == true) { labelIconHelper.sprite = labelIcon.sprite; }

                // Change the index
                if (index == 0) { index = items.Count - 1; }
                else { index--; }

                // Check for localization and change the label
                if (useLocalization == true && !string.IsNullOrEmpty(items[index].localizationKey)) { label.text = localizedObject.GetKeyOutput(items[index].localizationKey); }
                else { label.text = items[index].itemTitle; }

                // Change the label icon
                if (labelIcon != null && enableIcon == true) { labelIcon.sprite = items[index].itemIcon; }

                // Invoke events
                items[index].onItemSelect.Invoke();
                onValueChanged.Invoke(index);

                // Reset playback
                selectorAnimator.Play(null);
                selectorAnimator.StopPlayback();

                // Play the animation
                if (invertAnimation == true) { selectorAnimator.Play("Next"); }
                else { selectorAnimator.Play("Prev"); }
            }

            if (saveSelected == true) { PlayerPrefs.SetInt("HorizontalSelector_" + saveKey, index); }
            if (enableIndicator == true)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    GameObject go = indicatorParent.GetChild(i).gameObject;
                    Transform onObj = go.transform.Find("On");
                    Transform offObj = go.transform.Find("Off");

                    if (i == index) { onObj.gameObject.SetActive(true); offObj.gameObject.SetActive(false); }
                    else { onObj.gameObject.SetActive(false); offObj.gameObject.SetActive(true); }
                }
            }

            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
        }

        public void NextItem()
        {
            if (items.Count == 0) { return; }
            StopCoroutine("DisableAnimator");

            selectorAnimator.enabled = true;

            if (loopSelection == false)
            {
                if (index != items.Count - 1)
                {
                    // Change the label helper text
                    labelHelper.text = label.text;
                    if (labelIcon != null && enableIcon == true) { labelIconHelper.sprite = labelIcon.sprite; }

                    // Change the index
                    if ((index + 1) >= items.Count) { index = 0; }
                    else { index++; }

                    // Check for localization and change the label
                    if (useLocalization == true && !string.IsNullOrEmpty(items[index].localizationKey)) { label.text = localizedObject.GetKeyOutput(items[index].localizationKey); }
                    else { label.text = items[index].itemTitle; }

                    // Change the label icon
                    if (labelIcon != null && enableIcon == true) { labelIcon.sprite = items[index].itemIcon; }

                    // Invoke events
                    items[index].onItemSelect.Invoke();
                    onValueChanged.Invoke(index);

                    // Reset playback
                    selectorAnimator.Play(null);
                    selectorAnimator.StopPlayback();

                    // Play the animation
                    if (invertAnimation == true) { selectorAnimator.Play("Prev"); }
                    else { selectorAnimator.Play("Next"); }
                }
            }

            else
            {
                // Change the label helper text
                labelHelper.text = label.text;
                if (labelIcon != null && enableIcon == true) { labelIconHelper.sprite = labelIcon.sprite; }

                // Change the index
                if ((index + 1) >= items.Count) { index = 0; }
                else { index++; }

                // Check for localization and change the label
                if (useLocalization == true && !string.IsNullOrEmpty(items[index].localizationKey)) { label.text = localizedObject.GetKeyOutput(items[index].localizationKey); }
                else { label.text = items[index].itemTitle; }

                // Change the label icon
                if (labelIcon != null && enableIcon == true) { labelIcon.sprite = items[index].itemIcon; }

                // Invoke events
                items[index].onItemSelect.Invoke();
                onValueChanged.Invoke(index);

                // Reset playback
                selectorAnimator.Play(null);
                selectorAnimator.StopPlayback();

                // Play the animation
                if (invertAnimation == true) { selectorAnimator.Play("Prev"); }
                else { selectorAnimator.Play("Next"); }
            }

            if (saveSelected == true) { PlayerPrefs.SetInt("HorizontalSelector_" + saveKey, index); }
            if (enableIndicator == true)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    GameObject go = indicatorParent.GetChild(i).gameObject;
                    Transform onObj = go.transform.Find("On"); ;
                    Transform offObj = go.transform.Find("Off");

                    if (i == index) { onObj.gameObject.SetActive(true); offObj.gameObject.SetActive(false); }
                    else { onObj.gameObject.SetActive(false); offObj.gameObject.SetActive(true); }
                }
            }

            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
        }

        public void UpdateUI()
        {
            selectorAnimator.enabled = true;

            if (useLocalization == true && !string.IsNullOrEmpty(items[index].localizationKey)) { label.text = localizedObject.GetKeyOutput(items[index].localizationKey); }
            else { label.text = items[index].itemTitle; }

            if (labelIcon != null && enableIcon == true) { labelIcon.sprite = items[index].itemIcon; }

            UpdateContentLayout();
            UpdateIndicators();

            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
        }

        public void UpdateIndicators()
        {
            if (enableIndicator == false)
                return;

            foreach (Transform child in indicatorParent) { Destroy(child.gameObject); }
            for (int i = 0; i < items.Count; ++i)
            {
                GameObject go = Instantiate(indicatorObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(indicatorParent, false);
                go.name = items[i].itemTitle;

                Transform onObj = go.transform.Find("On");
                Transform offObj = go.transform.Find("Off");

                if (i == index) { onObj.gameObject.SetActive(true); offObj.gameObject.SetActive(false); }
                else { onObj.gameObject.SetActive(false); offObj.gameObject.SetActive(true); }
            }
        }

        public void UpdateContentLayout()
        {
            if (contentLayout != null) { contentLayout.spacing = contentSpacing; }
            if (contentLayoutHelper != null) { contentLayoutHelper.spacing = contentSpacing; }
            if (labelIcon != null)
            {
                labelIcon.transform.localScale = new Vector3(iconScale, iconScale, iconScale);
                labelIconHelper.transform.localScale = new Vector3(iconScale, iconScale, iconScale);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(label.transform.parent.GetComponent<RectTransform>());
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(cachedStateLength + 0.1f);
            selectorAnimator.enabled = false;
        }

        public void CreateNewItem(string title)
        {
            Item item = new Item();
            newItemTitle = title;
            item.itemTitle = newItemTitle;
            items.Add(item);
        }

        public void RemoveItem(string itemTitle)
        {
            var item = items.Find(x => x.itemTitle == itemTitle);
            items.Remove(item);
            InitializeSelector();
        }

        public void AddNewItem()
        {
            Item item = new Item();
            items.Add(item);
        }
    }
}