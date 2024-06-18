using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Michsky.UI.Heat
{
    public class SelectorInputHandler : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private ControllerManager controllerManager;
        [SerializeField] private HorizontalSelector selectorObject;
        [SerializeField] private GameObject indicator;

        [Header("Settings")]
        public float selectorCooldown = 0.4f;
        [SerializeField] private bool optimizeUpdates = true;
        public bool requireSelecting = true;

        // Helpers
        bool isInCooldown = false;

        void OnEnable()
        {
#if UNITY_2023_2_OR_NEWER
            if (controllerManager == null && FindObjectsByType<ControllerManager>(FindObjectsSortMode.None).Length > 0) { controllerManager = FindObjectsByType<ControllerManager>(FindObjectsSortMode.None)[0]; }
#else
            if (controllerManager == null && FindObjectsOfType(typeof(ControllerManager)).Length > 0) { controllerManager = (ControllerManager)FindObjectsOfType(typeof(ControllerManager))[0]; }
#endif
            else if (controllerManager == null || selectorObject == null) { Destroy(this); }

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
            if (Gamepad.current == null || controllerManager == null) { indicator.SetActive(false); return; }
            else if (requireSelecting == true && EventSystem.current.currentSelectedGameObject != gameObject) { indicator.SetActive(false); return; }
            else if (optimizeUpdates == true && controllerManager != null && controllerManager.gamepadEnabled == false) { indicator.SetActive(false); return; }
            else if (isInCooldown == true) { return; }

            indicator.SetActive(true);

            if (controllerManager.hAxis >= 0.75)
            {
                selectorObject.NextItem();
                isInCooldown = true;

                StopCoroutine("CooldownTimer");
                StartCoroutine("CooldownTimer");
            }

            else if (controllerManager.hAxis <= -0.75)
            {
                selectorObject.PreviousItem();
                isInCooldown = true;

                StopCoroutine("CooldownTimer");
                StartCoroutine("CooldownTimer");
            }
        }

        IEnumerator CooldownTimer()
        {
            yield return new WaitForSecondsRealtime(selectorCooldown);
            isInCooldown = false;
        }
    }
}