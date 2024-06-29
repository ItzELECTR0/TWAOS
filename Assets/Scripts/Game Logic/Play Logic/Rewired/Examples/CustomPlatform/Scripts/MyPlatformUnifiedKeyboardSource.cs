// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// An example custom keyboard input source that wraps UnityEngine.Input.
    /// </summary>
    public class MyPlatformUnifiedKeyboardSource : Rewired.Platforms.Custom.CustomPlatformUnifiedKeyboardSource {

        private static readonly Rewired.KeyboardKeyCode[] keyCodes = (Rewired.KeyboardKeyCode[])System.Enum.GetValues(typeof(Rewired.KeyboardKeyCode));

        protected override void OnInitialize() {
            base.OnInitialize();

            // Customize some key labels.
            // This could be used to support keyboard layouts and localization.
            // These key labels will be shown wherever the key name is returned
            // such as ActionElementMap.elementIdentifierName.

            // Create a new key property map
            // A new KeyPropertyMap will default to US keyboard layout key labels
            KeyPropertyMap map = new KeyPropertyMap();
            
            // Set a single key label
            map.Set(
                new KeyPropertyMap.Key() {
                    keyCode = Rewired.KeyboardKeyCode.A,
                    label = "[A]"
                }
            );

            // Set multiple of key labels at the same time
            map.Set(
                new [] {
                    new KeyPropertyMap.Key() { keyCode = Rewired.KeyboardKeyCode.B, label = "[B]" },
                    new KeyPropertyMap.Key() { keyCode = Rewired.KeyboardKeyCode.C, label = "[C]" },
                    new KeyPropertyMap.Key() { keyCode = Rewired.KeyboardKeyCode.D, label = "[D]" }
                }
            );

            // Assign the new map
            keyPropertyMap = map;
        }

        /// <summary>
        /// Called once per enabled update loop frame.
        /// </summary>
        protected override void Update() {

            // Update input values

            // Set by values key code
            for (int i = 0; i < keyCodes.Length; i++) {
                SetKeyValue(keyCodes[i], UnityEngine.Input.GetKey((UnityEngine.KeyCode)keyCodes[i]));
            }

            // Values can also be set by key index.Keyboard element identifier ids can be found here:
            // https://guavaman.com/rewired/files/docs/RewiredKeyboardElementIdentifiersCSV.zip
            // To get button index from element identfier id: Rewired.Keyboard.GetButtonIndexById
            // To get button index from key code: See Rewired.Keyboard.GetButtonIndexByKeyCode

            // Keyboard key count is fixed. Can be obtained from this.buttonCount.
        }
    }
}
