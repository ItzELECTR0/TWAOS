// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// Example custom platform manager.
    /// Add this component to the Rewired Input Manager GameObject or a child GameObject and Rewired
    /// will set the custom platform during initialization.
    /// The first active and enabled component that implements <see cref="Rewired.Platforms.Custom.ICustomPlatformInitializer"/>
    /// found that returns a non-null <see cref="Rewired.Platforms.Custom.CustomPlatformInitOptions"/> will be used by
    /// Rewired during initialization to set the custom platform.
    /// </summary>
    public sealed class CustomPlatformManager : UnityEngine.MonoBehaviour, Rewired.Platforms.Custom.ICustomPlatformInitializer {

        /// <summary>
        /// Provides custom platform joystick definition maps.
        /// </summary>
        public CustomPlatformHardwareJoystickMapProvider mapProvider;

        /// <summary>
        /// Gets the custom platform options to initialize a custom platform.
        /// Called during Rewired initialization.
        /// Return null to not use a custom platform.
        /// </summary>
        /// <returns>Custom platform init options</returns>
        public Rewired.Platforms.Custom.CustomPlatformInitOptions GetCustomPlatformInitOptions() {

            // You can use #if conditionals or other means to determine which custom platform to initialize, if any.

            // Create platform options
            var options = new Rewired.Platforms.Custom.CustomPlatformInitOptions();

            // Set the platform id
            options.platformId = (int)CustomPlatformType.MyPlatform;

            // Set the platform identifier string
            options.platformIdentifierString = "MyPlatform";

            // Set the map provider
            options.hardwareJoystickMapCustomPlatformMapProvider = mapProvider;

            // Create platform configuration values
            var configVars = new Rewired.Platforms.Custom.CustomPlatformConfigVars() {
                ignoreInputWhenAppNotInFocus = true,
                useNativeKeyboard = true,
                useNativeMouse = true
            };

            // Create and set the input source
            options.inputSource = new MyPlatformInputSource(configVars);
            
            return options;
        }
    }

}
