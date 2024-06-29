// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Editor {

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [UnityEditor.CustomEditor(typeof(Rewired.Platforms.Custom.HardwareJoystickMapCustomPlatformMapSO), true)]
    public sealed class HardwareJoystickMapCustomPlatformMapInspector : CustomInspector_External {

        private void OnEnable() {
            internalEditor = new Rewired.Editor.HardwareJoystickMapCustomPlatformMapSOInspector_Internal(this);
            base.Enabled();
        }
    }

}
