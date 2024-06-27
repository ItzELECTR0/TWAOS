using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        // Content
        public Sprite icon;
        public float currentValue;
        public float minValue = 0;
        public float maxValue = 100;
        public float minValueLimit = 0;
        public float maxValueLimit = 100;

        // Resources
        public Image barImage;
        [SerializeField] private Image iconObject;
        [SerializeField] private Image altIconObject;
        [SerializeField] private TextMeshProUGUI textObject;
        [SerializeField] private TextMeshProUGUI altTextObject;

        // Settings
        public bool addPrefix;
        public bool addSuffix;
        [Range(0, 5)] public int decimals = 0;
        public string prefix = "";
        public string suffix = "%";
        public BarDirection barDirection = BarDirection.Left;

        // Events
        [System.Serializable]
        public class OnValueChanged : UnityEvent<float> { }
        public OnValueChanged onValueChanged;

        // Helpers
        [HideInInspector] public Slider eventSource;

        public enum BarDirection { Left, Right, Top, Bottom }

        void Start()
        {
            Initialize();
            UpdateUI();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Application.isPlaying == true)
                return;

            UpdateUI();
            SetBarDirection();
        }
#endif

        public void UpdateUI()
        {
            if (barImage != null) { barImage.fillAmount = currentValue / maxValue; }
            if (iconObject != null) { iconObject.sprite = icon; }
            if (altIconObject != null) { altIconObject.sprite = icon; }
            if (textObject != null) { UpdateText(textObject); }
            if (altTextObject != null) { UpdateText(altTextObject); }
            if (eventSource != null) { eventSource.value = currentValue; }
        }

        void UpdateText(TextMeshProUGUI txt)
        {
            if (addSuffix == true) { txt.text = currentValue.ToString("F" + decimals) + suffix; }
            else { txt.text = currentValue.ToString("F" + decimals); }

            if (addPrefix == true) { txt.text = prefix + txt.text; }
        }

        public void Initialize()
        {
            if (Application.isPlaying == true && onValueChanged.GetPersistentEventCount() != 0)
            {
                if (eventSource == null) { eventSource = gameObject.AddComponent(typeof(Slider)) as Slider; }
                eventSource.transition = Selectable.Transition.None;
                eventSource.minValue = minValue;
                eventSource.maxValue = maxValue;
                eventSource.onValueChanged.AddListener(onValueChanged.Invoke);
            }

            SetBarDirection();
        }

        void SetBarDirection()
        {
            if (barImage != null)
            {
                barImage.type = Image.Type.Filled;
                if (barDirection == BarDirection.Left) { barImage.fillMethod = Image.FillMethod.Horizontal; barImage.fillOrigin = 0; }
                else if (barDirection == BarDirection.Right) { barImage.fillMethod = Image.FillMethod.Horizontal; barImage.fillOrigin = 1; }
                else if (barDirection == BarDirection.Top) { barImage.fillMethod = Image.FillMethod.Vertical; barImage.fillOrigin = 1; }
                else if (barDirection == BarDirection.Bottom) { barImage.fillMethod = Image.FillMethod.Vertical; barImage.fillOrigin = 0; }
            }
        }

        public void ClearEvents() { eventSource.onValueChanged.RemoveAllListeners(); }
        public void SetValue(float newValue) { currentValue = newValue; UpdateUI(); }
    }
}