using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Heat
{
    public class NewsSlider : MonoBehaviour
    {
        // Content
        public List<Item> items = new List<Item>();
        private List<Animator> timers = new List<Animator>();

        // Resources
        [SerializeField] private GameObject itemPreset;
        [SerializeField] private Transform itemParent;
        [SerializeField] private GameObject timerPreset;
        [SerializeField] private Transform timerParent;

        // Settings
        public bool useLocalization = true;
        [Range(1, 30)] public float sliderTimer = 4;

        // Helpers
        Animator currentItemObject;
        Animator currentIndicatorObject;
        Image currentIndicatorBar;
        int currentSliderIndex;
        float sliderTimerBar;
        bool isInitialized;
        LocalizedObject localizedObject;

        [System.Serializable]
        public class Item
        {
            public string title = "News title";
            [TextArea] public string description = "News description";
            public Sprite background;
            public string buttonText = "Show More";
            public UnityEvent onButtonClick = new UnityEvent();

            [Header("Localization")]
            public string titleKey = "TitleKey";
            public string descriptionKey = "DescriptionKey";
            public string buttonTextKey = "ButtonTextKey";
        }

        void OnEnable()
        {
            if (isInitialized == false) { Initialize(); }
            else { StartCoroutine(PrepareSlider()); }
        }

        void Update()
        {
            if (isInitialized == false)
                return;

            CheckForTimer();
        }

        void CheckForTimer()
        {
            if (sliderTimerBar <= sliderTimer && currentIndicatorBar != null) 
            { 
                sliderTimerBar += Time.unscaledDeltaTime;
                currentIndicatorBar.fillAmount = sliderTimerBar / sliderTimer;
            }
        }

        public void Initialize()
        {
            if (useLocalization == true)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();
                if (localizedObject == null || localizedObject.CheckLocalizationStatus() == false) { useLocalization = false; }
            }

            foreach (Transform child in itemParent) { Destroy(child.gameObject); }
            foreach (Transform child in timerParent) { Destroy(child.gameObject); }
            for (int i = 0; i < items.Count; ++i)
            {
                int tempIndex = i;

                GameObject itemGO = Instantiate(itemPreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                itemGO.transform.SetParent(itemParent, false);
                itemGO.gameObject.name = items[i].title;

                TextMeshProUGUI itemTitle = itemGO.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                if (useLocalization == false || string.IsNullOrEmpty(items[i].titleKey)) { itemTitle.text = items[i].title; }
                else 
                {
                    LocalizedObject tempLoc = itemTitle.GetComponent<LocalizedObject>();
                    if (tempLoc != null) 
                    {
                        tempLoc.tableIndex = localizedObject.tableIndex;
                        tempLoc.localizationKey = items[i].titleKey;
                        tempLoc.onLanguageChanged.AddListener(delegate { itemTitle.text = tempLoc.GetKeyOutput(tempLoc.localizationKey); });
                        tempLoc.InitializeItem();
                        tempLoc.UpdateItem();
                    }
                }

                TextMeshProUGUI itemDescription = itemGO.transform.Find("Description").GetComponent<TextMeshProUGUI>();
                if (useLocalization == false || string.IsNullOrEmpty(items[i].descriptionKey)) { itemDescription.text = items[i].description; }
                else
                {
                    LocalizedObject tempLoc = itemDescription.GetComponent<LocalizedObject>();
                    if (tempLoc != null) 
                    {
                        tempLoc.tableIndex = localizedObject.tableIndex;
                        tempLoc.localizationKey = items[i].descriptionKey;
                        tempLoc.onLanguageChanged.AddListener(delegate { itemDescription.text = tempLoc.GetKeyOutput(tempLoc.localizationKey); });
                        tempLoc.InitializeItem();
                        tempLoc.UpdateItem();
                    }
                }

                Image background = itemGO.transform.Find("Background").GetComponent<Image>();
                background.sprite = items[i].background;

                ButtonManager libraryItemButton = itemGO.GetComponentInChildren<ButtonManager>();
                if (libraryItemButton != null)
                {
                    if (useLocalization == false) { libraryItemButton.buttonText = items[i].buttonText; }
                    else
                    {
                        LocalizedObject tempLoc = libraryItemButton.GetComponent<LocalizedObject>();
                        if (tempLoc != null)
                        {
                            tempLoc.tableIndex = localizedObject.tableIndex;
                            tempLoc.localizationKey = items[i].buttonTextKey;
                            tempLoc.onLanguageChanged.AddListener(delegate
                            {
                                libraryItemButton.buttonText = tempLoc.GetKeyOutput(tempLoc.localizationKey);
                                libraryItemButton.UpdateUI();
                            });
                            tempLoc.InitializeItem();
                            tempLoc.UpdateItem();
                        }
                    }
                    libraryItemButton.UpdateUI();
                    libraryItemButton.onClick.AddListener(delegate { items[tempIndex].onButtonClick.Invoke(); });
                    if (string.IsNullOrEmpty(libraryItemButton.buttonText)) { libraryItemButton.gameObject.SetActive(false); }
                }

                GameObject timerGO = Instantiate(timerPreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                timerGO.transform.SetParent(timerParent, false);
                timerGO.gameObject.name = items[i].title;
                timers.Add(timerGO.GetComponent<Animator>());

                Button timerButton = timerGO.transform.Find("Dot").GetComponent<Button>();
                timerButton.onClick.AddListener(delegate
                {
                    StopCoroutine("WaitForSliderTimer");
                    StopCoroutine("DisableItemAnimators");

                    currentItemObject.gameObject.SetActive(true);
                    currentItemObject.enabled = true;
                    currentIndicatorObject.enabled = true;

                    currentItemObject.Play("Out");
                    currentIndicatorObject.Play("Out");
 
                    currentSliderIndex = timerGO.transform.GetSiblingIndex();
                    currentItemObject = itemParent.GetChild(currentSliderIndex).GetComponent<Animator>();

                    currentIndicatorBar = timers[currentSliderIndex].transform.Find("Bar/Filled").GetComponent<Image>();
                    currentIndicatorObject = timers[currentSliderIndex];

                    currentItemObject.gameObject.SetActive(true);
                    currentItemObject.enabled = true;
                    currentIndicatorObject.enabled = true;

                    currentItemObject.Play("In");
                    currentIndicatorObject.Play("In");

                    sliderTimerBar = 0;
                    currentIndicatorBar.fillAmount = sliderTimerBar;

                    StartCoroutine("WaitForSliderTimer");
                    StartCoroutine("DisableItemAnimators");
                });
            }

            isInitialized = true;
            StartCoroutine(PrepareSlider());
        }

        IEnumerator PrepareSlider()
        {
            yield return new WaitForSecondsRealtime(0.02f);

            sliderTimerBar = 0;

            currentItemObject = itemParent.GetChild(currentSliderIndex).GetComponent<Animator>();
            currentIndicatorBar = timers[currentSliderIndex].transform.Find("Bar/Filled").GetComponent<Image>();
            currentIndicatorObject = timers[currentSliderIndex];

            currentItemObject.enabled = true;
            currentIndicatorObject.enabled = true;

            currentItemObject.Play("In");
            currentIndicatorObject.Play("In");

            StopCoroutine("WaitForSliderTimer");
            StopCoroutine("DisableItemAnimators");

            StartCoroutine("WaitForSliderTimer");
            StartCoroutine("DisableItemAnimators");
        }

        IEnumerator WaitForSliderTimer()
        {
            yield return new WaitForSecondsRealtime(sliderTimer);

            currentItemObject.enabled = true;
            currentIndicatorObject.enabled = true;

            currentItemObject.Play("Out");
            currentIndicatorObject.Play("Out");

            if (currentSliderIndex == items.Count - 1) { currentSliderIndex = 0; }
            else { currentSliderIndex++; }

            sliderTimerBar = 0;

            currentItemObject = itemParent.GetChild(currentSliderIndex).GetComponent<Animator>();
            currentIndicatorBar = timers[currentSliderIndex].transform.Find("Bar/Filled").GetComponent<Image>();
            currentIndicatorBar.fillAmount = sliderTimerBar;
            currentIndicatorObject = timers[currentSliderIndex];

            currentItemObject.gameObject.SetActive(true);
            currentItemObject.enabled = true;
            currentIndicatorObject.enabled = true;

            currentItemObject.Play("In");
            currentIndicatorObject.Play("In");

            StartCoroutine("DisableItemAnimators");
            StartCoroutine("WaitForSliderTimer");
        }

        IEnumerator DisableItemAnimators()
        {
            yield return new WaitForSecondsRealtime(0.6f);

            for (int i = 0; i < items.Count; i++)
            {
                if (i != currentSliderIndex) { itemParent.GetChild(i).gameObject.SetActive(false); }
                itemParent.GetChild(i).GetComponent<Animator>().enabled = false;
                timers[i].enabled = false;
            }
        }
    }
}