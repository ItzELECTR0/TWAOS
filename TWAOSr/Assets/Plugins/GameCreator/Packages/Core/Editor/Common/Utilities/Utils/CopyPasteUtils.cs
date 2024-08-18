using System;
using UnityEngine;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    public static class CopyPasteUtils
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] private static object SourceObject { get; set; }
        [field: NonSerialized] private static Type SourceType { get; set; }

        public static object SourceObjectCopy
        {
            get
            {
                if (SourceObject == null) return null;
                
                string jsonSource = EditorJsonUtility.ToJson(SourceObject);
                object newInstance = Activator.CreateInstance(SourceType);
                
                EditorJsonUtility.FromJsonOverwrite(jsonSource, newInstance);
                EventSoftPaste?.Invoke();
                
                return newInstance;
            }
        }

        // EVENTS: --------------------------------------------------------------------------------

        public static event Action EventSoftCopy;
        public static event Action EventSoftPaste;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void CopyToClipboard(string text)
        {
            TextEditor textEditor = new TextEditor { text = text };

            textEditor.SelectAll();
            textEditor.Copy();
        }

        public static void SoftCopy(object source, Type baseType)
        {
            SourceType = source.GetType();
            SourceObject = source;

            EventSoftCopy?.Invoke();
        }

        public static bool CanSoftPaste(Type baseType)
        {
            if (SourceObject == null) return false;
            return SourceType != null && baseType.IsAssignableFrom(SourceType);
        }

        public static void Duplicate(SerializedProperty target, object source)
        {
            if (source == null) return;
            if (target.propertyType != SerializedPropertyType.ManagedReference) return;
            
            string jsonSource = EditorJsonUtility.ToJson(source);
            
            object newInstance = Activator.CreateInstance(source.GetType());
            EditorJsonUtility.FromJsonOverwrite(jsonSource, newInstance);

            target.SetManaged(newInstance);
        }

        public static void ClearCopy()
        {
            SourceType = null;
            SourceObject = null;
        }
    }
}
