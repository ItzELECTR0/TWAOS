using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [AddComponentMenu("Heat UI/Input/Hotkey Event")]
    public class HotkeyEvent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Content
        public HotkeyType hotkeyType = HotkeyType.Custom;
        public ControllerPreset controllerPreset;
        public InputAction hotkey;
        public string keyID = "Escape";
        public string hotkeyLabel = "Exit";

        // Resources
        [SerializeField] private GameObject iconParent;
        [SerializeField] private GameObject textParent;
        [SerializeField] private Image iconObj;
        [SerializeField] private TextMeshProUGUI labelObj;
        [SerializeField] private TextMeshProUGUI textObj;
        [SerializeField] private CanvasGroup normalCG;
        [SerializeField] private CanvasGroup highlightCG;
        [SerializeField] private UIManagerAudio audioManager;

        // Settings
        public bool useSounds = false;
        public bool useLocalization = true;
        [Range(1, 15)] public float fadingMultiplier = 8;

        // Events
        public UnityEvent onHotkeyPress = new UnityEvent();

        // Helpers
        bool isInitialized = false;
        ControllerManager controllerManager;
        LocalizedObject localizedObject;

#if UNITY_EDITOR
        public bool showOutputOnEditor = true;
#endif

        public enum HotkeyType { Dynamic, Custom }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && controllerPreset != null && hotkeyType == HotkeyType.Dynamic) { UpdateVisual(); }
            if (!Application.isPlaying) { return; }
#endif
            if (isInitialized == false) { Initialize(); }
#if UNITY_2023_2_OR_NEWER
            if (controllerManager == null && hotkeyType == HotkeyType.Dynamic && FindObjectsByType<ControllerManager>(FindObjectsSortMode.None).Length > 0)
#else
            if (controllerManager == null && hotkeyType == HotkeyType.Dynamic && FindObjectsOfType(typeof(ControllerManager)).Length > 0) 
#endif
            {
#if UNITY_2023_2_OR_NEWER
                controllerManager = FindObjectsByType<ControllerManager>(FindObjectsSortMode.None)[0];
#else
                controllerManager = (ControllerManager)FindObjectsOfType(typeof(ControllerManager))[0];
#endif
                controllerManager.hotkeyObjects.Add(this);
                controllerPreset = controllerManager.currentControllerPreset;
            }

            UpdateUI();
        }

        void Update()
        {
            if (hotkey.triggered) 
            { 
                onHotkeyPress.Invoke();
            }
        }

        void Initialize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) { return; }
#endif
            hotkey.Enable();

            if (hotkeyType == HotkeyType.Dynamic && gameObject.GetComponent<Image>() == null)
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

            if (useLocalization == true)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();

                if (localizedObject == null || localizedObject.CheckLocalizationStatus() == false) { useLocalization = false; }
                else if (localizedObject != null && !string.IsNullOrEmpty(localizedObject.localizationKey))
                {
                    // Forcing object to take the localized output on awake
                    hotkeyLabel = localizedObject.GetKeyOutput(localizedObject.localizationKey);

                    // Change label text on language change
                    localizedObject.onLanguageChanged.AddListener(delegate
                    {
                        hotkeyLabel = localizedObject.GetKeyOutput(localizedObject.localizationKey);
                        SetLabel(hotkeyLabel);
                    });
                }
            }

            if (useSounds == true && audioManager != null)
            {
                onHotkeyPress.AddListener(delegate { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); });
            }

            isInitialized = true;
        }

        public void UpdateUI()
        {
            if (hotkeyType == HotkeyType.Custom)
                return;

            if (hotkeyType == HotkeyType.Dynamic)
            {
                if (controllerPreset == null) { return; }
                if (highlightCG != null) { highlightCG.alpha = 0; }

                bool keyFound = false;

                for (int i = 0; i < controllerPreset.items.Count; i++)
                {
                    if (controllerPreset.items[i].itemID == keyID)
                    {
                        keyFound = true;
                        gameObject.SetActive(true);
                        if (labelObj != null) { labelObj.text = hotkeyLabel; }

                        if (controllerPreset.items[i].itemType == ControllerPreset.ItemType.Icon)
                        {
                            if (iconParent != null) { iconParent.SetActive(true); }
                            if (textParent != null) { textParent.SetActive(false); }
                            if (iconObj != null) { iconObj.sprite = controllerPreset.items[i].itemIcon; }
                        }

                        else if (controllerPreset.items[i].itemType == ControllerPreset.ItemType.Text)
                        {
                            if (iconParent != null) { iconParent.SetActive(false); }
                            if (textParent != null) { textParent.SetActive(true); }
                            if (textObj != null) { textObj.text = controllerPreset.items[i].itemText; }
                        }

                        break;
                    }
                }

                if (keyFound == false) 
                {
                    // gameObject.SetActive(false);
                    return;
                }
            }

            if (gameObject.activeInHierarchy == true && Application.isPlaying) { StartCoroutine("LayoutFix"); }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
            }
        }

#if UNITY_EDITOR
        public void UpdateVisual()
        {
            if (controllerPreset == null) { return; }
            if (highlightCG != null) { highlightCG.alpha = 0; }
            for (int i = 0; i < controllerPreset.items.Count; i++)
            {
                if (controllerPreset.items[i].itemID == keyID)
                {
                    if (controllerPreset.items[i].itemType == ControllerPreset.ItemType.Icon)
                    {
                        if (iconParent != null) { iconParent.SetActive(true); }
                        if (textParent != null) { textParent.SetActive(false); }
                        if (iconObj != null) { iconObj.sprite = controllerPreset.items[i].itemIcon; }
                    }

                    else if (controllerPreset.items[i].itemType == ControllerPreset.ItemType.Text)
                    {
                        if (iconParent != null) { iconParent.SetActive(false); }
                        if (textParent != null) { textParent.SetActive(true); }
                        if (textObj != null) { textObj.text = controllerPreset.items[i].itemText; }
                    }

                    if (labelObj != null) { labelObj.text = hotkeyLabel; }
                    break;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
        }
#endif

        public void SetLabel(string value)
        {
            if (labelObj == null)
                return;

            labelObj.text = value;

            if (gameObject.activeInHierarchy == true) { StartCoroutine("LayoutFix"); }
            else
            {
                if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onHotkeyPress.Invoke();

            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
            if (normalCG == null || highlightCG == null || gameObject.activeInHierarchy == false) { return; }

            StopCoroutine("SetHighlight");
            StartCoroutine("SetNormal");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (audioManager != null && useSounds == true) { audioManager.audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }
            if (normalCG == null || highlightCG == null) { return; }

            StopCoroutine("SetNormal");
            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (normalCG == null || highlightCG == null)
                return;

            StopCoroutine("SetHighlight");
            StartCoroutine("SetNormal");
        }

        IEnumerator LayoutFix()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            if (normalCG != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCG.GetComponent<RectTransform>()); }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }

        IEnumerator SetNormal()
        {
            while (highlightCG.alpha > 0.01f)
            {
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            highlightCG.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            while (highlightCG.alpha < 0.99f)
            {
                highlightCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            highlightCG.alpha = 1;
        }
    }
}