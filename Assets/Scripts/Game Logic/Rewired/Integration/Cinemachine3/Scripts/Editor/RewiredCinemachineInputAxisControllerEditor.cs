// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
// Portions of this code were derived from Unity.Cinemachine.CinemachineInputAxisController and are the property of Unity Technologies:
// https://github.com/Unity-Technologies/com.unity.cinemachine

#pragma warning disable 0649

namespace Rewired.Integration.Cinemachine3.Editor {
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;

    [CustomEditor(typeof(RewiredCinemachineInputAxisController), true)]
    class RewiredCinemachineInputAxisControllerEditor : UnityEditor.Editor {

        public override VisualElement CreateInspectorGUI() {
            VisualElement visualElement = new VisualElement();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true)) {
                AddRemainingProperties(visualElement, property);
            }
            return visualElement;
        }

        private static void AddRemainingProperties(VisualElement visualElement, SerializedProperty property) {
            if (property == null) return;
            SerializedProperty p = property.Copy();
            do {
                if (p.name != "m_Script")
                    visualElement.Add(new PropertyField(p));
            }
            while (p.NextVisible(false));
        }
    }

    [InitializeOnLoad]
    class RewiredCinemachineInputAxisControllerDefaultControlInitializer {
        static RewiredCinemachineInputAxisControllerDefaultControlInitializer() {
            RewiredCinemachineInputAxisController.SetControlDefaults
            = (in Unity.Cinemachine.IInputAxisOwner.AxisDescriptor axis, RewiredCinemachineInputAxisController.Controller controller) => {
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
