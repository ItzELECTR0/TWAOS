using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Michsky.UI.Heat
{
    public class UISway : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Resources")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private RectTransform swayObject;

        [Header("Settings")]
        [SerializeField] [Range(1, 20)] private float smoothness = 10;
        [SerializeField] private InputType inputType = InputType.Mouse;

        bool allowSway;
        Vector3 cursorPos;
        Vector2 defaultPos;

        public enum InputType { Mouse, Touchscreen }

        void Start()
        {
            defaultPos = swayObject.anchoredPosition;

            if (mainCamera == null) { mainCamera = Camera.main; }
            if (mainCanvas == null) { mainCanvas = GetComponentInParent<Canvas>(); }
            if (swayObject == null) { swayObject = GetComponent<RectTransform>(); }
        }

        void Update()
        {
            if (allowSway == true && inputType == InputType.Mouse) { cursorPos = Mouse.current.position.ReadValue(); }
            else if (allowSway == true && inputType == InputType.Touchscreen) { cursorPos = Touchscreen.current.position.ReadValue(); }

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay) { ProcessOverlay(); }
            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera) { ProcessSSC(); }
        }

        void ProcessOverlay()
        {
            if (allowSway == true) { swayObject.position = Vector2.Lerp(swayObject.position, cursorPos, Time.unscaledDeltaTime * smoothness); }
            else { swayObject.localPosition = Vector2.Lerp(swayObject.localPosition, defaultPos, Time.unscaledDeltaTime * smoothness); }
        }

        void ProcessSSC()
        {
            if (allowSway == true) { swayObject.position = Vector2.Lerp(swayObject.position, mainCamera.ScreenToWorldPoint(cursorPos), Time.unscaledDeltaTime * smoothness); }
            else { swayObject.localPosition = Vector2.Lerp(swayObject.localPosition, defaultPos, Time.unscaledDeltaTime * smoothness); }
        }

        public void OnPointerEnter(PointerEventData data) { allowSway = true; }
        public void OnPointerExit(PointerEventData data) { allowSway = false; }
    }
}