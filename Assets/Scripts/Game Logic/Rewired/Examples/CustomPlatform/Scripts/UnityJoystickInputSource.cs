// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// Using UnityEngine.Input as the input source for this example.
    /// It is safe to ignore most of this class. It is just a placeholder
    /// implementation using UnityEngine.Input just to illustrate some
    /// concepts for this example.
    /// This implementation has a lot of problems with controller connection
    /// and disconnection because of inherent flaws in UnityEngine.Input's design.
    /// Those flaws are not important for the purposes of this example.
    /// </summary>
    public class UnityInputJoystickSource {

        private const float joystickCheckInterval = 1f;
        private static int systemIdCounter;

        private string[] _unityJoysticks = new string[0];
        private double _nextJoystickCheckTime;

        private System.Collections.Generic.List<Joystick> _joysticks;
        private System.Collections.ObjectModel.ReadOnlyCollection<Joystick> _joysticks_readOnly;

        public UnityInputJoystickSource() {
            _joysticks = new System.Collections.Generic.List<Joystick>();
            _joysticks_readOnly = new System.Collections.ObjectModel.ReadOnlyCollection<Joystick>(_joysticks);
            RefreshJoysticks();
        }

        public void Update() {
            CheckForJoystickChanges();
        }

        public System.Collections.Generic.IList<Joystick> GetJoysticks() {
            return _joysticks_readOnly;
        }

        private void CheckForJoystickChanges() {
            double time = Rewired.ReInput.time.unscaledTime;
            if (time >= _nextJoystickCheckTime) {
                _nextJoystickCheckTime = time + joystickCheckInterval;
                if (DidJoysticksChange()) {
                    RefreshJoysticks();
                }
            }
        }

        private bool DidJoysticksChange() {
            string[] newJoysticks = UnityEngine.Input.GetJoystickNames();
            string[] prevJoysticks = _unityJoysticks;
            _unityJoysticks = newJoysticks;
            if (prevJoysticks.Length != newJoysticks.Length) return true;
            for (int i = 0; i < newJoysticks.Length; i++) {
                if (!string.Equals(prevJoysticks[i], newJoysticks[i], System.StringComparison.Ordinal)) return true;
            }
            return false;
        }

        private void RefreshJoysticks() {

            // Unity's legacy input system is a mess. Joystick index is not fixed on some platforms and will
            // change around as joysticks are added or removed. This behavior differs depending on the platform.
            // This example cannot handle all these problem scenarios. This is just a basic usage example.
            // UnityEngine.Input was used for the example because it is available on all platforms.
            // For purposes of this example, it is unsafe to use more than one controller at a time that returns
            // the same name in UnityEngine.Input.GetJoystickNames.
            // This example does not attempt to provide a complete system for dealing with the myriad
            // UnityEngine.Input problems on all platforms.

            bool[] previouslyConnected = new bool[_unityJoysticks.Length];

            // Find disconnections
            for (int i = _joysticks.Count - 1; i >= 0; i--) { // iterate in reverse so we can remove from list
                int unityIndex = _joysticks[i].unityIndex;
                if (unityIndex >= _unityJoysticks.Length || // index is now out of range
                    !string.Equals(_joysticks[i].deviceName, _unityJoysticks[unityIndex]) // name in this index changed
                ) {
                    // See if joystick moved to a different index.
                    // Assume that this joystick would be the last one found in the list.
                    // This will likely fail if multiple controllers are connected that report the same name
                    // if the platform reorders device indices when controllers are connected / disconnected.
                    bool found = false;
                    for (int j = _unityJoysticks.Length - 1; j >= 0; j--) { // find last match
                        if (previouslyConnected[j]) continue; // already accounted for this one, skip
                        if (string.Equals(_unityJoysticks[j], _joysticks[i].deviceName)) {
                            // Update the Unity index in the source joystick so input values can be
                            // obtained from the correct Unity joystick id.
                            // NOTE: This will also fail on some platforms.
                            // Update the Unity joystick index
                            // This will fail on Windows because the array index can mismatch the
                            // actual Unity joystick id used for getting input values.
#if !UNITY_EDITOR_WIN || (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
                            _joysticks[i].unityIndex = j;
#endif
                            previouslyConnected[j] = true; // mark this index as previously connected
                            found = true;
                            break;
                        }
                    }
                    if (found) continue;

                    UnityEngine.Debug.Log(_joysticks[i].deviceName + " was disconnected.");
                    _joysticks.RemoveAt(i);
                } else { // joystick was still connected and is at the same index
                    previouslyConnected[unityIndex] = true; // mark this index as previously connected
                }
            }

            // Find connections
            // Because of Unity joystick index problems, we have to assume new joysticks will appear at the end of the list.
            // This, however, is not always the case on all platforms, so there WILL be problems.
            for (int i = 0; i < _unityJoysticks.Length; i++) {
                if (previouslyConnected[i]) continue; // already accounted for this one, skip
                if (string.IsNullOrEmpty(_unityJoysticks[i])) continue; // invalid or empty

                // Create a new joystick

                Joystick joystick;

                // Special handling for Xbox controllers for this example just to have something to illustrate various things like
                // using vendor id, product id, and element counts. If this were a real implementation, these values would come from
                // the input API. Just add them here for this example so they can be used in platform map to controller matching.
                if (_unityJoysticks[i].ToLower().Contains("xbox one") || _unityJoysticks[i].ToLower().Contains("xbox bluetooth")) {

                    joystick = new Joystick(
                        systemIdCounter++, // system id -- just use a counter because UnityEngine.Input cannot provide any sort of reliable, fixed unique id
                        _unityJoysticks[i], // device name
                        7, // axis count -- Unity maps to axis index 6, so need at least 7 axes
                        16 // button count
                    );

                    // Create an identifier to pass user-defined information for controller matching
                    joystick.identifier = new MyPlatformControllerIdentifier() {
                        vendorId = 0x45e, // Microsoft
                        productId = 0x2D1 // Xbox One Controller
                    };

                    // Set the vibration motor count
                    joystick.vibrationMotorCount = 2;

                } else {
                    joystick = new Joystick(
                        systemIdCounter++, // system id -- just use a counter because UnityEngine.Input cannot provide any sort of reliable, fixed unique id
                        _unityJoysticks[i], // device name
                        10, // axis count
                        20 // button count
                    );
                }

                joystick.unityIndex = i;

                UnityEngine.Debug.Log(_unityJoysticks[i] + " was connected.");
                _joysticks.Add(joystick);
            }
        }

        /// <summary>
        /// A represenatation of low-level system joystick.
        /// This could provide an interface to a native input API, for example.
        /// </summary>
        public class Joystick : Rewired.Interfaces.IControllerVibrator {

            private const int maxJoysticks = 8;
            private const int maxAxes = 10;
            private const int maxButtons = 20;

            /// <summary>
            /// System id may or may not be a long depending on the input API in use.
            /// This could be a long pointer, for example, or unique id assigned by your code
            /// using whatever means the input API provides to uniquely identify controllers.
            /// For example, if the API uses a string path to identify a device, each unique string could be
            /// assigned a long id and stored for later lookup, then that id value could be stored here.
            /// </summary>
            public readonly long systemId;
            public readonly string deviceName;
            /// <summary>
            /// Persistent unique identifier.
            /// This is an optional value that is for uniquely identifying controllers persistently
            /// across application sessions and system reboots. This value may not be available on
            /// all platforms or input sources. If this value cannot be obtained or created in some
            /// way with the available information about the device through the input API, leave it blank.
            /// </summary>
            public System.Guid deviceInstanceGuid;
            public readonly int axisCount;
            public readonly int buttonCount;
            public MyPlatformControllerIdentifier identifier;
            public readonly bool[] buttonValues;
            public readonly float[] axisValues;
            // This is only for this example because of issues dealing with Unity's input system
            public int unityIndex;

            public Joystick(long systemId, string deviceName, int axisCount, int buttonCount) {
                this.systemId = systemId;
                this.deviceName = deviceName;
                this.axisCount = axisCount;
                this.buttonCount = buttonCount;
                axisValues = new float[axisCount];
                buttonValues = new bool[buttonCount];
            }

            public bool GetButtonValue(int index) {
                if (index >= maxButtons) return false;
                if (systemId >= maxJoysticks) return false;
                return UnityEngine.Input.GetKey((UnityEngine.KeyCode)(UnityEngine.KeyCode.Joystick1Button0 + ((int)unityIndex * 20) + index));
            }

            public float GetAxisValue(int index) {
                if (index >= maxAxes) return 0f;
                if (systemId >= maxJoysticks) return 0f;
                return UnityEngine.Input.GetAxis("Joy" + (unityIndex + 1) + "Axis" + (index + 1));
            }

            // IControllerVibrator implementation for Controller Extension

            public int vibrationMotorCount { get; set; }

            public void SetVibration(int motorIndex, float motorLevel) {
                // Just logging values here because UnityEngine.Input does not support vibration
                UnityEngine.Debug.Log("Vibrate " + deviceName + ": motorIndex: " + motorIndex + ", motorLevel: " + motorLevel);
            }

            public void SetVibration(int motorIndex, float motorLevel, float duration) {
                // Just logging values here because UnityEngine.Input does not support vibration
                UnityEngine.Debug.Log("Vibrate " + deviceName + ": motorIndex: " + motorIndex + ", motorLevel: " + motorLevel + ", duration: " + duration);
            }

            public void SetVibration(int motorIndex, float motorLevel, bool stopOtherMotors) {
                // Just logging values here because UnityEngine.Input does not support vibration
                UnityEngine.Debug.Log("Vibrate " + deviceName + ": motorIndex: " + motorIndex + ", motorLevel: " + motorLevel + ", stopOtherMotors: " + stopOtherMotors);
            }

            public void SetVibration(int motorIndex, float motorLevel, float duration, bool stopOtherMotors) {
                // Just logging values here because UnityEngine.Input does not support vibration
                UnityEngine.Debug.Log("Vibrate " + deviceName + ": motorIndex: " + motorIndex + ", motorLevel: " + motorLevel + ", duration: " + duration + ", stopOtherMotors: " + stopOtherMotors);
            }

            public float GetVibration(int motorIndex) {
                // Just return 0 here because UnityEngine.Input does not support vibration
                return 0f;
            }

            public void StopVibration() {
                // Just logging values here because UnityEngine.Input does not support vibration
                UnityEngine.Debug.Log("Stop vibration " + deviceName);
            }
        }
    }
}
