// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
// Portions of this code were derived from Unity.Cinemachine.CinemachineInputAxisController and are the property of Unity Technologies:
// https://github.com/Unity-Technologies/com.unity.cinemachine

namespace Rewired.Integration.Cinemachine3.Editor {
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(RewiredCinemachineInputAxisControllerBase<RewiredCinemachineInputAxisControllerReader>.Controller), true)]
    class RewiredCinemachineInputAxisControllerReaderPropertyDrawer : PropertyDrawer {

        private InspectorUtility __inspectorUtility;
        private InspectorUtility inspectorUtility {
            get {
                return __inspectorUtility != null ? __inspectorUtility : (__inspectorUtility = new InspectorUtility());
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            if (!inspectorUtility.isValid) {
                return base.CreatePropertyGUI(property);
            }

            var overlay = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };
            overlay.Add(new PropertyField(property.FindPropertyRelative("Enabled"), "") { style = { flexGrow = 0, flexBasis = EditorGUIUtility.singleLineHeight, alignSelf = Align.Center } });

            // Draw the input value on the same line as the foldout, for convenience
            var inputProperty = property.FindPropertyRelative("Input");

            {
                SerializedProperty sp = inputProperty.FindPropertyRelative("_actionName");
                if (sp == null) sp = inputProperty.FindPropertyRelative("_actionId"); // handle custom version with Consts
                if (sp != null) {
                    overlay.Add(new PropertyField(sp, "") { style = { flexGrow = 1, flexBasis = 5 * EditorGUIUtility.singleLineHeight } });
                }
            }

            var foldout = new Foldout() { text = property.displayName, tooltip = property.tooltip };
            foldout.BindProperty(property);

            var childProperty = property.Copy();
            var endProperty = childProperty.GetEndProperty();
            childProperty.NextVisible(true);

            while (!SerializedProperty.EqualContents(childProperty, endProperty)) {
                foldout.Add(new PropertyField(childProperty));
                childProperty.NextVisible(false);
            }

            return inspectorUtility.CreateFoldoutWithOverlay(foldout, overlay, null, 12, 3);
        }
    }
}
