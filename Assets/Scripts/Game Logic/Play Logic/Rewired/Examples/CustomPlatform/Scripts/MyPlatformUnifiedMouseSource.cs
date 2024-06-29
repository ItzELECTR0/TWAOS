// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// An example custom mouse input source that wraps UnityEngine.Input.
    /// </summary>
    public class MyPlatformUnifiedMouseSource : Rewired.Platforms.Custom.CustomPlatformUnifiedMouseSource {

        /// <summary>
        /// Mouse screen position in pixels.
        /// </summary>
        public override UnityEngine.Vector2 mousePosition {
            get {
                return UnityEngine.Input.mousePosition;
            }
        }

        /// <summary>
        /// Called once per enabled update loop frame.
        /// </summary>
        protected override void Update() {

            // Update input values

            // Set axis values
            // Mouse axis count is fixed. Can be obtained from this.axisCount.
            SetAxisValue(0, UnityEngine.Input.GetAxis("MouseAxis1"));
            SetAxisValue(1, UnityEngine.Input.GetAxis("MouseAxis2"));
            SetAxisValue(2, UnityEngine.Input.GetAxis("MouseAxis3"));

            // Set button values
            // Mouse button count is fixed. Can be obtained from this.buttonCount.
            SetButtonValue(0, UnityEngine.Input.GetButton("MouseButton0"));
            SetButtonValue(1, UnityEngine.Input.GetButton("MouseButton1"));
            SetButtonValue(2, UnityEngine.Input.GetButton("MouseButton2"));
            SetButtonValue(3, UnityEngine.Input.GetButton("MouseButton3"));
            SetButtonValue(4, UnityEngine.Input.GetButton("MouseButton4"));
            SetButtonValue(5, UnityEngine.Input.GetButton("MouseButton5"));
            SetButtonValue(6, UnityEngine.Input.GetButton("MouseButton6"));
        }
    }
}
