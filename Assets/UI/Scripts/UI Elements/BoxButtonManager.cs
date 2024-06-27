using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class BoxButtonManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Content
        public Sprite buttonBackground;
        public Sprite buttonIcon;
        public string buttonTitle = "Button";
        public string titleLocalizationKey;
        public string buttonDescription = "Description";
        public string descriptionLocalizationKey;
        public BackgroundFilter backgroundFilter;

        // Resources
        [SerializeField] private Animator animator;
        [SerializeField] private UIManagerAudio audioManager;
        public Image backgroundObj;
        public Image iconObj;
        public TextMeshProUGUI titleObj;
        public TextMeshProUGUI descriptionObj;
        public Image filterObj;
        public List<Sprite> filters = new List<Sprite>();

        // Settings
        public bool isInteractable = true;
        public bool enableBackground = true;
        public bool enableIcon = false;
        public bool enableTitle = true;
        public bool enableDescription = true;
        public bool enableFilter = true;
        public bool useCustomContent = false;
        public bool checkForDoubleClick = true;
        public bool useLocalization = true;
        public bool bypassUpdateOnEnable = false;
        public bool useUINavigation = false;
        public Navigation.Mode navigationMode = Navigation.Mode.Automatic;
        public GameObject selectOnUp;
        public GameObject selectOnDown;
        public GameObject selectOnLeft;
        public GameObject selectOnRight;
        public bool wrapAround = false;
        public bool useSounds = true;
        [Range(0.1f, 1)] public float doubleClickPeriod = 0.25f;

        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onDoubleClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();
        public UnityEvent onSelect = new UnityEvent();
        public UnityEvent onDeselect = new UnityEvent();

        // Helpers
        bool isInitialized = false;
        float cachedStateLength = 0.5f;
        bool waitingForDoubleClickInput;
        Button targetButton;
        LocalizedObject localizedObject;
#if UNITY_EDITOR
        public int latestTabIndex = 0;
#endif

        public enum BackgroundFilter
        {
            Aqua,
            Dawn,
            Dusk,
            Emerald,
            Kylo,
            Memory,
            Mice,
            Pinky,
            Retro,
            Rock,
            Sunset,
            Violet,
            Warm,
            Random
        }

        void Awake()
        {
            cachedStateLength = HeatUIInternalTools.GetAnimatorClipLength(animator, "BoxButton_Highlighted") + 0.1f;
        }

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

        void Initialize()
        {
            if (!Application.isPlaying) { return; }
            if (animator == null) { animator = GetComponent<Animator>(); }
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

            TriggerAnimation("Start");

            if (useLocalization && !useCustomContent)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();

                if (localizedObject == null || !localizedObject.CheckLocalizationStatus()) { useLocalization = false; }
                else if (localizedObject != null && !string.IsNullOrEmpty(titleLocalizationKey))
                {
                    // Forcing button to take the localized output on awake
                    buttonTitle = localizedObject.GetKeyOutput(titleLocalizationKey);

                    // Change button text on language change
                    localizedObject.onLanguageChanged.AddListener(delegate
                    {
                        buttonTitle = localizedObject.GetKeyOutput(titleLocalizationKey);
                        buttonDescription = localizedObject.GetKeyOutput(descriptionLocalizationKey);
                        UpdateUI();
                    });
                }
            }

            isInitialized = true;
        }

        public void UpdateUI()
        {
            if (enableIcon && iconObj != null) { iconObj.gameObject.SetActive(true); iconObj.sprite = buttonIcon; }
            else if (iconObj != null) { iconObj.gameObject.SetActive(false); }

            if (enableTitle && titleObj != null) { titleObj.gameObject.SetActive(true); titleObj.text = buttonTitle; }
            else if (titleObj != null) { titleObj.gameObject.SetActive(false); }

            if (enableDescription && descriptionObj != null) { descriptionObj.gameObject.SetActive(true); descriptionObj.text = buttonDescription; }
            else if (descriptionObj != null) 
            { 
                descriptionObj.gameObject.SetActive(false);
                if (Application.isPlaying && enableTitle && titleObj != null) { titleObj.transform.parent.gameObject.name = titleObj.gameObject.name + "_D"; }
            }

            if (!enableFilter && filterObj != null) { filterObj.gameObject.SetActive(false); }
            else if (enableFilter && filterObj != null && filters.Count >= (int)backgroundFilter + 1)
            {
                filterObj.gameObject.SetActive(true);

                if (backgroundFilter == BackgroundFilter.Random) { filterObj.sprite = filters[(int)Random.Range(0, filters.Count - 2)]; }
                else { filterObj.sprite = filters[(int)backgroundFilter]; }
            }

            if (enableBackground && backgroundObj != null) { backgroundObj.sprite = buttonBackground; }
            if (!Application.isPlaying || !gameObject.activeInHierarchy) { return; }

            TriggerAnimation("Start");
        }

        public void SetText(string text) { buttonTitle = text; UpdateUI(); }
        public void SetDescription(string text) { buttonDescription = text; UpdateUI(); }
        public void SetIcon(Sprite icon) { buttonIcon = icon; UpdateUI(); }
        public void SetBackground(Sprite bg) { buttonBackground = bg; UpdateUI(); }
        public void SetInteractable(bool value) { isInteractable = value; }

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

        public void InvokeOnClick() 
        { 
            onClick.Invoke(); 
        }

        void TriggerAnimation(string triggername)
        {
            animator.enabled = true;

            animator.ResetTrigger("Start");
            animator.ResetTrigger("Normal");
            animator.ResetTrigger("Highlighted");

            animator.SetTrigger(triggername);

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");
        }

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

            TriggerAnimation("Highlighted");
            onHover.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractable)
                return;

            TriggerAnimation("Normal");
            onLeave.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }

            TriggerAnimation("Highlighted");
            onSelect.Invoke();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isInteractable)
                return;

            TriggerAnimation("Normal");
            onDeselect.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isInteractable) { return; }
            if (audioManager != null && useSounds) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
            if (EventSystem.current.currentSelectedGameObject != gameObject) { TriggerAnimation("Normal"); }

            onClick.Invoke();
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

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(cachedStateLength);
            animator.enabled = false;
        }
    }
}