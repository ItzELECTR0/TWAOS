using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    [Icon(RuntimePaths.GIZMOS + "GizmoTouchstick.png")]
    
    public abstract class TTouchStick : MonoBehaviour, ITouchStick
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public virtual Vector2 Value { get; protected set; }
        public virtual GameObject Root { get; protected internal set; }

        protected internal virtual RectTransform Surface { get; set; }
        protected internal virtual RectTransform Stick { get; set; }
        
        // INITIALIZERS: --------------------------------------------------------------------------

        protected void Start()
        {
            EventSystemManager.RequestEventSystem();
        }

        protected virtual void OnEnable()
        {
            this.Value = Vector2.zero;
            if (this.Stick != null) this.Stick.anchoredPosition = Vector2.zero;
        }

        protected virtual void OnDisable()
        {
            this.Value = Vector2.zero;
            if (this.Stick != null) this.Stick.anchoredPosition = Vector2.zero;
        }

        // POINTER METHODS: -----------------------------------------------------------------------
        
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (this.Stick == null) return;
            if (this.Surface == null) return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.Surface,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 position
            );

            Vector2 surfaceSize = this.Surface.sizeDelta;
            Vector2 stickSize = this.Stick.sizeDelta;
            
            position.x /= surfaceSize.x;
            position.y /= surfaceSize.y;

            float x = Mathf.Lerp(position.x * 2 + 1, position.x * 2 - 1, this.Surface.pivot.x);
            float y = Mathf.Lerp(position.y * 2 + 1, position.y * 2 - 1, this.Surface.pivot.y);

            this.Value = Vector2.ClampMagnitude(new Vector2(x, y), 1f);
            this.Stick.anchoredPosition = new Vector2(
                this.Value.x * (surfaceSize.x / 2 - stickSize.x / 2f),
                this.Value.y * (surfaceSize.y / 2 - stickSize.y / 2f)
            );
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            this.Value = Vector2.zero;
            if (this.Stick != null) this.Stick.anchoredPosition = Vector2.zero;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.OnDrag(eventData);
        }
    }
}