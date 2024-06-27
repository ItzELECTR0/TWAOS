using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ButtonManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Content
        public Sprite buttonIcon;
        public string buttonText = "Button";
        [Range(0.1f, 10)] public float iconScale = 1;
        [Range(1, 200)] public float textSize = 24;

        // Resources
        [SerializeField] private CanvasGroup normalCG;
        [SerializeField] private CanvasGroup highlightCG;
        [SerializeField] private CanvasGroup disabledCG;
        public UIManagerAudio audioManager;
        public TextMeshProUGUI normalTextObj;
        public TextMeshProUGUI highlightTextObj;
        public TextMeshProUGUI disabledTextObj;
        public Image normalImageObj;
        public Image highlightImageObj;
        public Image disabledImageObj;

        // Auto Size
        public bool autoFitContent = true;
        public Padding padding;
        [Range(0, 100)] public int spacing = 12;
        [SerializeField] private HorizontalLayoutGroup disabledLayout;
        [SerializeField] private HorizontalLayoutGroup normalLayout;
        [SerializeField] private HorizontalLayoutGroup highlightedLayout;
        public HorizontalLayoutGroup mainLayout;
        [SerializeField] private ContentSizeFitter mainFitter;
        [SerializeField] private ContentSizeFitter targetFitter;
        [SerializeField] private RectTransform targetRect;

        // Settings
        public bool isInteractable = true;
        public bool enableIcon = false;
        public bool enableText = true;
        public bool useCustomContent = false;
        [SerializeField] private bool useCustomTextSize = false;
        public bool checkForDoubleClick = true;
        public bool useLocalization = true;
        public bool bypassUpdateOnEnable = false;
        public bool bypassControllerManager = false;
        public bool useUINavigation = false;
        public Navigation.Mode navigationMode = Navigation.Mode.Automatic;
        public GameObject selectOnUp;
        public GameObject selectOnDown;
        public GameObject selectOnLeft;
        public GameObject selectOnRight;
        public bool wrapAround = false;
        public bool useSounds = true;
        [Range(0.1f, 1)] public float doubleClickPeriod = 0.25f;
        [Range(1, 15)] public float fadingMultiplier = 8;

        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onDoubleClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();
        public UnityEvent onSelect = new UnityEvent();
        public UnityEvent onDeselect = new UnityEvent();

        // Helpers
        bool isInitialized = false;
        Button targetButton;
        LocalizedObject localizedObject;
        bool waitingForDoubleClickInput;
#if UNITY_EDITOR
        public int latestTabIndex = 0;
#endif

        [System.Serializable] public class Padding { public int left = 18; public int right = 18; public int top = 15; public int bottom = 15; }

        void OnEnable()
        {
            if (!isInitialized) { Initialize(); }
            if (!bypassUpdateOnEnable) { UpdateUI(); }
            if (Application.isPlaying && useUINavigation) { AddUINavigation(); }
            else if (Application.isPlaying && !useUINavigation && targetButton == null)
            {
                targetButton = gameObject.AddComponent<Button>();
                targetButton.transition = Selectable.Transition.None;
            }
        }

        void OnDisable()
        {
            if (!isInteractable)
                return;

            if (disabledCG != null) { disabledCG.alpha = 0; }
            if (normalCG != null) { normalCG.alpha = 1; }
            if (highlightCG != null) { highlightCG.alpha = 0; }
        }

        void Initialize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
#if UNITY_2023_2_OR_NEWER
            if (useSounds && audioManager == null && FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None).Length > 0) { audioManager = FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None)[0]; }
#else
            if (useSounds && audioManager == null && FindObjectsOfType(typeof(UIManagerAudio)).Length > 0) { audioManager = (UIManagerAudio)FindObjectsOfType(typeof(UIManagerAudio))[0]; }
#endif
            else { useSounds = false; }

            if (GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

            if (normalCG == null) { normalCG = new GameObject().AddComponent<CanvasGroup>(); normalCG.gameObject.AddComponent<RectTransform>(); normalCG.transform.SetParent(transform); normalCG.gameObject.name = "Normal"; }
            if (highlightCG == null) { highlightCG = new GameObject().AddComponent<CanvasGroup>(); highlightCG.gameObject.AddComponent<RectTransform>(); highlightCG.transform.SetParent(transform); highlightCG.gameObject.name = "Highlight"; }
            if (disabledCG == null) { disabledCG = new GameObject().AddComponent<CanvasGroup>(); disabledCG.gameObject.AddComponent<RectTransform>(); disabledCG.transform.SetParent(transform); disabledCG.gameObject.name = "Disabled"; }

            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            disabledCG.alpha = 0;

            if (useLocalization && !useCustomContent)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();

                if (localizedObject == null || !localizedObject.CheckLocalizationStatus()) { useLocalization = false; }
                else if (localizedObject != null && !string.IsNullOrEmpty(localizedObject.localizationKey))
                {
                    // Forcing button to take the localized output on awake
                    buttonText = localizedObject.GetKeyOutput(localizedObject.localizationKey);

                    // Change button text on language change
                    localizedObject.onLanguageChanged.AddListener(delegate
                    {
                        buttonText = localizedObject.GetKeyOutput(localizedObject.localizationKey);
                        UpdateUI();
                    });
                }
            }

            isInitialized = true;
        }

        public void UpdateUI()
        {
            if (!autoFitContent)
            {
                if (mainFitter != null) { mainFitter.enabled = false; }
                if (mainLayout != null) { mainLayout.enabled = false; }
                if (disabledLayout != null) { disabledLayout.childForceExpandWidth = false; }
                if (normalLayout != null) { normalLayout.childForceExpandWidth = false; }
                if (highlightedLayout != null) { highlightedLayout.childForceExpandWidth = false; }
                if (targetFitter != null)
                {
                    targetFitter.enabled = false;

                    if (targetRect != null)
                    {
                        targetRect.anchorMin = new Vector2(0, 0);
                        targetRect.anchorMax = new Vector2(1, 1);
                        targetRect.offsetMin = new Vector2(0, 0);
                        targetRect.offsetMax = new Vector2(0, 0);
                    }
                }
            }

            else
            {
                if (disabledLayout != null) { disabledLayout.childForceExpandWidth = true; }
                if (normalLayout != null) { normalLayout.childForceExpandWidth = true; }
                if (highlightedLayout != null) { highlightedLayout.childForceExpandWidth = true; }
                if (mainFitter != null) { mainFitter.enabled = true; }
                if (mainLayout != null) { mainLayout.enabled = true; }
                if (targetFitter != null) { targetFitter.enabled = true; }
            }

            if (disabledLayout != null && autoFitContent) { disabledLayout.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom); disabledLayout.spacing = spacing; }
            if (normalLayout != null && autoFitContent) { normalLayout.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom); normalLayout.spacing = spacing; }
            if (highlightedLayout != null && autoFitContent) { highlightedLayout.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom); highlightedLayout.spacing = spacing; }

            if (enableText)
            {
                if (normalTextObj != null)
                {
                    normalTextObj.gameObject.SetActive(true);
                    normalTextObj.text = buttonText;
                    if (useCustomTextSize == false) { normalTextObj.fontSize = textSize; }
                }

                if (highlightTextObj != null)
                {
                    highlightTextObj.gameObject.SetActive(true);
                    highlightTextObj.text = buttonText;
                    if (useCustomTextSize == false) { highlightTextObj.fontSize = textSize; }
                }

                if (disabledTextObj != null)
                {
                    disabledTextObj.gameObject.SetActive(true);
                    disabledTextObj.text = buttonText;
                    if (useCustomTextSize == false) { disabledTextObj.fontSize = textSize; }
                }
            }

            else if (!enableText)
            {
                if (normalTextObj != null) { normalTextObj.gameObject.SetActive(false); }
                if (highlightTextObj != null) { highlightTextObj.gameObject.SetActive(false); }
                if (disabledTextObj != null) { disabledTextObj.gameObject.SetActive(false); }
            }

            if (enableIcon)
            {
                Vector3 tempScale = new Vector3(iconScale, iconScale, iconScale);
               
                if (normalImageObj != null) { normalImageObj.transform.parent.gameObject.SetActive(true); normalImageObj.sprite = buttonIcon; normalImageObj.transform.localScale = tempScale; }
                if (highlightImageObj != null) { highlightImageObj.transform.parent.gameObject.SetActive(true); highlightImageObj.sprite = buttonIcon; highlightImageObj.transform.localScale = tempScale; }
                if (disabledImageObj != null) { disabledImageObj.transform.parent.gameObject.SetActive(true); disabledImageObj.sprite = buttonIcon; disabledImageObj.transform.localScale = tempScale; }
            }

            else
            {
                if (normalImageObj != null) { normalImageObj.transform.parent.gameObject.SetActive(false); }
                if (highlightImageObj != null) { highlightImageObj.transform.parent.gameObject.SetActive(false); }
                if (disabledImageObj != null) { disabledImageObj.transform.parent.gameObject.SetActive(false); }
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && autoFitContent)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                if (disabledCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(disabledCG.GetComponent<RectTransform>()); }
                if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
                if (highlightCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(highlightCG.GetComponent<RectTransform>()); }
            }
#endif

            if (!Application.isPlaying || !gameObject.activeInHierarchy) { return; }
            if (!isInteractable) { StartCoroutine("SetDisabled"); }
            else if (isInteractable && disabledCG.alpha == 1) { StartCoroutine("SetNormal"); }

            StartCoroutine("LayoutFix");
        }

        public void UpdateState()
        {
            if (!Application.isPlaying || !gameObject.activeInHierarchy) { return; }
            if (!isInteractable) { StartCoroutine("SetDisabled"); }
            else if (isInteractable) { StartCoroutine("SetNormal"); }
        }


        public void SetText(string text) { buttonText = text; UpdateUI(); }
        public void SetIcon(Sprite icon) { buttonIcon = icon; UpdateUI(); }

        public void Interactable(bool value)
        {
            isInteractable = value;

            if (gameObject.activeInHierarchy == false) { return; }
            if (!isInteractable) { StartCoroutine("SetDisabled"); }
            else if (isInteractable && disabledCG.alpha == 1) { StartCoroutine("SetNormal"); }
        }

        public void AddUINavigation()
        {
            if (targetButton == null)
            {
                targetButton = gameObject.AddComponent<Button>();
                targetButton.transition = Selectable.Transition.None;
            }

            if (targetButton.navigation.mode == navigationMode)
                return;

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

        public void InvokeOnClick() { onClick.Invoke(); }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable || eventData.button != PointerEventData.InputButton.Left) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }

            // Invoke click actions
            onClick.Invoke();

            // Check for double click
            if (!checkForDoubleClick) { return; }
            if (waitingForDoubleClickInput)
            {
                onDoubleClick.Invoke();
                waitingForDoubleClickInput = false;
                return;
            }

            waitingForDoubleClickInput = true;
            
            if (gameObject.activeInHierarchy)
            {
                StopCoroutine("CheckForDoubleClick");
                StartCoroutine("CheckForDoubleClick");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isInteractable) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }

            StartCoroutine("SetHighlight");
            onHover.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractable)
                return;

            StartCoroutine("SetNormal");
            onLeave.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }

            StartCoroutine("SetHighlight");
            onSelect.Invoke();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isInteractable)
                return;

            StartCoroutine("SetNormal");
            onDeselect.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isInteractable) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
            if (EventSystem.current.currentSelectedGameObject != gameObject) { StartCoroutine("SetNormal"); }

            onClick.Invoke();
        }

        IEnumerator LayoutFix()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            if (disabledCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(disabledCG.GetComponent<RectTransform>()); }
            if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
            if (highlightCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(highlightCG.GetComponent<RectTransform>()); }
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetHighlight");
            StopCoroutine("SetDisabled");

            while (normalCG.alpha < 0.99f)
            {
                normalCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            disabledCG.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetDisabled");

            while (highlightCG.alpha < 0.99f)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 0;
            highlightCG.alpha = 1;
            disabledCG.alpha = 0;
        }

        IEnumerator SetDisabled()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");

            while (disabledCG.alpha < 0.99f)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 0;
            highlightCG.alpha = 0;
            disabledCG.alpha = 1;
        }

        IEnumerator CheckForDoubleClick()
        {
            yield return new WaitForSecondsRealtime(doubleClickPeriod);
            waitingForDoubleClickInput = false;
        }

        IEnumerator InitUINavigation(Navigation nav)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (selectOnUp != null) { nav.selectOnUp = selectOnUp.GetComponent<Selectable>(); }
            if (selectOnDown != null) { nav.selectOnDown = selectOnDown.GetComponent<Selectable>(); }
            if (selectOnLeft != null) { nav.selectOnLeft = selectOnLeft.GetComponent<Selectable>(); }
            if (selectOnRight != null) { nav.selectOnRight = selectOnRight.GetComponent<Selectable>(); }
            targetButton.navigation = nav;
        }
    }
}