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
    public class PanelButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Content
        public Sprite buttonIcon;
        public string buttonText = "Button";

        // Resources
        [SerializeField] private CanvasGroup disabledCG;
        [SerializeField] private CanvasGroup normalCG;
        [SerializeField] private CanvasGroup highlightCG;
        [SerializeField] private CanvasGroup selectCG;
        [SerializeField] private UIManagerAudio audioManager;
        [SerializeField] private TextMeshProUGUI disabledTextObj;
        [SerializeField] private TextMeshProUGUI normalTextObj;
        [SerializeField] private TextMeshProUGUI highlightTextObj;
        [SerializeField] private TextMeshProUGUI selectTextObj;
        [SerializeField] private Image disabledImageObj;
        [SerializeField] private Image normalImageObj;
        [SerializeField] private Image highlightImageObj;
        [SerializeField] private Image selectedImageObj;
        [SerializeField] private GameObject seperator;

        // Settings
        public bool isInteractable = true;
        public bool isSelected;
        public bool useLocalization = true;
        public bool useCustomText = false;
        public bool useSeperator = true;
        public bool useUINavigation = false;
        public Navigation.Mode navigationMode = Navigation.Mode.Automatic;
        public GameObject selectOnUp;
        public GameObject selectOnDown;
        public GameObject selectOnLeft;
        public GameObject selectOnRight;
        public bool wrapAround = false;
        public bool useSounds = true;
        [Range(1, 15)] public float fadingMultiplier = 8;

        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();
        public UnityEvent onSelect = new UnityEvent();

        // Helpers
        bool isInitialized = false;
        Button targetButton;
        LocalizedObject localizedObject;
        [HideInInspector] public NavigationBar navbar;

        void OnEnable()
        {
            if (!isInitialized) { Initialize(); }
            UpdateUI();
        }

        void Initialize()
        {
            if (!Application.isPlaying)
                return;

#if UNITY_2023_2_OR_NEWER
            if (useSounds && audioManager == null && FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None).Length > 0) { audioManager = FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None)[0]; }
#else
            if (useSounds && audioManager == null && FindObjectsOfType(typeof(UIManagerAudio)).Length > 0) { audioManager = (UIManagerAudio)FindObjectsOfType(typeof(UIManagerAudio))[0]; }
#endif
            else { useSounds = false; }

            if (useUINavigation) { AddUINavigation(); }
            if (gameObject.GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

            disabledCG.alpha = 0;
            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            selectCG.alpha = 0;

            if (useLocalization)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();

                if (localizedObject == null || !localizedObject.CheckLocalizationStatus()) { useLocalization = false; }
                else if (useLocalization && !string.IsNullOrEmpty(localizedObject.localizationKey))
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

        public void IsInteractable(bool value)
        {
            isInteractable = value;

            if (!isInteractable) { StartCoroutine("SetDisabled"); }
            else if (isInteractable && !isSelected) { StartCoroutine("SetNormal"); }
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
        
            onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (navbar != null) { navbar.DimButtons(this); }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }
            if (!isInteractable || isSelected) { return; }

            onHover.Invoke();
            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (navbar != null) { navbar.LitButtons(); }
            if (!isInteractable || isSelected) { return; }

            onLeave.Invoke();
            StartCoroutine("SetNormal");
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetHighlight");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetNormal");
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            onClick.Invoke();
        }

        public void UpdateUI()
        {
            if (useSeperator && transform.parent != null && transform.GetSiblingIndex() != transform.parent.childCount - 1 && seperator != null) { seperator.SetActive(true); }
            else if (seperator != null) { seperator.SetActive(false); }

            if (useCustomText)
                return;

            if (disabledTextObj != null) { disabledTextObj.text = buttonText; }
            if (normalTextObj != null) { normalTextObj.text = buttonText; }
            if (highlightTextObj != null) { highlightTextObj.text = buttonText; }
            if (selectTextObj != null) { selectTextObj.text = buttonText; }

            if (disabledImageObj != null && buttonIcon != null) { disabledImageObj.transform.parent.gameObject.SetActive(true); disabledImageObj.sprite = buttonIcon; }
            else if (disabledImageObj != null && buttonIcon == null) { disabledImageObj.transform.parent.gameObject.SetActive(false); }

            if (normalImageObj != null && buttonIcon != null) { normalImageObj.transform.parent.gameObject.SetActive(true); normalImageObj.sprite = buttonIcon; }
            else if (normalImageObj != null && buttonIcon == null) { normalImageObj.transform.parent.gameObject.SetActive(false); }

            if (highlightImageObj != null && buttonIcon != null) { highlightImageObj.transform.parent.gameObject.SetActive(true); highlightImageObj.sprite = buttonIcon; }
            else if (highlightImageObj != null && buttonIcon == null) { highlightImageObj.transform.parent.gameObject.SetActive(false); }

            if (selectedImageObj != null && buttonIcon != null) { selectedImageObj.transform.parent.gameObject.SetActive(true); selectedImageObj.sprite = buttonIcon; }
            else if (selectedImageObj != null && buttonIcon == null) { selectedImageObj.transform.parent.gameObject.SetActive(false); }

            if (isSelected)
            {
                disabledCG.alpha = 0;
                normalCG.alpha = 0;
                highlightCG.alpha = 0;
                selectCG.alpha = 1;
            }

            else if (!isInteractable)
            {
                disabledCG.alpha = 1;
                normalCG.alpha = 0;
                highlightCG.alpha = 0;
                selectCG.alpha = 0;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void SetSelected(bool value)
        {
            isSelected = value;

            if (navbar != null) { navbar.LitButtons(this); }
            if (isSelected) { StartCoroutine("SetSelect"); onSelect.Invoke(); }
            else { StartCoroutine("SetNormal"); }
        }

        IEnumerator SetDisabled()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");
            StopCoroutine("SetSelect");

            while (disabledCG.alpha < 0.99f)
            {
                disabledCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCG.alpha = 1;
            normalCG.alpha = 0;
            highlightCG.alpha = 0;
            selectCG.alpha = 0;
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetDisabled");
            StopCoroutine("SetHighlight");
            StopCoroutine("SetSelect");

            while (normalCG.alpha < 0.99f)
            {
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCG.alpha = 0;
            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            selectCG.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetDisabled");
            StopCoroutine("SetNormal");
            StopCoroutine("SetSelect");

            while (highlightCG.alpha < 0.99f)
            {
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCG.alpha = 0;
            normalCG.alpha = 0;
            highlightCG.alpha = 1;
            selectCG.alpha = 0;
        }

        IEnumerator SetSelect()
        {
            StopCoroutine("SetDisabled");
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");

            while (selectCG.alpha < 0.99f)
            {
                disabledCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCG.alpha = 0;
            normalCG.alpha = 0;
            highlightCG.alpha = 0;
            selectCG.alpha = 1;
        }
    }
}