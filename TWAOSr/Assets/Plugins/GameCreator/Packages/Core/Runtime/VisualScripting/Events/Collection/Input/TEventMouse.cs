using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Button", "The mouse button to detect")]
    [Parameter(
        "Min Distance", 
        "If set to None, the mouse input acts globally. If set to Game Object, the event " +
        "only fires if the target object is within a certain radius"
    )]
    
    [Keywords("Left", "Middle", "Right")]
    
    [Serializable]
    public abstract class TEventMouse : Event
    {
        private static readonly List<RaycastResult> HITS = new List<RaycastResult>();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected MouseButton m_Button = MouseButton.Left;
        
        [SerializeField]
        private CompareMinDistanceOrNone m_MinDistance = new CompareMinDistanceOrNone();
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            
            if (!this.InteractionSuccessful(trigger)) return;
            if (IsPointerOverUI()) return;
            if (!this.m_MinDistance.Match(trigger.transform, new Args(this.Self))) return;
            
            _ = this.m_Trigger.Execute(this.Self);
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract bool InteractionSuccessful(Trigger trigger);
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected bool WasPressedThisFrame
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && this.GetButton().wasPressedThisFrame;
            }
        }
        
        protected bool WasReleasedThisFrame
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && this.GetButton().wasReleasedThisFrame;
            }
        }
        
        protected bool IsPressed
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && this.GetButton().IsPressed();
            }
        }

        protected int PressCount => Mouse.current != null 
            ? Mouse.current.clickCount.ReadValue() 
            : 0;

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private ButtonControl GetButton()
        {
            return this.m_Button switch
            {
                MouseButton.Left => Mouse.current.leftButton,
                MouseButton.Right => Mouse.current.rightButton,
                MouseButton.Middle => Mouse.current.middleButton,
                MouseButton.Forward => Mouse.current.forwardButton,
                MouseButton.Back => Mouse.current.backButton,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };
            
            EventSystem.current.RaycastAll(eventDataCurrentPosition, HITS);
            HITS.Sort(CompareHitDistance);

            return HITS.Count != 0 && HITS[0].gameObject.layer == UIUtils.LAYER_UI;
        }

        private static int CompareHitDistance(RaycastResult x, RaycastResult y)
        {
            return x.distance.CompareTo(y.distance);
        }

        // GIZMOS: --------------------------------------------------------------------------------

        protected internal override void OnDrawGizmosSelected(Trigger trigger)
        {
            base.OnDrawGizmosSelected(trigger);
            this.m_MinDistance.OnDrawGizmos(
                trigger.transform,
                new Args(trigger.gameObject)
            );
        }
    }
}