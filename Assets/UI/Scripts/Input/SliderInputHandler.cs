using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Michsky.UI.Heat
{
    public class SliderInputHandler : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private ControllerManager controllerManager;
        [SerializeField] private Slider sliderObject;
        [SerializeField] private GameObject indicator;

        [Header("Settings")]
        [Range(0.1f, 50)] public float valueMultiplier = 1;
        [SerializeField] [Range(0.01f, 1)] private float deadzone = 0.1f;
        [SerializeField] private bool optimizeUpdates = true;
        public bool requireSelecting = true;
        [SerializeField] private bool reversePosition;
        [SerializeField] private bool divideByMaxValue;

        // Helpers
        float divideValue = 1000;

        void OnEnable()
        {
#if UNITY_2023_2_OR_NEWER
            if (controllerManager == null && FindObjectsByType<ControllerManager>(FindObjectsSortMode.None).Length > 0) { controllerManager = FindObjectsByType<ControllerManager>(FindObjectsSortMode.None)[0]; }
#else
            if (controllerManager == null && FindObjectsOfType(typeof(ControllerManager)).Length > 0) { controllerManager = (ControllerManager)FindObjectsOfType(typeof(ControllerManager))[0]; }
#endif
            else if (controllerManager == null || sliderObject == null) { Destroy(this); }

            if (indicator == null)
            {
                indicator = new GameObject();
                indicator.name = "[Generated Indicator]";
                indicator.transform.SetParent(transform);
            }

            indicator.SetActive(false);

            if (divideByMaxValue == true)
            {
                divideValue = sliderObject.maxValue;
            }
        }

        void Update()
        {
            if (Gamepad.current == null || controllerManager == null ) { indicator.SetActive(false); return; }
            else if (requireSelecting == true && EventSystem.current.currentSelectedGameObject != gameObject) { indicator.SetActive(false); return; }
            else if (optimizeUpdates == true && controllerManager != null && controllerManager.gamepadEnabled == false) { indicator.SetActive(false); return; }
            
            indicator.SetActive(true);

            if (reversePosition == true && controllerManager.hAxis >= deadzone) { sliderObject.value -= (valueMultiplier / divideValue) * controllerManager.hAxis; }
            else if (reversePosition == false && controllerManager.hAxis >= deadzone) { sliderObject.value += (valueMultiplier / divideValue) * controllerManager.hAxis; }
            else if (reversePosition == true && controllerManager.hAxis <= -deadzone) { sliderObject.value += (valueMultiplier / divideValue) * Mathf.Abs(controllerManager.hAxis); }
            else if (reversePosition == false && controllerManager.hAxis <= -deadzone) { sliderObject.value -= (valueMultiplier / divideValue) * Mathf.Abs(controllerManager.hAxis); }
        }
    }
}