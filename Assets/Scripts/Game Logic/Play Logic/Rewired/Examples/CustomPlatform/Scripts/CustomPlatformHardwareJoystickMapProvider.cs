// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// An example implementation of a hardware joystick map platform map provider.
    /// Finds the matching platform map for the controller in question for the current custom platform.
    /// </summary>
    [System.Serializable]
    public class CustomPlatformHardwareJoystickMapProvider : Rewired.Platforms.Custom.IHardwareJoystickMapCustomPlatformMapProvider {

        /// <summary>
        /// The list of Platform joystick data sets.
        /// </summary>
        public System.Collections.Generic.List<PlatformDataSet> platformJoystickDataSets;

        public Rewired.Data.Mapping.HardwareJoystickMap.Platform GetPlatformMap(int customPlatformId, System.Guid hardwareTypeGuid) {
            var platformDataSet = GetPlatformDataSet(customPlatformId);
            if (platformDataSet == null) return null;
            return GetPlatformMap(platformDataSet, hardwareTypeGuid);
        }

        private CustomPlatformHardwareJoystickMapPlatformDataSet GetPlatformDataSet(int customPlatformId) {
            int count = platformJoystickDataSets.Count;
            for (int i = 0; i < count; i++) {
                if (platformJoystickDataSets[i] != null && (int)platformJoystickDataSets[i].platformType == customPlatformId) {
                    return platformJoystickDataSets[i].dataSet;
                }
            }
            return null;
        }

        static private Rewired.Data.Mapping.HardwareJoystickMap.Platform GetPlatformMap(CustomPlatformHardwareJoystickMapPlatformDataSet platformDataSet, System.Guid hardwareTypeGuid) {
            if (platformDataSet == null || platformDataSet.platformMaps == null) return null;
            int count = platformDataSet.platformMaps.Count;
            for (int i = 0; i < count; i++) {
                if (platformDataSet.platformMaps[i] != null && platformDataSet.platformMaps[i].Matches(hardwareTypeGuid)) {
                    return platformDataSet.platformMaps[i].GetPlatformMap();
                }
            }
            return null;
        }

        [System.Serializable]
        public class PlatformDataSet {
            public CustomPlatformType platformType;
            public CustomPlatformHardwareJoystickMapPlatformDataSet dataSet;
        }
    }
}
