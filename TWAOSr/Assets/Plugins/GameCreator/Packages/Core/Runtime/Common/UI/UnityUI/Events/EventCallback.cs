using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("")]
    public class EventCallback : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action<PointerEventData> EventInitializePotentialDrag;
        public event Action<PointerEventData> EventBeginDrag;
        public event Action<PointerEventData> EventEndDrag;
        public event Action<PointerEventData> EventDrag;
        public event Action<PointerEventData> EventDrop;

        public event Action<AxisEventData> EventMove;
        public event Action<PointerEventData> EventScroll;
        public event Action<BaseEventData> EventSelect;
        public event Action<BaseEventData> EventDeselect;
        public event Action<BaseEventData> EventSubmit;
        public event Action<BaseEventData> EventCancel;
        public event Action<BaseEventData> EventUpdateSelected;
        
        public event Action<PointerEventData> EventPointerClick;
        public event Action<PointerEventData> EventPointerDown;
        public event Action<PointerEventData> EventPointerUp;
        public event Action<PointerEventData> EventPointerEnter;
        public event Action<PointerEventData> EventPointerExit;

        // OVERRIDE EVENTS: -----------------------------------------------------------------------

        public void OnMove(AxisEventData data)
        {
            this.EventMove?.Invoke(data);
        }
        
        public void OnScroll(PointerEventData data)
        {
            this.EventScroll?.Invoke(data);
        }
        
        public void OnSelect(BaseEventData data)
        {
            this.EventSelect?.Invoke(data);
        }
        
        public void OnDeselect(BaseEventData data)
        {
            this.EventDeselect?.Invoke(data);
        }
        
        public void OnSubmit(BaseEventData data)
        {
            this.EventSubmit?.Invoke(data);
        }
        
        public void OnCancel(BaseEventData data)
        {
            this.EventCancel?.Invoke(data);
        }
        
        public void OnUpdateSelected(BaseEventData data)
        {
            this.EventUpdateSelected?.Invoke(data);
        }
        
        public void OnInitializePotentialDrag(PointerEventData data)
        {
            this.EventInitializePotentialDrag?.Invoke(data);
        }

        public void OnBeginDrag(PointerEventData data)
        {
            this.EventBeginDrag?.Invoke(data);
        }

        public void OnEndDrag(PointerEventData data)
        {
            this.EventEndDrag?.Invoke(data);
        }
        
        public void OnDrag(PointerEventData data)
        {
            this.EventDrag?.Invoke(data);
        }
        
        public void OnDrop(PointerEventData data)
        {
            this.EventDrop?.Invoke(data);
        }

        public void OnPointerClick(PointerEventData data)
        {
            this.EventPointerClick?.Invoke(data);
        }
        
        public void OnPointerDown(PointerEventData data)
        {
            this.EventPointerDown?.Invoke(data);
        }
        
        public void OnPointerUp(PointerEventData data)
        {
            this.EventPointerUp?.Invoke(data);
        }
        
        public void OnPointerEnter(PointerEventData data)
        {
            this.EventPointerEnter?.Invoke(data);
        }
        
        public void OnPointerExit(PointerEventData data)
        {
            this.EventPointerExit?.Invoke(data);
        }
        
        // REGISTER METHODS: ----------------------------------------------------------------------

        public static bool RegisterInitPotentialDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventInitializePotentialDrag += callback;
            return true;
        }
        
        public static bool RegisterBeginDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventBeginDrag += callback;
            return true;
        }
        
        public static bool RegisterEndDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventEndDrag += callback;
            return true;
        }
        
        public static bool RegisterDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventDrag += callback;
            return true;
        }
        
        public static bool RegisterDrop(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventDrop += callback;
            return true;
        }

        public static bool RegisterMove(GameObject target, Action<AxisEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventMove += callback;
            return true;
        }
        
        public static bool RegisterScroll(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventScroll += callback;
            return true;
        }
        
        public static bool RegisterSelect(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventSelect += callback;
            return true;
        }
        
        public static bool RegisterDeselect(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventDeselect += callback;
            return true;
        }
        
        public static bool RegisterSubmit(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventSubmit += callback;
            return true;
        }
        
        public static bool RegisterCancel(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventCancel += callback;
            return true;
        }
        
        public static bool RegisterUpdateSelected(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventUpdateSelected += callback;
            return true;
        }
        
        public static bool RegisterPointerClick(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventPointerClick += callback;
            return true;
        }
        
        public static bool RegisterPointerDown(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventPointerDown += callback;
            return true;
        }
        
        public static bool RegisterPointerUp(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventPointerUp += callback;
            return true;
        }
        
        public static bool RegisterPointerEnter(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventPointerEnter += callback;
            return true;
        }
        
        public static bool RegisterPointerExit(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback == null) return false;

            eventCallback.EventPointerExit += callback;
            return true;
        }
        
        // UNREGISTER: ----------------------------------------------------------------------------
        
        public static void ForgetInitPotentialDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventInitializePotentialDrag -= callback;
        }
        
        public static void ForgetBeginDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventBeginDrag -= callback;
        }
        
        public static void ForgetEndDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventEndDrag -= callback;
        }
        
        public static void ForgetDrag(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventDrag -= callback;
        }
        
        public static void ForgetDrop(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventDrop -= callback;
        }

        public static void ForgetMove(GameObject target, Action<AxisEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventMove -= callback;
        }
        
        public static void ForgetScroll(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventScroll -= callback;
        }
        
        public static void ForgetSelect(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventSelect -= callback;
        }
        
        public static void ForgetDeselect(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventDeselect -= callback;
        }
        
        public static void ForgetSubmit(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventSubmit -= callback;
        }
        
        public static void ForgetCancel(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventCancel -= callback;
        }
        
        public static void ForgetUpdateSelected(GameObject target, Action<BaseEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventUpdateSelected -= callback;
        }
        
        public static void ForgetPointerClick(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventPointerClick -= callback;
        }
        
        public static void ForgetPointerDown(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventPointerDown -= callback;
        }
        
        public static void ForgetPointerUp(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventPointerUp -= callback;
        }
        
        public static void ForgetPointerEnter(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventPointerEnter -= callback;
        }
        
        public static void ForgetPointerExit(GameObject target, Action<PointerEventData> callback)
        {
            EventCallback eventCallback = RequestEventTrigger(target);
            if (eventCallback != null) eventCallback.EventPointerExit -= callback;
        }
        
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------
        
        private static EventCallback RequestEventTrigger(GameObject target)
        {
            if (target == null) return null;
            
            EventCallback eventCallback = target.Get<EventCallback>();
            if (eventCallback != null) return eventCallback;

            eventCallback = target.Add<EventCallback>();
            eventCallback.hideFlags = HideFlags.HideInInspector;
            
            return eventCallback;
        }
    }
}