using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.Heat
{
    public class SwitchManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Resources
        [SerializeField] private Animator switchAnimator;
        [SerializeField] private CanvasGroup highlightCG;
        [SerializeField] private UIManagerAudio audioManager;

        // Saving
        public bool saveValue = false;
        public string saveKey = "My Switch";

        // Settings
        public bool isOn = true;
        public bool isInteractable = true;
        public bool invokeAtStart = true;
        public bool useSounds = true;
        public bool useUINavigation = false;
        [Range(1, 15)] public float fadingMultiplier = 8;

        // Events
        [SerializeField] public SwitchEvent onValueChanged = new SwitchEvent();
        public UnityEvent onEvents;
        public UnityEvent offEvents;

        [System.Serializable]
        public class SwitchEvent : UnityEvent<bool> { }

        bool isInitialized = false;

        void Awake()
        {
            if (saveValue == true) { GetSavedData(); }
            else
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("DisableAnimator"); }
                if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }

                switchAnimator.enabled = true;

                if (isOn == true) { switchAnimator.Play("On Instant"); }
                else { switchAnimator.Play("Off Instant"); }
            }

            if (gameObject.GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

#if UNITY_2023_2_OR_NEWER
            if (useSounds && audioManager == null && FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None).Length > 0) { audioManager = FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None)[0]; }
#else
            if (useSounds && audioManager == null && FindObjectsOfType(typeof(UIManagerAudio)).Length > 0) { audioManager = (UIManagerAudio)FindObjectsOfType(typeof(UIManagerAudio))[0]; }
#endif
            else { useSounds = false; }

            if (useUINavigation == true) { AddUINavigation(); }
            if (highlightCG == null) { highlightCG = new GameObject().AddComponent<CanvasGroup>(); highlightCG.transform.SetParent(transform); highlightCG.gameObject.name = "Highlighted"; }

            if (invokeAtStart == true && isOn == true) { onEvents.Invoke(); onValueChanged.Invoke(isOn); }
            else if (invokeAtStart == true && isOn == false) { offEvents.Invoke(); onValueChanged.Invoke(isOn); }

            isInitialized = true;
        }

        void OnEnable() 
        { 
            if (isInitialized == true) { UpdateUI(); } 
        }

        void GetSavedData()
        {
            if (gameObject.activeInHierarchy == true) { StopCoroutine("DisableAnimator"); }
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }

            switchAnimator.enabled = true;

            if (PlayerPrefs.GetString("Switch_" + saveKey) == "" || PlayerPrefs.HasKey("Switch_" + saveKey) == false)
            {
                if (isOn == true) { switchAnimator.Play("Off"); PlayerPrefs.SetString("Switch_" + saveKey, "true"); }
                else { switchAnimator.Play("Off"); PlayerPrefs.SetString("Switch_" + saveKey, "false"); }
            }
            else if (PlayerPrefs.GetString("Switch_" + saveKey) == "true") { switchAnimator.Play("Off"); isOn = true; }
            else if (PlayerPrefs.GetString("Switch_" + saveKey) == "false") { switchAnimator.Play("Off"); isOn = false; }
        }

        public void AddUINavigation()
        {
            Button navButton = gameObject.AddComponent<Button>();
            navButton.transition = Selectable.Transition.None;
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Automatic;
            navButton.navigation = customNav;
        }

        public void AnimateSwitch()
        {
            if (gameObject.activeInHierarchy == true) { StopCoroutine("DisableAnimator"); }
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }

            switchAnimator.enabled = true;

            if (isOn == true)
            {
                switchAnimator.Play("Off");
                isOn = false;
                offEvents.Invoke();

                if (saveValue == true) { PlayerPrefs.SetString("Switch_" + saveKey, "false"); }
            }

            else
            {
                switchAnimator.Play("On");
                isOn = true;
                onEvents.Invoke();

                if (saveValue == true) { PlayerPrefs.SetString("Switch_" + saveKey, "true"); }
            }

            onValueChanged.Invoke(isOn);
        }

        public void SetOn()
        {
            if (gameObject.activeInHierarchy == true) { StopCoroutine("DisableAnimator"); }
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
            if (saveValue == true) { PlayerPrefs.SetString("Switch_" + saveKey, "true"); }

            switchAnimator.enabled = true;
            switchAnimator.Play("On");

            isOn = true;
            onEvents.Invoke();
            onValueChanged.Invoke(true);
        }

        public void SetOff()
        {
            if (gameObject.activeInHierarchy == true) { StopCoroutine("DisableAnimator"); }
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
            if (saveValue == true) { PlayerPrefs.SetString("Switch_" + saveKey, "false"); }

            switchAnimator.enabled = true;
            switchAnimator.Play("Off");

            isOn = false;
            offEvents.Invoke();
            onValueChanged.Invoke(false);
        }

        public void UpdateUI()
        {
            if (gameObject.activeInHierarchy == true) { StopCoroutine("DisableAnimator"); }
            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }

            switchAnimator.enabled = true;

            if (isOn == true && switchAnimator.gameObject.activeInHierarchy == true) { switchAnimator.Play("On Instant"); }
            else if (isOn == false && switchAnimator.gameObject.activeInHierarchy == true) { switchAnimator.Play("Off Instant"); }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractable == false || eventData.button != PointerEventData.InputButton.Left) { return; }
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }

            AnimateSwitch();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }
            if (isInteractable == false) { return; }

            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractable == false)
                return;

            StartCoroutine("SetNormal");
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (isInteractable == false)
                return;

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
            if (isInteractable == false)
                return;

            AnimateSwitch();
            StartCoroutine("SetNormal");
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(0.5f);
            switchAnimator.enabled = false;
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