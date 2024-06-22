//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

#endif

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Min and max range of int (inclusive)
    /// </summary>
    [System.Serializable]
    public struct RangeOfIntegers
    {
        /// <summary>Minimum value (inclusive)</summary>
        [Tooltip("Minimum value (inclusive)")]
        public int Minimum;

        /// <summary>Maximum value (inclusive)</summary>
        [Tooltip("Maximum value (inclusive)")]
        public int Maximum;

        /// <summary>
        /// Generate a random value
        /// </summary>
        /// <returns></returns>
		public int Random() { return UnityEngine.Random.Range(Minimum, Maximum + 1); }

        /// <summary>
        /// Generate a random value with a random instance
        /// </summary>
        /// <param name="r">Random</param>
        /// <returns>Random value</returns>
        public int Random(System.Random r) { return r.Next(Minimum, Maximum + 1); }
    }

    /// <summary>
    /// Min and max range of floats (inclusive)
    /// </summary>
    [System.Serializable]
    public struct RangeOfFloats
    {
        /// <summary>Minimum value (inclusive)</summary>
        [Tooltip("Minimum value (inclusive)")]
        public float Minimum;

        /// <summary>Maximum value (inclusive)</summary>
        [Tooltip("Maximum value (inclusive)")]
        public float Maximum;

        /// <summary>
        /// Generate a random value
        /// </summary>
        /// <returns></returns>
		public float Random() { return UnityEngine.Random.Range(Minimum, Maximum); }

        /// <summary>
        /// Generate a random value with a random instance
        /// </summary>
        /// <param name="r">Random</param>
        /// <returns>Random value</returns>
        public float Random(System.Random r) { return Minimum + ((float)r.NextDouble() * (Maximum - Minimum)); }
    }

    /// <summary>
    /// Apply to a field to make it render in one line
    /// </summary>
    public class SingleLineAttribute : PropertyAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tooltip">Tooltip</param>
        public SingleLineAttribute(string tooltip) { Tooltip = tooltip; }

        /// <summary>
        /// Tooltip
        /// </summary>
        public string Tooltip { get; private set; }
    }

    /// <summary>
    /// Same as SingleLineAttribute but with clamp
    /// </summary>
    public class SingleLineClampAttribute : SingleLineAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tooltip">Tooltip</param>
        /// <param name="minValue">Min value</param>
        /// <param name="maxValue">Max value</param>
        public SingleLineClampAttribute(string tooltip, double minValue, double maxValue) : base(tooltip)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        /// Min value
        /// </summary>
        public double MinValue { get; private set; }

        /// <summary>
        /// Max value
        /// </summary>
        public double MaxValue { get; private set; }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Single line drawer, used on Vector4, RangeOfFloats and RangeOfIntegers
    /// </summary>
    [CustomPropertyDrawer(typeof(SingleLineAttribute))]
    [CustomPropertyDrawer(typeof(SingleLineClampAttribute))]
    public class SingleLineDrawer : PropertyDrawer
    {
        private void DrawIntTextField(Rect position, string text, string tooltip, SerializedProperty prop)
        {
            EditorGUI.BeginChangeCheck();
            int value = EditorGUI.IntField(position, new GUIContent(text, tooltip), prop.intValue);
            SingleLineClampAttribute clamp = attribute as SingleLineClampAttribute;
            if (clamp != null)
            {
                value = Mathf.Clamp(value, (int)clamp.MinValue, (int)clamp.MaxValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                prop.intValue = value;
            }
        }

        private void DrawFloatTextField(Rect position, string text, string tooltip, SerializedProperty prop)
        {
            EditorGUI.BeginChangeCheck();
            float value = EditorGUI.FloatField(position, new GUIContent(text, tooltip), prop.floatValue);
            SingleLineClampAttribute clamp = attribute as SingleLineClampAttribute;
            if (clamp != null)
            {
                value = Mathf.Clamp(value, (float)clamp.MinValue, (float)clamp.MaxValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        private void DrawRangeField(Rect position, SerializedProperty prop, bool floatingPoint)
        {
            EditorGUIUtility.labelWidth = 30.0f;
            EditorGUIUtility.fieldWidth = 40.0f;
            float width = position.width * 0.49f;
            float spacing = position.width * 0.02f;
            position.width = width;
            if (floatingPoint)
            {
                DrawFloatTextField(position, "Min", "Minimum value", prop.FindPropertyRelative("Minimum"));
            }
            else
            {
                DrawIntTextField(position, "Min", "Minimum value", prop.FindPropertyRelative("Minimum"));
            }
            position.x = position.xMax + spacing;
            position.width = width;
            if (floatingPoint)
            {
                DrawFloatTextField(position, "Max", "Maximum value", prop.FindPropertyRelative("Maximum"));
            }
            else
            {
                DrawIntTextField(position, "Max", "Maximum value", prop.FindPropertyRelative("Maximum"));
            }
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="prop">Property</param>
        /// <param name="label">Label</param>
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label.text, (attribute as SingleLineAttribute).Tooltip));

            switch (prop.type)
            {
                case "RangeOfIntegers":
                    DrawRangeField(position, prop, false);
                    break;

                case "RangeOfFloats":
                    DrawRangeField(position, prop, true);
                    break;

                default:
                    EditorGUI.HelpBox(position, "[SingleLineDrawer] doesn't work with type '" + prop.type + "'", MessageType.Error);
                    break;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

#endif

}
