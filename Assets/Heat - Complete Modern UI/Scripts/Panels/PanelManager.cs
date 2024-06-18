using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.Heat
{
    public class PanelManager : MonoBehaviour
    {
        // Content
        public List<PanelItem> panels = new List<PanelItem>();

        // Settings
        public int currentPanelIndex = 0;
        private int currentButtonIndex = 0;
        private int newPanelIndex;
        public bool cullPanels = true;
        [SerializeField] private bool initializeButtons = true;
        [SerializeField] private bool bypassAnimationOnEnable = false;
        [SerializeField] private UpdateMode updateMode = UpdateMode.UnscaledTime;
        [SerializeField] private PanelMode panelMode = PanelMode.Custom;
        [Range(0.75f, 2)] public float animationSpeed = 1;

        // Events
        [System.Serializable] public class PanelChangeCallback : UnityEvent<int> { }
        public PanelChangeCallback onPanelChanged = new PanelChangeCallback();

        // Helpers
        Animator currentPanel;
        Animator nextPanel;
        PanelButton currentButton;
        PanelButton nextButton;
        string panelFadeIn = "Panel In";
        string panelFadeOut = "Panel Out";
        string animSpeedKey = "AnimSpeed";
        bool isInitialized = false;
        public float cachedStateLength = 1;
        [HideInInspector] public ControllerManager controllerManager;
        [HideInInspector] public int managerIndex;

        public enum PanelMode { MainPanel, SubPanel, Custom }
        public enum UpdateMode { DeltaTime, UnscaledTime }

        [System.Serializable]
        public class PanelItem
        {
            [Tooltip("[Required] This is the variable that you use to call specific panels.")]
            public string panelName = "My Panel";
            [Tooltip("[Required] Main panel object.")]
            public Animator panelObject;
            [Tooltip("[Optional] If you want the panel manager to have tabbing capability, you can assign a panel button here.")]
            public PanelButton panelButton;
            [Tooltip("[Optional] Alternate panel button variable that supports standard buttons instead of panel buttons.")]
            public ButtonManager altPanelButton;
            [Tooltip("[Optional] Alternate panel button variable that supports box buttons instead of panel buttons.")]
            public BoxButtonManager altBoxButton;
            [Tooltip("[Optional] This is the object that will be selected as the current UI object on panel activation. Useful for gamepad navigation.")]
            public GameObject firstSelected;
            [Tooltip("[Optional] Enables or disables child hotkeys depending on the panel state to avoid conflict between hotkeys.")]
            public Transform hotkeyParent;
            [Tooltip("Enable or disable panel navigation when using the 'Previous' or 'Next' methods.")]
            public bool disableNavigation = false;
            [HideInInspector] public GameObject latestSelected;
            [HideInInspector] public HotkeyEvent[] hotkeys;
        }

        void Awake()
        {
            if (panels.Count == 0)
                return;

            if (panelMode == PanelMode.MainPanel) { cachedStateLength = HeatUIInternalTools.GetAnimatorClipLength(panels[currentPanelIndex].panelObject, "MainPanel_In"); }
            else if (panelMode == PanelMode.SubPanel) { cachedStateLength = HeatUIInternalTools.GetAnimatorClipLength(panels[currentPanelIndex].panelObject, "SubPanel_In"); }
            else if (panelMode == PanelMode.Custom) { cachedStateLength = 1f; }
        }

        void Start()
        {
            if (panels.Count == 0)
                return;

            InitializePanels();
        }

        void OnEnable()
        {
            if (controllerManager != null) 
            { 
                controllerManager.currentManagerIndex = managerIndex; 
            }

            if (bypassAnimationOnEnable)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    if (panels[i].panelObject == null)
                        continue;

                    if (currentPanelIndex == i) 
                    {
                        panels[i].panelObject.gameObject.SetActive(true);
                        panels[i].panelObject.enabled = true;
                        panels[i].panelObject.Play("Panel Instant In"); 
                    }

                    else 
                    { 
                        panels[i].panelObject.gameObject.SetActive(false);
                    }
                }
            }

            else if (isInitialized && !bypassAnimationOnEnable && nextPanel == null)
            {
                currentPanel.enabled = true;
                currentPanel.SetFloat(animSpeedKey, animationSpeed);
                currentPanel.Play(panelFadeIn);
                if (currentButton != null) { currentButton.SetSelected(true); }
            }

            else if (isInitialized && !bypassAnimationOnEnable && nextPanel != null)
            {
                nextPanel.enabled = true;
                nextPanel.SetFloat(animSpeedKey, animationSpeed);
                nextPanel.Play(panelFadeIn);
                if (nextButton != null) { nextButton.SetSelected(true); }
            }

            StopCoroutine("DisablePreviousPanel");
            StopCoroutine("DisableAnimators");
            StartCoroutine("DisableAnimators");
        }

        public void InitializePanels()
        {
            if (panels[currentPanelIndex].panelButton != null)
            {
                currentButton = panels[currentPanelIndex].panelButton;
                currentButton.SetSelected(true);
            }

            currentPanel = panels[currentPanelIndex].panelObject;
            currentPanel.enabled = true;
            currentPanel.gameObject.SetActive(true);

            currentPanel.SetFloat(animSpeedKey, animationSpeed);
            currentPanel.Play(panelFadeIn);

            onPanelChanged.Invoke(currentPanelIndex);
            isInitialized = true;

            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].panelObject == null) { continue; }
                if (panels[i].hotkeyParent != null) { panels[i].hotkeys = panels[i].hotkeyParent.GetComponentsInChildren<HotkeyEvent>(); }
                if (i != currentPanelIndex && cullPanels) { panels[i].panelObject.gameObject.SetActive(false); }
                if (initializeButtons)
                {
                    string tempName = panels[i].panelName;
                    if (panels[i].panelButton != null) { panels[i].panelButton.onClick.AddListener(() => OpenPanel(tempName)); }
                    if (panels[i].altPanelButton != null) { panels[i].altPanelButton.onClick.AddListener(() => OpenPanel(tempName)); }
                    if (panels[i].altBoxButton != null) { panels[i].altBoxButton.onClick.AddListener(() => OpenPanel(tempName)); }
                }
            }

            StopCoroutine("DisableAnimators");
            StartCoroutine("DisableAnimators");
        }

        public void OpenFirstPanel() 
        {
            OpenPanelByIndex(0); 
        }

        public void OpenPanel(string newPanel)
        {
            bool catchedPanel = false;

            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].panelName == newPanel)
                {
                    newPanelIndex = i;
                    catchedPanel = true;
                    break;
                }
            }

            if (catchedPanel == false)
            {
                Debug.LogWarning("There is no panel named '" + newPanel + "' in the panel list.", this);
                return;
            }

            if (newPanelIndex != currentPanelIndex)
            {
                if (cullPanels) { StopCoroutine("DisablePreviousPanel"); }
                if (controllerManager != null) { controllerManager.currentManagerIndex = managerIndex; }

                currentPanel = panels[currentPanelIndex].panelObject;

                if (panels[currentPanelIndex].hotkeyParent != null) { foreach (HotkeyEvent he in panels[currentPanelIndex].hotkeys) { he.enabled = false; } }
                if (panels[currentPanelIndex].panelButton != null) { currentButton = panels[currentPanelIndex].panelButton; }
                if (controllerManager != null && EventSystem.current.currentSelectedGameObject != null) { panels[currentPanelIndex].latestSelected = EventSystem.current.currentSelectedGameObject; }

                currentPanelIndex = newPanelIndex;
                nextPanel = panels[currentPanelIndex].panelObject;
                nextPanel.gameObject.SetActive(true);
           
                currentPanel.enabled = true;
                nextPanel.enabled = true;

                currentPanel.SetFloat(animSpeedKey, animationSpeed);
                nextPanel.SetFloat(animSpeedKey, animationSpeed);

                currentPanel.Play(panelFadeOut);
                nextPanel.Play(panelFadeIn);

                if (cullPanels) { StartCoroutine("DisablePreviousPanel"); }
                if (panels[currentPanelIndex].hotkeyParent != null) { foreach (HotkeyEvent he in panels[currentPanelIndex].hotkeys) { he.enabled = true; } }

                currentButtonIndex = newPanelIndex;

                if (controllerManager != null && panels[currentPanelIndex].latestSelected != null) { controllerManager.SelectUIObject(panels[currentPanelIndex].latestSelected); }
                else if (controllerManager != null && panels[currentPanelIndex].latestSelected == null) { controllerManager.SelectUIObject(panels[currentPanelIndex].firstSelected); }

                if (currentButton != null) { currentButton.SetSelected(false); }
                if (panels[currentButtonIndex].panelButton != null)
                {
                    nextButton = panels[currentButtonIndex].panelButton;
                    nextButton.SetSelected(true);
                }

                onPanelChanged.Invoke(currentPanelIndex);

                StopCoroutine("DisableAnimators");
                StartCoroutine("DisableAnimators");
            }
        }

        public void OpenPanelByIndex(int panelIndex)
        {
            if (panelIndex > panels.Count || panelIndex < 0)
            {
                Debug.LogWarning("Index '" + panelIndex.ToString() + "' doesn't exist.", this);
                return;
            }

            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].panelName == panels[panelIndex].panelName)
                {
                    OpenPanel(panels[panelIndex].panelName);
                    break;
                }
            }
        }

        public void NextPanel()
        {
            if (currentPanelIndex <= panels.Count - 2 && !panels[currentPanelIndex + 1].disableNavigation)
            {
                OpenPanelByIndex(currentPanelIndex + 1);
            }
        }

        public void PreviousPanel()
        {
            if (currentPanelIndex >= 1 && !panels[currentPanelIndex - 1].disableNavigation)
            {
                OpenPanelByIndex(currentPanelIndex - 1);
            }
        }

        public void ShowCurrentPanel()
        {
            if (nextPanel == null) 
            {
                StopCoroutine("DisableAnimators");
                StartCoroutine("DisableAnimators");

                currentPanel.enabled = true;
                currentPanel.SetFloat(animSpeedKey, animationSpeed);
                currentPanel.Play(panelFadeIn); 
            }
          
            else 
            {
                StopCoroutine("DisableAnimators");
                StartCoroutine("DisableAnimators");

                nextPanel.enabled = true;
                nextPanel.SetFloat(animSpeedKey, animationSpeed);
                nextPanel.Play(panelFadeIn); 
            }
        }

        public void HideCurrentPanel()
        {
            if (nextPanel == null) 
            {
                StopCoroutine("DisableAnimators");
                StartCoroutine("DisableAnimators");

                currentPanel.enabled = true;
                currentPanel.SetFloat(animSpeedKey, animationSpeed);
                currentPanel.Play(panelFadeOut); 
            }

            else 
            {
                StopCoroutine("DisableAnimators");
                StartCoroutine("DisableAnimators");

                nextPanel.enabled = true;
                nextPanel.SetFloat(animSpeedKey, animationSpeed);
                nextPanel.Play(panelFadeOut);
            }
        }

        public void ShowCurrentButton()
        {
            if (nextButton == null) { currentButton.SetSelected(true); }
            else { nextButton.SetSelected(true); }
        }

        public void HideCurrentButton()
        {
            if (nextButton == null) { currentButton.SetSelected(false); }
            else { nextButton.SetSelected(false); }
        }

        public void AddNewItem()
        {
            PanelItem panel = new PanelItem();

            if (panels.Count != 0 && panels[panels.Count - 1].panelObject != null)
            {
                int tempIndex = panels.Count - 1;

                GameObject tempPanel = panels[tempIndex].panelObject.transform.parent.GetChild(tempIndex).gameObject;
                GameObject newPanel = Instantiate(tempPanel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

                newPanel.transform.SetParent(panels[tempIndex].panelObject.transform.parent, false);
                newPanel.gameObject.name = "New Panel " + tempIndex.ToString();

                panel.panelName = "New Panel " + tempIndex.ToString();
                panel.panelObject = newPanel.GetComponent<Animator>();

                if (panels[tempIndex].panelButton != null)
                {
                    GameObject tempButton = panels[tempIndex].panelButton.transform.parent.GetChild(tempIndex).gameObject;
                    GameObject newButton = Instantiate(tempButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

                    newButton.transform.SetParent(panels[tempIndex].panelButton.transform.parent, false);
                    newButton.gameObject.name = "New Panel " + tempIndex.ToString();

                    panel.panelButton = newButton.GetComponent<PanelButton>();
                }

                else if (panels[tempIndex].altPanelButton != null)
                {
                    GameObject tempButton = panels[tempIndex].altPanelButton.transform.parent.GetChild(tempIndex).gameObject;
                    GameObject newButton = Instantiate(tempButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

                    newButton.transform.SetParent(panels[tempIndex].panelButton.transform.parent, false);
                    newButton.gameObject.name = "New Panel " + tempIndex.ToString();

                    panel.altPanelButton = newButton.GetComponent<ButtonManager>();
                }

                else if (panels[tempIndex].altBoxButton != null)
                {
                    GameObject tempButton = panels[tempIndex].altBoxButton.transform.parent.GetChild(tempIndex).gameObject;
                    GameObject newButton = Instantiate(tempButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

                    newButton.transform.SetParent(panels[tempIndex].panelButton.transform.parent, false);
                    newButton.gameObject.name = "New Panel " + tempIndex.ToString();

                    panel.altBoxButton = newButton.GetComponent<BoxButtonManager>();
                }
            }

            panels.Add(panel);
        }

        IEnumerator DisablePreviousPanel()
        {
            if (updateMode == UpdateMode.UnscaledTime) { yield return new WaitForSecondsRealtime(cachedStateLength * animationSpeed); }
            else { yield return new WaitForSeconds(cachedStateLength * animationSpeed); }

            for (int i = 0; i < panels.Count; i++)
            {
                if (i == currentPanelIndex)
                    continue;

                panels[i].panelObject.gameObject.SetActive(false);
            }
        }

        IEnumerator DisableAnimators()
        {
            if (updateMode == UpdateMode.UnscaledTime) { yield return new WaitForSecondsRealtime(cachedStateLength * animationSpeed); }
            else { yield return new WaitForSeconds(cachedStateLength * animationSpeed); }

            if (currentPanel != null) { currentPanel.enabled = false; }
            if (nextPanel != null) { nextPanel.enabled = false; }
        }
    }
}