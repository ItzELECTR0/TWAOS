// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// A controller extension for this custom platform.
    /// This allows supporting special features such as vibration and other custom functionality.
    /// Implementing Rewired.Interfaces.IControllerVibrator allows Rewired's Controller and Player vibration function calls to work.
    /// </summary>
    public sealed class MyPlatformControllerExtension : ControllerExtensions.CustomControllerExtension, Rewired.Interfaces.IControllerVibrator {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceJoystick">Source joystick.</param>
        public MyPlatformControllerExtension(MyPlatformInputSource.Joystick sourceJoystick) : base(new Source(sourceJoystick)) {
        }
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Object to copy</param>
        private MyPlatformControllerExtension(MyPlatformControllerExtension other) : base(other) {
        }

        /// <summary>
        /// Makes a shallow copy of the object.
        /// </summary>
        /// <returns>Shallow copy of the object.</returns>
        public override Controller.Extension ShallowCopy() {
            return new MyPlatformControllerExtension(this);
        }

        // Implement IControllerVibrator interface so Rewired vibration works

        public int vibrationMotorCount {
            get {
                return 2;
            }
        }

        public void SetVibration(int motorIndex, float motorLevel) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel);
        }

        public void SetVibration(int motorIndex, float motorLevel, float duration) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel, duration);
        }

        public void SetVibration(int motorIndex, float motorLevel, bool stopOtherMotors) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel, stopOtherMotors);
        }

        public void SetVibration(int motorIndex, float motorLevel, float duration, bool stopOtherMotors) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel, duration, stopOtherMotors);
        }

        public float GetVibration(int motorIndex) {
            return ((Source)GetSource()).sourceJoystick.GetVibration(motorIndex);
        }

        public void StopVibration() {
            ((Source)GetSource()).sourceJoystick.StopVibration();
        }

        class Source : Rewired.Interfaces.IControllerExtensionSource {

            public readonly MyPlatformInputSource.Joystick sourceJoystick;

            public Source(MyPlatformInputSource.Joystick sourceJoystick) {
                this.sourceJoystick = sourceJoystick;
            }
        }
    }
}
