using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections;
using GameCreator.Runtime.Common;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    public static class SerializationUtils
    {
        // CONSTANTS: -----------------------------------------------------------------------------
        
        private const BindingFlags BINDINGS = BindingFlags.Instance |
                                              BindingFlags.Static   |
                                              BindingFlags.Public   |
                                              BindingFlags.NonPublic;

        private static readonly Regex RX_ARRAY = new Regex(@"\[\d+\]");

        private const string SPACE = " ";
        
        private const string ERR_GENERIC = "Generic access is to be deprecated. Review '{0}'";
        
        // ENUMS: ---------------------------------------------------------------------------------
        
        public enum ChildrenMode
        {
            ShowLabelsInChildren,
            HideLabelsInChildren,
            FullWidthChildren
        }

        // UTILITIES: -----------------------------------------------------------------------------

        public static string GetNiceTypeName(this SerializedProperty property)
        {
            return TypeUtils.GetNiceName(property.managedReferenceFullTypename);
        }

        public static bool HideLabelsInEditor(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference) return false;

            Type type = TypeUtils.GetTypeFromProperty(property, true);
            HideLabelsInEditorAttribute hideLabels = type?
                .GetCustomAttribute<HideLabelsInEditorAttribute>();

            return hideLabels is { Hide: true };
        }

        // UI TOOLKIT: ----------------------------------------------------------------------------

        public static bool CreateChildProperties(VisualElement root, SerializedProperty prop,
            ChildrenMode mode, bool indent, params string[] excludeFields)
        {
            SerializedProperty iteratorProperty = prop.Copy();
            SerializedProperty endProperty = iteratorProperty.GetEndProperty();
        
            int numProperties = 0;
            if (!iteratorProperty.NextVisible(true)) return false;
        
            do
            {
                if (SerializedProperty.EqualContents(iteratorProperty, endProperty)) break;
                if (excludeFields.Contains(iteratorProperty.name)) continue;
                
                PropertyField field = mode switch
                {
                    ChildrenMode.ShowLabelsInChildren => new PropertyField(iteratorProperty),
                    ChildrenMode.HideLabelsInChildren => new PropertyField(iteratorProperty, SPACE),
                    ChildrenMode.FullWidthChildren => new PropertyField(iteratorProperty, string.Empty),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
        
                root.Add(field);
                numProperties += 1;
            } while (iteratorProperty.NextVisible(false));
        
            root.Bind(prop.serializedObject);
            return numProperties != 0;
        }

        // UPDATE SERIALIZATION: ------------------------------------------------------------------

        public static void ApplyUnregisteredSerialization(SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
            
            Component component = serializedObject.targetObject as Component;
            if (component == null || !component.gameObject.scene.isLoaded) return;
            
            if (Application.isPlaying) return;
            EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
        }
        
        // GET MANAGED REFERENCES: ----------------------------------------------------------------

        public static object GetManagedValue(this SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ManagedReference 
                ? property.managedReferenceValue 
                : null;
        }
        
        public static T GetValue<T>(this SerializedProperty property)
        {
            // Now that Unity supports managedReferenceValue 'getters' use it by default.
            // However there is no way at the moment to get the value of a generic object
            // so instead, use the object-path traverse method.
            
            // Update 5/2/2022: There is a new boxed object value property available inside the
            // SerializedProperty class. Might be what we are looking for.
            // Resolution: Negative. It would work, but if the boxed value contains any
            // UnityEngine.Object reference the deserialization fails and throws an exception.
            
            // Update 18/9/2023: Since Unity has provided much more support for serialized
            // references it is clear that generic data should never be accessed and modified
            // as-is. Therefore all generic data should be converted to managed reference values.

            if (property == null) return default;
            ApplyUnregisteredSerialization(property.serializedObject);
            
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                return property.managedReferenceValue is T managedReference
                    ? managedReference
                    : default;
            }

            Debug.LogWarning(string.Format(ERR_GENERIC, property.propertyPath));
            
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");
        
            string[] fieldStructure = path.Split('.');
            foreach (string field in fieldStructure)
            {
                if (field.Contains("["))
                {
                    int index = Convert.ToInt32(new string(field
                        .Where(char.IsDigit)
                        .ToArray()
                    ));
                
                    obj = GetFieldValueWithIndex(
                        RX_ARRAY.Replace(field, string.Empty), 
                        obj, 
                        index
                    );
                }
                else
                {
                    obj = GetFieldValue(field, obj);
                }
            }
        
            return (T) obj;
        }

        private static object GetFieldValue(string fieldName, object obj)
        {
            FieldInfo field = obj?.GetType().GetField(fieldName, BINDINGS);
            return field != null ? field.GetValue(obj) : default;
        }
        
        private static object GetFieldValueWithIndex(string fieldName, object obj, int index)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, BINDINGS);
            if (field == null) return default;
            
            object list = field.GetValue(obj);
            
            if (list.GetType().IsArray)
            {
                return ((object[])list)[index];
            }
            
            return list is IEnumerable 
                ? ((IList)list)[index] 
                : default;
        }

        // SET MANAGED REFERENCES: ----------------------------------------------------------------

        public static void SetManaged(this SerializedProperty property, object value)
        {
            property.managedReferenceValue = value;
        }
        
        public static void SetValue<T>(this SerializedProperty property, T value)
        {
            property.managedReferenceValue = value;
        }
    }
}