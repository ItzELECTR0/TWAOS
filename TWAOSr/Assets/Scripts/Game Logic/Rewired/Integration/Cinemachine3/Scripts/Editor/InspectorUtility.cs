// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Integration.Cinemachine3.Editor {
    using System;
    using System.Reflection;
    using System.Globalization;
    using UnityEngine.UIElements;

    public class InspectorUtility {

        // InspectorUtility is required for this property drawer, but it is internal.
        // Ugly, brittle old reflection to the rescue once again...

        private const string reflectionErrorMessagePrefix = "Unity may have made breaking changes to Cinemachine. Please report this to Rewired support.";

        private bool _reflectionError;
        private Type __inspectorUtility_FoldoutWithOverlay;
        private Type inspectorUtility_FoldoutWithOverlay {
            get {
                if (_reflectionError) return null;
                if (__inspectorUtility_FoldoutWithOverlay != null) return __inspectorUtility_FoldoutWithOverlay;

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Type type = null;

                foreach (var assembly in assemblies) {
                    if (!assembly.FullName.StartsWith("Unity.Cinemachine.Editor")) continue;
                    type = assembly.GetType("Unity.Cinemachine.Editor.InspectorUtility+FoldoutWithOverlay");
                    if (type != null) {
                        break;
                    }
                }

                if (type == null) {
                    UnityEngine.Debug.LogError("Rewired: Cannot find InspectorUtility.FoldoutWithOverlay class. " + reflectionErrorMessagePrefix);
                    _reflectionError = true;
                    return null;
                }

                return type;
            }
        }

        public bool isValid { get { return !_reflectionError; } }

        public InspectorUtility() {
            if (_reflectionError) return;
            var x = inspectorUtility_FoldoutWithOverlay;
            if (_reflectionError) return;
            VisualElement f = CreateFoldoutWithOverlay(new Foldout(), new VisualElement(), null, 0, 0);
            if (f == null) {
                _reflectionError = true;
                UnityEngine.Debug.LogError("Rewired: Cannot create instance of InspectorUtility.FoldoutWithOverlay class. " + reflectionErrorMessagePrefix);
            }
        }

        public VisualElement CreateFoldoutWithOverlay(Foldout foldout, VisualElement visualElement, Label label, int marginLeft, int marginRight) {
            VisualElement result = Activator.CreateInstance(
                inspectorUtility_FoldoutWithOverlay,
                BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance,
                null,
                new object[] {
                    foldout, visualElement, label
                },
                CultureInfo.CurrentCulture
            ) as VisualElement;
            if (result != null) {
                result.style.marginLeft = marginLeft;
                result.style.marginRight = marginRight;
            }
            return result;
        }
    }
}