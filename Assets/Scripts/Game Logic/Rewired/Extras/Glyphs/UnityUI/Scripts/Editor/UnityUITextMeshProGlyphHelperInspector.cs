// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2018 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2018_PLUS
#endif

#if UNITY_2018_PLUS
#define REWIRED_SUPPORTS_TMPRO
#endif

#if REWIRED_SUPPORTS_TMPRO

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI.Editor {

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [UnityEditor.CustomEditor(typeof(UnityUITextMeshProGlyphHelper))]
    public sealed class UnityUITextMeshProGlyphHelperInspector : UnityEditor.Editor {

        private const string fieldName_text = "_text";
        private const string fieldName_options = "_options";
        private const string fieldName_spriteOptions = "_spriteOptions";
        private const string fieldName_baseSpriteMaterial = "_baseSpriteMaterial";
        private const string fieldName_overrideSpriteMaterialProperties = "_overrideSpriteMaterialProperties";
        private const string fieldName_spriteMaterialProperties = "_spriteMaterialProperties";
        private const string fieldName_spriteMaterialProperties_color = fieldName_spriteMaterialProperties + "._color";

        new private UnityUITextMeshProGlyphHelper target { get { return (UnityUITextMeshProGlyphHelper)base.target; } }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName_text));
            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName_options));
            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName_spriteOptions), true);
            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName_baseSpriteMaterial));
            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName_overrideSpriteMaterialProperties));
            if (serializedObject.FindProperty(fieldName_overrideSpriteMaterialProperties).boolValue) {
                UnityEditor.EditorGUI.indentLevel += 1;
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName_spriteMaterialProperties_color));
                UnityEditor.EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
