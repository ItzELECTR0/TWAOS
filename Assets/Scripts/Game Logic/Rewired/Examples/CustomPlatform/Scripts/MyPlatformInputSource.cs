// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// An example custom input source for a custom platform.
    /// This is for managing Joysticks.
    /// Must implement <see cref="Rewired.Platforms.Custom.CustomPlatformInputSource"/>.
    /// This is an extremely simple implementation and only one of many possible approaches.
    /// Real-world design depends on how the platform input API works as each work very differently.
    /// (Polling vs event-based, separate input thread, etc.)
    /// No function in this class should be called from a separate thread.
    /// If multi-threaded input is required, input should be read on a separate thread, enqueued,
    /// and consumed by the Joystick/Controller on Update in a thread-safe manner (mutex).
    /// </summary>
    public sealed class MyPlatformInputSource : Rewired.Platforms.Custom.CustomPlatformInputSource {

        /// <summary>
        /// Source of joysticks. This is just for this example.
        /// This would be replaced by a system that interfaces with the platform input API.
        /// </summary>
        private UnityInputJoystickSource _joystickInputSource = new UnityInputJoystickSource();

        /// <summary>
        /// Prevents anything from being called until input source is initialized.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Determines if the input source is ready.
        /// Input source must be ready before it can be used.
        /// </summary>
        public override bool isReady {
            get {
                return _initialized;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configVars">Custom platform configuration variables</param>
        public MyPlatformInputSource(Rewired.Platforms.Custom.CustomPlatformConfigVars configVars) :
            base(
                configVars,
                new InitOptions() {
                    // Optionally create a custom keyboard source. If this is ommitted or set to null, UnityEngine.Input will be used for keyboard handling.
                    // configVars.useNativeKeyboard must also be set to true or this keyboard source will not be used.
                    unifiedKeyboardSource = new MyPlatformUnifiedKeyboardSource(),
                    // Optionally create a custom mouse source. If this is ommitted or set to null, UnityEngine.Input will be used for mouse handling.
                    // configVars.useNativeMouse must also be set to true or this mouse source will not be used.
                    unifiedMouseSource = new MyPlatformUnifiedMouseSource()
                }
            )
        {
        }

        /// <summary>
        /// This is called once automatically during Rewired initialization.
        /// </summary>
        protected override void OnInitialize() {
            base.OnInitialize();

            // Do any required platform initialization...

            _initialized = true;

            // Get joysticks on init so they are ready in Awake for scripts
            MonitorDeviceChanges();
        }

        /// <summary>
        /// Called once per enabled update loop frame.
        /// </summary>
        public override void Update() {

            _joystickInputSource.Update();

            // Handle device connection and disconnection
            MonitorDeviceChanges();
        }

        /// <summary>
        /// Check to see if any devices have been connected or disconnected.
        /// </summary>
        private void MonitorDeviceChanges() {

            // Find joysticks on the system and add/remove them as necessary

            // Get the current joystick list
            var joysticks = GetJoysticks();

            // Get the current system joystick list
            var systemJoysticks = _joystickInputSource.GetJoysticks();

            // Handle joystick disconnections first
            // For each joystick already connected
            //     if system joystick list does not contain joystick, remove it

            for (int i = joysticks.Count - 1; i >= 0; i--) { // iterate list backwards so items can be removed
                var joystick = joysticks[i] as Joystick; // cast to local override Joystick type
                if (ContainsSystemJoystickBySystemId(systemJoysticks, joystick.sourceJoystick.systemId)) continue;

                // Found an orphaned joystick. Remove it.
                RemoveJoystick(joystick); // remove it from the joystick list
            }

            // Handle new joystick connections
            // For each joystick detected on system:
            //    if joysticks does not contain system joystick, add it
            //        if maintaining a joystick history, retrieve joystick from history, otherwise create new Joystick and add it

            for (int i = 0; i < systemJoysticks.Count; i++) {
                var systemJoystick = systemJoysticks[i];
                if (ContainsJoystickBySystemId(joysticks, systemJoystick.systemId)) continue;

                // Found a new joystick
                Joystick newJoystick = new Joystick(systemJoystick); // create the joystick

                // Add a custom controller extension if you have extra features you want to support through a Controller Extension
                if (systemJoystick.vibrationMotorCount > 0) {
                    newJoystick.extension = new MyPlatformControllerExtension(newJoystick);
                }

                AddJoystick(newJoystick); // add it to the joystick list
            }
        }

        #region IDisposable Implementation

        private bool _disposed;

        /* Uncomment if freeing native resources
        ~TestInputSource() {
            Dispose(false);
        }*/

        protected override void Dispose(bool disposing) {
            if (_disposed) return;

            if (disposing) {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
        /// Find the index of a joystick by system id.
        /// </summary>
        /// <param name="joysticks">The joysticks list.</param>
        /// <param name="systemId">The system id of the joystick.</param>
        /// <returns>True if found, false if not.</returns>
        private static bool ContainsJoystickBySystemId(System.Collections.Generic.IList<Rewired.Platforms.Custom.CustomInputSource.Joystick> joysticks, long systemId) {
            for (int i = 0; i < joysticks.Count; i++) {
                if (joysticks[i].systemId == systemId) return true;
            }
            return false;
        }

        /// <summary>
        /// Find the index of a system joystick by system id.
        /// </summary>
        /// <param name="systemJoysticks">The joysticks list.</param>
        /// <param name="systemId">The system id of the joystick.</param>
        /// <returns>True if found, false if not.</returns>
        private static bool ContainsSystemJoystickBySystemId(System.Collections.Generic.IList<UnityInputJoystickSource.Joystick> systemJoysticks, long systemId) {
            for (int i = 0; i < systemJoysticks.Count; i++) {
                if (systemJoysticks[i].systemId == systemId) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Example Joystick implementation that supports vibration.
        /// </summary>
        new public sealed class Joystick : Rewired.Platforms.Custom.CustomPlatformInputSource.Joystick, Rewired.Interfaces.IControllerVibrator {

            private UnityInputJoystickSource.Joystick _sourceJoystick;

            public UnityInputJoystickSource.Joystick sourceJoystick { get { return _sourceJoystick; } }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="sourceJoystick">The source joystick.</param>
            /// <exception cref="System.ArgumentNullException">sourceJoystick is null.</exception>
            public Joystick(UnityInputJoystickSource.Joystick sourceJoystick)
                : base(sourceJoystick.deviceName, sourceJoystick.systemId, sourceJoystick.axisCount, sourceJoystick.buttonCount)
            {
                if (sourceJoystick == null) throw new System.ArgumentNullException("sourceJoystick");
                _sourceJoystick = sourceJoystick;
                customIdentifier = _sourceJoystick.identifier; // add additional identifying information for definition matching
                deviceInstanceGuid = sourceJoystick.deviceInstanceGuid; // set persistent device identifier, if available. do not set this if a persistent unique identifier cannot be created or obtained on the current platform.
            }

            // Public Methods

            /// <summary>
            /// Called once per frame. Used to update input values.
            /// </summary>
            public override void Update() {

                // Fill button values
                for (int i = 0; i < buttonCount; i++) {
                    SetButtonValue(i, sourceJoystick.GetButtonValue(i));
                    //SetButtonFloatValue(i, sourceJoystick.GetButtonFloatValue(i)); // use if pressure-sensitive buttons
                }

                // Fill axis values
                for (int i = 0; i < axisCount; i++) {
                    SetAxisValue(i, sourceJoystick.GetAxisValue(i));
                }
            }

            // Implement IControllerVibrator interface so Rewired vibration works

            public int vibrationMotorCount {
                get {
                    return _sourceJoystick.vibrationMotorCount;
                }
            }

            public void SetVibration(int motorIndex, float motorLevel) {
                _sourceJoystick.SetVibration(motorIndex, motorLevel);
            }

            public void SetVibration(int motorIndex, float motorLevel, float duration) {
                _sourceJoystick.SetVibration(motorIndex, motorLevel, duration);
            }

            public void SetVibration(int motorIndex, float motorLevel, bool stopOtherMotors) {
                _sourceJoystick.SetVibration(motorIndex, motorLevel, stopOtherMotors);
            }

            public void SetVibration(int motorIndex, float motorLevel, float duration, bool stopOtherMotors) {
                _sourceJoystick.SetVibration(motorIndex, motorLevel, duration, stopOtherMotors);
            }

            public float GetVibration(int motorIndex) {
                return _sourceJoystick.GetVibration(motorIndex);
            }

            public void StopVibration() {
                _sourceJoystick.StopVibration();
            }
        }
    }
}
