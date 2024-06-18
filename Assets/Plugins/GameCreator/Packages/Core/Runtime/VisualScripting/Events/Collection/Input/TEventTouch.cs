using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter(
        "Min Distance", 
        "If set to None, the touch input acts globally. If set to Game Object, the event " +
        "only fires if the target object is within a certain radius"
    )]
    
    [Keywords("Finger", "Press", "Click")]
    
    [Serializable]
    public abstract class TEventTouch : Event
    {
        private static readonly List<RaycastResult> HITS = new List<RaycastResult>();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [UnityEngine.SerializeField]
        private CompareMinDistanceOrNone m_MinDistance = new CompareMinDistanceOrNone();
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            InputManager.Instance.RequireEnhancedTouchInput();
        }

        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            InputManager.Instance.RequireEnhancedTouchInput();
            
            if (!this.InteractionSuccessful(trigger)) return;
            if (IsPointerOverUI()) return;
            if (!this.m_MinDistance.Match(trigger.transform, new Args(this.Self))) return;
            
            _ = this.m_Trigger.Execute(this.Self);
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract bool InteractionSuccessful(Trigger trigger);
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected bool WasTouchedThisFrame
        {
            get
            {
                ReadOnlyArray<Touch> touches = Touch.activeTouches;
                foreach (Touch touch in touches)
                {
                    if (!touch.began) continue;
                    return true;
                }

                return false;
            }
        }
        
        protected bool WasReleasedThisFrame
        {
            get
            {
                ReadOnlyArray<Touch> touches = Touch.activeTouches;
                foreach (Touch touch in touches)
                {
                    if (!touch.ended) continue;
                    return true;
                }

                return false;
            }
        }
        
        protected bool IsPressed
        {
            get
            {
                ReadOnlyArray<Touch> touches = Touch.activeTouches;
                foreach (Touch touch in touches)
                {
                    if (!touch.inProgress) continue;
                    return true;
                }

                return false;
            }
        }
        
        protected int TapCount
        {
            get
            {
                ReadOnlyArray<Touch> touches = Touch.activeTouches;
                int maxTaps = touches.Count > 0 ? 1 : 0;
                
                foreach (Touch touch in touches)
                {
                    if (maxTaps >= touch.tapCount) continue;
                    maxTaps = touch.tapCount;
                }

                return maxTaps;
            }
        }

        protected UnityEngine.Vector2 Position
        {
            get
            {
                ReadOnlyArray<Touch> touches = Touch.activeTouches;
                return touches.Count > 0
                    ? touches[^1].screenPosition
                    : UnityEngine.Vector2.one * -1f;
            }
        }

        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            
            ReadOnlyArray<Touch> touches = Touch.activeTouches;
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            
            foreach (Touch touch in touches)
            {
                eventDataCurrentPosition.position = touch.screenPosition;

                EventSystem.current.RaycastAll(eventDataCurrentPosition, HITS);
                HITS.Sort(CompareHitDistance);

                if (HITS.Count != 0 && HITS[0].gameObject.layer == UIUtils.LAYER_UI) return true;
            }

            return false;
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