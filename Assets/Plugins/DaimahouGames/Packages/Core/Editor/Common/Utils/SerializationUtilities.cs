using System;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Common
{
    public static class SerializationUtilities
    {
        private const BindingFlags BINDINGS = BindingFlags.Instance |
                                              BindingFlags.Static   |
                                              BindingFlags.Public   |
                                              BindingFlags.NonPublic;

        private static readonly Regex RX_ARRAY = new Regex(@"\[\d+\]");
        
        private const string SPACE = " ";

        public static bool CreateChildProperties(this SerializedObject serializedObject, VisualElement root,
            bool hideLabelsInChildren, params string[] excludeFields)
        {
            var iteratorProperty = serializedObject.GetIterator();
            iteratorProperty.Next(true);
            var endProperty = iteratorProperty.GetEndProperty(true);

            var numProperties = 0;
            if (!iteratorProperty.NextVisible(true)) return false;

            do
            {
                if (SerializedProperty.EqualContents(iteratorProperty, endProperty)) break;
                if (excludeFields.Contains(iteratorProperty.name)) continue;

                var field = hideLabelsInChildren
                    ? new PropertyField(iteratorProperty, SPACE)
                    : new PropertyField(iteratorProperty);

                root.Add(field);
                numProperties += 1;
            } while (iteratorProperty.NextVisible(false));

            root.Bind(serializedObject);
            return numProperties != 0;  
        }
        
        public static T GetSerializedValue<T>(this SerializedProperty property)
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

        private static void ApplyUnregisteredSerialization(SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
            
            Component component = serializedObject.targetObject as Component;
            if (component == null || !component.gameObject.scene.isLoaded) return;
            
            if (Application.isPlaying) return;
            EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
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
    }
}