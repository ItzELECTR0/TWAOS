using UnityEditor;
using UnityEngine;
using GameCreator.Runtime.Common;
using System;

namespace GameCreator.Editor.Common
{
    public abstract class ConditionBaseDrawer : PropertyDrawer
    {
        protected bool FieldValue(SerializedProperty property)
        {
            if (this.attribute is not TConditionAttribute condition) return true;
            bool result = true;

            foreach (string field in condition.Fields)
            {
                result &= this.CheckField(property, field);
            }

            return result;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private bool CheckField(SerializedProperty property, string fieldName)
        {
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, fieldName);

            SerializedProperty field = property.serializedObject.FindProperty(conditionPath);
            if (field == null) return false;

            switch (field.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return field.intValue != 0;

                case SerializedPropertyType.Boolean:
                    return field.boolValue;

                case SerializedPropertyType.Float:
                    return Math.Abs(field.floatValue) > float.Epsilon;

                case SerializedPropertyType.String:
                    return !string.IsNullOrEmpty(field.stringValue);

                case SerializedPropertyType.Color:
                    return field.colorValue != Color.black;

                case SerializedPropertyType.ObjectReference:
                    return field.objectReferenceValue != null;

                case SerializedPropertyType.LayerMask:
                    return field.intValue != 0;

                case SerializedPropertyType.ArraySize:
                    return field.arraySize != 0;

                case SerializedPropertyType.Character:
                    return !string.IsNullOrEmpty(field.stringValue);

                case SerializedPropertyType.ExposedReference:
                    return field.exposedReferenceValue != null;

                case SerializedPropertyType.FixedBufferSize:
                    return field.fixedBufferSize > 0;

                case SerializedPropertyType.ManagedReference:
                    return field.GetValue<object>() != null;

                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Hash128:
                    return true;
                
                default:
                    Debug.LogErrorFormat("Unhandled Type: {0}", field.propertyType);
                    return false;
            }
        }

        // HEIGHT METHOD: -------------------------------------------------------------------------

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}