using System;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;

namespace GameCreator.Runtime.Common
{
    [Keywords("Finger", "Touch", "Press", "Tap")]
    
    [Serializable]
    public abstract class TInputButtonTouch : TInputButton
    {
        // PROPERTIES: ----------------------------------------------------------------------------

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
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnStartup()
        {
            base.OnStartup();
            InputManager.Instance.RequireEnhancedTouchInput();
        }
    }
}