using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CreditsManager : MonoBehaviour
    {
        // Content
        [SerializeField] private CreditsPreset creditsPreset;

        // Resources
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private VerticalLayoutGroup creditsListParent;
        [SerializeField] private Scrollbar scrollHelper;
        [SerializeField] private GameObject creditsSectionPreset;
        [SerializeField] private GameObject creditsMentionPreset;

        // Settings
        [SerializeField] private bool closeAutomatically = true;
        [SerializeField] [Range(1, 15)] private float fadingMultiplier = 4;
        [SerializeField] [Range(0, 10)] private float scrollDelay = 1.25f;
        [Range(0, 0.5f)] public float scrollSpeed = 0.05f;
        [Range(1.1f, 15)] public float boostValue = 3f;
        [SerializeField] private InputAction boostHotkey;

        // Events
        public UnityEvent onOpen = new UnityEvent();
        public UnityEvent onClose = new UnityEvent();
        public UnityEvent onCreditsEnd = new UnityEvent();

        bool isOpen = false;
        bool enableScrolling;
        bool invokedEndEvents;

        void Awake()
        {
            InitCredits();
            boostHotkey.Enable();
            if (closeAutomatically == true) { onCreditsEnd.AddListener(() => ClosePanel()); }
        }

        void OnEnable()
        {
            StartScrolling();
        }

        void OnDisable()
        {
            invokedEndEvents = false;
            enableScrolling = false;
        }

        void Update()
        {
            if (enableScrolling == false)
                return;

            if (boostHotkey.IsInProgress()) { scrollHelper.value -= (scrollSpeed * boostValue) * Time.deltaTime; }
            else { scrollHelper.value -= scrollSpeed * Time.deltaTime; }

            if (scrollHelper.value <= 0.005f && invokedEndEvents == false) { onCreditsEnd.Invoke(); invokedEndEvents = true; }
            if (scrollHelper.value <= 0) { enableScrolling = false; onCreditsEnd.Invoke(); }
        }

        void InitCredits()
        {
            if (creditsPreset == null)
            {
                Debug.LogWarning("'Credits Preset' is missing.", this);
                return;
            }

            backgroundImage.sprite = creditsPreset.backgroundSprite;

            foreach (Transform child in creditsListParent.transform)
            {
                if (child.GetComponent<CreditsSectionItem>() != null || child.GetComponent<CreditsMentionItem>() != null)
                    Destroy(child.gameObject);
            }

            for (int i = 0; i < creditsPreset.credits.Count; ++i)
            {
                GameObject go = Instantiate(creditsSectionPreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(creditsListParent.transform, false);
                go.name = creditsPreset.credits[i].headerTitle;

                CreditsSectionItem csi = go.GetComponent<CreditsSectionItem>();
                csi.preset = creditsPreset;
                csi.SetHeader(creditsPreset.credits[i].headerTitle);
                if (creditsPreset.credits[i].items.Count < 2) { csi.headerLayout.padding.bottom = 0; }
                if (!string.IsNullOrEmpty(creditsPreset.credits[i].headerTitleKey)) { csi.CheckForLocalization(creditsPreset.credits[i].headerTitleKey); }
                foreach (string txt in creditsPreset.credits[i].items) { csi.AddNameToList(txt); }
                Destroy(csi.namePreset);
                csi.UpdateLayout();
            }

            for (int i = 0; i < creditsPreset.mentions.Count; ++i)
            {
                GameObject go = Instantiate(creditsMentionPreset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(creditsListParent.transform, false);
                go.name = creditsPreset.mentions[i].ID;

                CreditsMentionItem cmi = go.GetComponent<CreditsMentionItem>();
                cmi.preset = creditsPreset;
                cmi.UpdateLayout(creditsPreset.mentions[i].layoutSpacing, creditsPreset.mentions[i].descriptionSpacing);
                if (i == 0) { cmi.listLayout.padding.top = cmi.listLayout.padding.top * 2; }
                cmi.SetIcon(creditsPreset.mentions[i].icon);
                cmi.SetDescription(creditsPreset.mentions[i].description);
                if (!string.IsNullOrEmpty(creditsPreset.mentions[i].descriptionKey)) { cmi.CheckForLocalization(creditsPreset.mentions[i].descriptionKey); }
            }

            creditsListParent.padding.bottom = (int)(Screen.currentResolution.height / 1.26f);
            StartCoroutine("FixListLayout");
        }

        void StartScrolling()
        {
            if (enableScrolling == true)
                return;

            StopCoroutine("StartTimer");

            enableScrolling = false;
            scrollHelper.value = 1;

            if (scrollDelay != 0) { StartCoroutine("StartTimer"); }
            else { enableScrolling = true; }
        }

        public void OpenPanel() 
        {
            if (isOpen == true)
                return;

            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            isOpen = true;
            onOpen.Invoke();

            StopCoroutine("SetInvisible");
            StartCoroutine("SetVisible");
        }

        public void ClosePanel() 
        {
            if (isOpen == false)
                return;

            onClose.Invoke();
            isOpen = false;

            StopCoroutine("SetVisible");
            StartCoroutine("SetInvisible");
        }

        public void EnableScrolling(bool state)
        {
            if (state == false) { enableScrolling = false; }
            else { enableScrolling = true; }
        }

        IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(scrollDelay);
            enableScrolling = true;
        }

        IEnumerator FixListLayout()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(creditsListParent.GetComponent<RectTransform>());
        }

        IEnumerator SetVisible()
        {
            StopCoroutine("SetInvisible");

            while (canvasGroup.alpha < 0.99f)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            canvasGroup.alpha = 1;
        }

        IEnumerator SetInvisible()
        {
            StopCoroutine("SetVisible");

            while (canvasGroup.alpha > 0.01f)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            canvasGroup.alpha = 0;
            scrollHelper.value = 1;
            gameObject.SetActive(false);
        }
    }
}