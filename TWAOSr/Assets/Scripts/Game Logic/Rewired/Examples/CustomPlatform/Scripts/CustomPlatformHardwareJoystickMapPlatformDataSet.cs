// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// Contains a list of controller definition platform map for a particular custom platform.
    /// </summary>
    [System.Serializable]
    public class CustomPlatformHardwareJoystickMapPlatformDataSet : UnityEngine.ScriptableObject {

        public System.Collections.Generic.List<Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMapSO> platformMaps;
    }
}
