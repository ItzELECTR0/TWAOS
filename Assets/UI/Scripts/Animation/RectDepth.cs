using UnityEngine;
using UnityEngine.InputSystem;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Heat UI/Animation/Rect Depth")]
    public class RectDepth : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Range(0.05f, 1)] private float smoothness = 0.25f;
        [SerializeField] [Range(0.5f, 10)] private float multiplier = 2;
        [SerializeField] [Range(1, 2)] private float maxRectScale = 1;

        [Header("Resources")]
        [SerializeField] private RectTransform targetRect;
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private Camera targetCamera;

        private Vector2 mousePos;

        void Awake()
        {
            if (targetRect == null) { targetRect = GetComponent<RectTransform>(); }
            if (targetCanvas == null) { targetCanvas = GetComponentInParent<Canvas>(); }
            if (targetCamera == null) { targetCamera = Camera.main; }

            targetRect.transform.localScale = new Vector3(maxRectScale, maxRectScale, maxRectScale);
        }

        void OnEnable()
        {
            targetRect.anchoredPosition = new Vector2(0, 0);
        }

        void Update()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetCanvas.transform as RectTransform, Mouse.current.position.ReadValue(), targetCamera, out mousePos);
            targetRect.anchoredPosition = Vector2.Lerp(targetRect.anchoredPosition, targetCanvas.transform.TransformPoint(mousePos) * multiplier, smoothness / (multiplier * 4f));
        }
    }
}