// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
// Portions of this code were derived from Unity.Cinemachine.CinemachineInputAxisController and are the property of Unity Technologies:
// https://github.com/Unity-Technologies/com.unity.cinemachine

namespace Rewired.Integration.Cinemachine3.Editor {
    using System;

    public static class EditorUtils {

        public struct SetInputAxisControllerDefaultsValues {
            public Unity.Cinemachine.IInputAxisOwner.AxisDescriptor axis;
            public Action<float> setAbsoluteAxisGain;
            public Action<float> setRelativeAxisGain;
            public Action<bool> setEnabled;
            public Action<Unity.Cinemachine.DefaultInputAxisDriver> setDriver;
        }

        public static void SetInputAxisControllerDefaults(SetInputAxisControllerDefaultsValues values) {
            bool isMomentary = (values.axis.DrivenAxis().Restrictions & Unity.Cinemachine.InputAxis.RestrictionFlags.Momentary) != 0;
            var invertY = false;

            if (values.axis.Name.Contains("Look")) {
                invertY = values.axis.Hint == Unity.Cinemachine.IInputAxisOwner.AxisDescriptor.Hints.Y;
                values.setDriver(Unity.Cinemachine.DefaultInputAxisDriver.Default);
            }

            const float defaultAbsoluteGain = 200f; // to match Cinemachine defaults
            const float defaultRelativeGain = 1f;

            values.setAbsoluteAxisGain(isMomentary ? 1 : defaultAbsoluteGain * (invertY ? -1 : 1));
            values.setRelativeAxisGain(defaultRelativeGain * (invertY ? -1 : 1));
            values.setEnabled(true);
        }
    }
}
