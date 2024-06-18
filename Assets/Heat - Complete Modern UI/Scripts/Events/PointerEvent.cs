using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.Heat
{
    public class PointerEvent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        public bool addEventTrigger = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onClick = new UnityEvent();
        [SerializeField] private UnityEvent onEnter = new UnityEvent();
        [SerializeField] private UnityEvent onExit = new UnityEvent();

        void Awake() { if (addEventTrigger == true) { gameObject.AddComponent<EventTrigger>(); } }
        public void OnPointerClick(PointerEventData eventData) { onClick.Invoke(); }
        public void OnPointerEnter(PointerEventData eventData) { onEnter.Invoke(); }
        public void OnPointerExit(PointerEventData eventData) { onExit.Invoke(); }
    }
}