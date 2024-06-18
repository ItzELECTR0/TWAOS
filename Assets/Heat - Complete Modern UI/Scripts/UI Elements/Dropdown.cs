using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.Heat
{
    public class Dropdown : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Resources
        public GameObject triggerObject;
        public TextMeshProUGUI headerText;
        public Image headerImage;
        public Transform itemParent;
        [SerializeField] private GameObject itemPreset;
        public Scrollbar scrollbar;
        public VerticalLayoutGroup itemList;
        [SerializeField] private CanvasGroup highlightCG;
        public CanvasGroup contentCG;
        [SerializeField] private CanvasGroup listCG;
        [SerializeField] private RectTransform listRect;
        [SerializeField] private UIManagerAudio audioManager;

        // Settings
        public bool isInteractable = true;
        public bool enableIcon = true;
        public bool enableTrigger = true;
        public bool enableScrollbar = true;
        [SerializeField] private bool startAtBottom = false;
        public bool setHighPriority = true;
        public bool updateOnEnable = true;
        public bool outOnPointerExit = false;
        public bool invokeOnAwake = false;
        public bool initOnAwake = true;
        public bool useSounds = true;
        [Range(1, 15)] public float fadingMultiplier = 8;
        [Range(1, 50)] public int itemPaddingTop = 8;
        [Range(1, 50)] public int itemPaddingBottom = 8;
        [Range(1, 50)] public int itemPaddingLeft = 8;
        [Range(1, 50)] public int itemPaddingRight = 25;
        [Range(1, 50)] public int itemSpacing = 8;
        public int selectedItemIndex = 0;

        // UI Navigation
        public bool useUINavigation = false;
        public Navigation.Mode navigationMode = Navigation.Mode.Automatic;
        public GameObject selectOnUp;
        public GameObject selectOnDown;
        public GameObject selectOnLeft;
        public GameObject selectOnRight;
        public bool wrapAround = false;

        // Animation
        public PanelDirection panelDirection;
        [Range(25, 1000)] public float panelSize = 200;
        [Range(0.5f, 10)] public float curveSpeed = 2;
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        // Saving
        public bool saveSelected = false;
        public string saveKey = "My Dropdown";

        // Item list
        [SerializeField]
        public List<Item> items = new List<Item>();

        // Events
        [System.Serializable] public class DropdownEvent : UnityEvent<int> { }
        public DropdownEvent onValueChanged;

        // Helpers
        [HideInInspector] public bool isOn;
        [HideInInspector] public int index = 0;
        [HideInInspector] public int siblingIndex = 0;
        EventTrigger triggerEvent;
        Button targetButton;

        public enum PanelDirection { Bottom, Top }

        [System.Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public string localizationKey;
            public Sprite itemIcon;
            [HideInInspector] public int itemIndex;
            [HideInInspector] public bool isInvisible = false;
            [HideInInspector] public ButtonManager itemButton;
            public UnityEvent onItemSelection = new UnityEvent();
        }

        void OnEnable()
        {
            if (listCG == null) { listCG = gameObject.GetComponentInChildren<CanvasGroup>(); }
            if (listRect == null) { listRect = listCG.GetComponent<RectTransform>(); }
            if (updateOnEnable == true && index < items.Count) { SetDropdownIndex(selectedItemIndex); }
            if (contentCG != null) { contentCG.alpha = 1; }

            listCG.alpha = 0;
            listCG.interactable = false;
            listCG.blocksRaycasts = false;
            listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, 0);
            isOn = false;
        }

        void Awake()
        {
            if (initOnAwake == true) { Initialize(); }
            if (useUINavigation == true) { AddUINavigation(); }
           
            if (enableTrigger == true && triggerObject != null)
            {
                // triggerButton = gameObject.GetComponent<Button>();
                triggerEvent = triggerObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { Animate(); });
                triggerEvent.GetComponent<EventTrigger>().triggers.Add(entry);
            }

#if UNITY_2023_2_OR_NEWER
            if (useSounds && audioManager == null && FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None).Length > 0) { audioManager = FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None)[0]; }
#else
            if (useSounds && audioManager == null && FindObjectsOfType(typeof(UIManagerAudio)).Length > 0) { audioManager = (UIManagerAudio)FindObjectsOfType(typeof(UIManagerAudio))[0]; }
#endif
            else { useSounds = false; }

            if (highlightCG == null)
            {
                highlightCG = new GameObject().AddComponent<CanvasGroup>();
                highlightCG.gameObject.AddComponent<RectTransform>(); 
                highlightCG.transform.SetParent(transform);
                highlightCG.gameObject.name = "Highlight";
            }

            if (gameObject.GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

            if (setHighPriority == true)
            {
                if (contentCG == null) { contentCG = transform.Find("Content/Item List").GetComponent<CanvasGroup>(); }
                contentCG.alpha = 1;

                Canvas tempCanvas = contentCG.gameObject.AddComponent<Canvas>();
                tempCanvas.overrideSorting = true;
                tempCanvas.sortingOrder = 30000;
                contentCG.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        public void Initialize()
        {
            if (items.Count == 0) { return; }
            if (enableScrollbar == false && scrollbar != null) { Destroy(scrollbar); }
            if (itemList == null) { itemList = itemParent.GetComponent<VerticalLayoutGroup>(); }

            UpdateItemLayout();
            index = 0;

            foreach (Transform child in itemParent) { Destroy(child.gameObject); }
            for (int i = 0; i < items.Count; ++i)
            {
                GameObject go = Instantiate(itemPreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);
                go.name = items[i].itemName;

                ButtonManager goBtn = go.GetComponent<ButtonManager>();           
                goBtn.buttonText = items[i].itemName;
                goBtn.bypassUpdateOnEnable = true;
                items[i].itemButton = goBtn;

                if (items[i].isInvisible == true)
                {
                    go.SetActive(false);
                    continue;
                }

                LocalizedObject loc = goBtn.GetComponent<LocalizedObject>();
                if (string.IsNullOrEmpty(items[i].localizationKey) && loc != null) { Destroy(loc); }
                else if(!string.IsNullOrEmpty(items[i].localizationKey) && loc != null)
                {
                    loc.localizationKey = items[i].localizationKey;
                    loc.onLanguageChanged.AddListener(delegate
                    {
                        goBtn.buttonText = loc.GetKeyOutput(loc.localizationKey);
                        goBtn.UpdateUI();
                    });
                    loc.InitializeItem();
                    loc.UpdateItem();
                }

                if (items[i].itemIcon == null) { goBtn.enableIcon = false; }
                else { goBtn.enableIcon = true; goBtn.buttonIcon = items[i].itemIcon; }

                goBtn.UpdateUI();
              
                items[i].itemIndex = i;
                Item mainItem = items[i];

                goBtn.onClick.AddListener(Animate);
                goBtn.onClick.AddListener(items[index = mainItem.itemIndex].onItemSelection.Invoke);
                goBtn.onClick.AddListener(delegate
                {
                    SetDropdownIndex(index = mainItem.itemIndex);
                    onValueChanged.Invoke(index = mainItem.itemIndex);
                    if (saveSelected == true) { PlayerPrefs.SetInt("Dropdown_" + saveKey, mainItem.itemIndex); }
                });
            }

            if (headerImage != null && enableIcon == false) { headerImage.gameObject.SetActive(false); }
            else if (headerImage != null) { headerImage.sprite = items[selectedItemIndex].itemIcon; }
            if (headerText != null) { headerText.text = items[selectedItemIndex].itemName; }
            
            if (saveSelected == true)
            {
                if (invokeOnAwake == true) { items[PlayerPrefs.GetInt("Dropdown_" + saveKey)].onItemSelection.Invoke(); }
                else { SetDropdownIndex(PlayerPrefs.GetInt("Dropdown_" + saveKey)); }
            }
            else if(invokeOnAwake == true) { items[selectedItemIndex].onItemSelection.Invoke(); }
        }

        public void SetDropdownIndex(int itemIndex)
        {
            selectedItemIndex = itemIndex;
           
            if (headerText != null) { headerText.text = items[itemIndex].itemButton.buttonText; }
            if (items[itemIndex].isInvisible == true) { return; }

            if (headerImage != null && enableIcon == true && items[itemIndex].itemButton.enableIcon == true) { headerImage.gameObject.SetActive(true); headerImage.sprite = items[itemIndex].itemButton.buttonIcon; }
            else if (headerImage != null && enableIcon == true && items[itemIndex].itemButton.enableIcon == false) { headerImage.gameObject.SetActive(false); }      
        }

        public void Animate()
        {
            if (isOn == false)
            {
                if (enableScrollbar == true && scrollbar != null && startAtBottom == true) 
                { 
                    scrollbar.value = 0; 
                }

                isOn = true;
                listCG.blocksRaycasts = true;
                listCG.interactable = true;
                listCG.gameObject.SetActive(true);

                StopCoroutine("StartMinimize");
                StopCoroutine("StartExpand");
                StartCoroutine("StartExpand");
            }

            else if (isOn == true)
            {
                isOn = false;
                listCG.blocksRaycasts = false;
                listCG.interactable = false;

                StopCoroutine("StartMinimize");
                StopCoroutine("StartExpand");
                StartCoroutine("StartMinimize");
            }

            if (enableTrigger == true && triggerObject != null && isOn == false) { triggerObject.SetActive(false); }
            else if (enableTrigger == true && triggerObject != null && isOn == true) { triggerObject.SetActive(true); }
            if (enableTrigger == true && outOnPointerExit == true && triggerObject != null) { triggerObject.SetActive(false); }
        }

        public void AddUINavigation()
        {
            if (targetButton == null)
            {
                targetButton = gameObject.AddComponent<Button>();
                targetButton.transition = Selectable.Transition.None;
            }

            Navigation customNav = new Navigation();
            customNav.mode = navigationMode;

            if (navigationMode == Navigation.Mode.Vertical || navigationMode == Navigation.Mode.Horizontal) { customNav.wrapAround = wrapAround; }
            else if (navigationMode == Navigation.Mode.Explicit) { StartCoroutine("InitUINavigation", customNav); return; }

            targetButton.navigation = customNav;
        }

        public void DisableUINavigation()
        {
            if (targetButton != null)
            {
                Navigation customNav = new Navigation();
                Navigation.Mode navMode = Navigation.Mode.None;
                customNav.mode = navMode;
                targetButton.navigation = customNav;
            }
        }

        public void Interactable(bool value)
        {
            isInteractable = value;
            if (gameObject.activeInHierarchy == false) { return; }
            StartCoroutine("SetNormal");
        }

        public void CreateNewItem(string title, Sprite icon, bool notify)
        {
            Item item = new Item();
            item.itemName = title;
            item.itemIcon = icon;
            items.Add(item);
            if (notify == true) { Initialize(); }
        }

        public void CreateNewItem(string title, bool notify)
        {
            Item item = new Item();
            item.itemName = title;
            items.Add(item);
            if (notify == true) { Initialize(); }
        }

        public void CreateNewItem(string title)
        {
            Item item = new Item();
            item.itemName = title;
            items.Add(item);
            Initialize();
        }

        public void RemoveItem(string itemTitle, bool notify)
        {
            var item = items.Find(x => x.itemName == itemTitle);
            items.Remove(item);
            if (notify == true) { Initialize(); }
        }

        public void RemoveItem(string itemTitle)
        {
            var item = items.Find(x => x.itemName == itemTitle);
            items.Remove(item);
            Initialize();
        }

        public void UpdateItemLayout()
        {
            if (itemList == null)
                return;

            itemList.spacing = itemSpacing;
            itemList.padding.top = itemPaddingTop;
            itemList.padding.bottom = itemPaddingBottom;
            itemList.padding.left = itemPaddingLeft;
            itemList.padding.right = itemPaddingRight;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
            
            Animate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }

            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (outOnPointerExit == true && isOn == true) { Animate(); }

            StartCoroutine("SetNormal");
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }

            StartCoroutine("SetHighlight");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (isInteractable == false)
                return;

            StartCoroutine("SetNormal");
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (isInteractable == false) { return; }
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
            if (EventSystem.current.currentSelectedGameObject != gameObject) { StartCoroutine("SetNormal"); }
        }

        IEnumerator StartExpand()
        {
            float elapsedTime = 0;
            Vector2 startPos = listRect.sizeDelta;
            Vector2 endPos = new Vector2(listRect.sizeDelta.x, panelSize);

            while (listRect.sizeDelta.y <= panelSize - 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                listCG.alpha += Time.unscaledDeltaTime * (curveSpeed * 2);
                listRect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));
                yield return null;
            }

            listCG.alpha = 1;
            listRect.sizeDelta = endPos;
        }

        IEnumerator StartMinimize()
        {
            float elapsedTime = 0;
            Vector2 startPos = listRect.sizeDelta;
            Vector2 endPos = new Vector2(listRect.sizeDelta.x, 0);

            while (listRect.sizeDelta.y >= 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                listCG.alpha -= Time.unscaledDeltaTime * (curveSpeed * 2);
                listRect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));

                yield return null;
            }

            listCG.alpha = 0;
            listRect.sizeDelta = endPos;
            listCG.gameObject.SetActive(false);
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetHighlight");

            while (highlightCG.alpha > 0.01f)
            {
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            highlightCG.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetNormal");

            while (highlightCG.alpha < 0.99f)
            {
                highlightCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            highlightCG.alpha = 1;
        }
    }
}