using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(Scrollbar))]
    public class ScrollbarInputHandler : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private ControllerManager controllerManager;
        [SerializeField] private Scrollbar scrollbarObject;
        [SerializeField] private GameObject indicator;

        [Header("Settings")]
        [SerializeField] private ScrollbarDirection direction = ScrollbarDirection.Vertical;
        [Range(0.1f, 50)] public float scrollSpeed = 3;
        [SerializeField] [Range(0.01f, 1)] private float deadzone = 0.1f;
        [SerializeField] private bool optimizeUpdates = true;
        public bool allowInputs = true;
        [SerializeField] private bool reversePosition;

        // Helpers
        float divideValue = 1000;

        public enum ScrollbarDirection { Horizontal, Vertical }

        void OnEnable()
        {
            if (scrollbarObject == null) { scrollbarObject = gameObject.GetComponent<Scrollbar>(); }
#if UNITY_2023_2_OR_NEWER
            if (controllerManager == null && FindObjectsByType<ControllerManager>(FindObjectsSortMode.None).Length > 0) { controllerManager = FindObjectsByType<ControllerManager>(FindObjectsSortMode.None)[0]; }
#else
            if (controllerManager == null && FindObjectsOfType(typeof(ControllerManager)).Length > 0) { controllerManager = (ControllerManager)FindObjectsOfType(typeof(ControllerManager))[0]; }
#endif
            else if (controllerManager == null) { Destroy(this); }

            if (indicator == null)
            {
                indicator = new GameObject();
                indicator.name = "[Generated Indicator]";
                indicator.transform.SetParent(transform);
            }

            indicator.SetActive(false);
        }

        void Update()
        {
            if (Gamepad.current == null || controllerManager == null || allowInputs == false) { indicator.SetActive(false); return; }
            else if (optimizeUpdates == true && controllerManager != null && controllerManager.gamepadEnabled == false) { indicator.SetActive(false); return; }

            indicator.SetActive(true);

            if (direction == ScrollbarDirection.Vertical)
            {
                if (reversePosition == true && controllerManager.vAxis >= 0.1f) { scrollbarObject.value -= (scrollSpeed / divideValue) * controllerManager.vAxis; }
                else if (reversePosition == false && controllerManager.vAxis >= 0.1f) { scrollbarObject.value += (scrollSpeed / divideValue) * controllerManager.vAxis; }
                else if (reversePosition == true && controllerManager.vAxis <= -0.1f) { scrollbarObject.value += (scrollSpeed / divideValue) * Mathf.Abs(controllerManager.vAxis); }
                else if (reversePosition == false && controllerManager.vAxis <= -0.1f) { scrollbarObject.value -= (scrollSpeed / divideValue) * Mathf.Abs(controllerManager.vAxis); }
            }

            else if (direction == ScrollbarDirection.Horizontal)
            {
                if (reversePosition == true && controllerManager.hAxis >= deadzone) { scrollbarObject.value -= (scrollSpeed / divideValue) * controllerManager.hAxis; }
                else if (reversePosition == false && controllerManager.hAxis >= deadzone) { scrollbarObject.value += (scrollSpeed / divideValue) * controllerManager.hAxis; }
                else if (reversePosition == true && controllerManager.hAxis <= -deadzone) { scrollbarObject.value += (scrollSpeed / divideValue) * Mathf.Abs(controllerManager.hAxis); }
                else if (reversePosition == false && controllerManager.hAxis <= -deadzone) { scrollbarObject.value -= (scrollSpeed / divideValue) * Mathf.Abs(controllerManager.hAxis); }
            }
        }
    }
}