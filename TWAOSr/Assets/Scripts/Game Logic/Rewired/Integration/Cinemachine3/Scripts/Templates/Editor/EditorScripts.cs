// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

// This is a template to use to allow you to use exported Rewired constants so you can configure Players and Actions in the
// inspector of the Cinemachine input axis using drop-down menus instead of tying strings.
// Copy this script to a location outside of the Rewired folder and place it into a folder named "Editor", then uncomment
// the script and modify the 1 item below to use it.

/*

namespace YOURNAMESPACE { // 1. Change this to a namespace for your project.

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // DO NOT EDIT ANYTHING BELOW THIS LINE UNLESS YOU WANT TO RENAME THE CLASS ////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    using Rewired.Integration.Cinemachine3.Editor;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(CustomRewiredCinemachineInputAxisController.Controller), true)]
    class CustomRewiredCinemachineInputAxisControllerReaderPropertyDrawer : RewiredCinemachineInputAxisControllerReaderPropertyDrawer {
    }

    [CustomEditor(typeof(CustomRewiredCinemachineInputAxisController))]
    class CustomRewiredCinemachineInputAxisControllerEditor : RewiredCinemachineInputAxisControllerEditor {
    }

    [InitializeOnLoad]
    class CustomRewiredCinemachineInputAxisControllerDefaultControlInitializer {
        static CustomRewiredCinemachineInputAxisControllerDefaultControlInitializer() {
            CustomRewiredCinemachineInputAxisController.SetControlDefaults
            = (in Unity.Cinemachine.IInputAxisOwner.AxisDescriptor axis, CustomRewiredCinemachineInputAxisController.Controller controller) => {
                EditorUtils.SetInputAxisControllerDefaults(
                    new EditorUtils.SetInputAxisControllerDefaultsValues() {
                        axis = axis,
                        setAbsoluteAxisGain = x => controller.Input.absoluteAxisGain = x,
                        setRelativeAxisGain = x => controller.Input.relativeAxisGain = x,
                        setEnabled = x => controller.Enabled = x,
                        setDriver = x => controller.Driver = x
                    }
                );
            };
        }
    }
}

*/
