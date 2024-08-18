using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class PropertyEditor : PropertyDrawer
    {
        public struct PropertyKey : IEqualityComparer<PropertyKey> 
        {
            private readonly int instanceId;
            private readonly string propertyPath;
            
            // CONSTRUCTORS: ----------------------------------------------------------------------

            public PropertyKey(SerializedProperty property)
            {
                this.instanceId = property.serializedObject.targetObject.GetInstanceID();
                this.propertyPath = property.propertyPath;
            }

            public PropertyKey(int instanceId, string propertyPath)
            {
                this.instanceId = instanceId;
                this.propertyPath = propertyPath;
            }
            
            // EQUALITY METHODS: ------------------------------------------------------------------

            public bool Equals(PropertyKey x, PropertyKey y)
            {
                return x.instanceId == y.instanceId && x.propertyPath == y.propertyPath;
            }

            public int GetHashCode(PropertyKey propertyKey)
            {
                return HashCode.Combine(propertyKey.instanceId, propertyKey.propertyPath);
            }
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // STATIC MEMBERS: ------------------------------------------------------------------------

        private const string ERR_GET = "Invalid SerializedProperty. Get using root PropertyEditor";
        private const string ERR_SET = "Invalid SerializedProperty. Set using root PropertyEditor";

        private static readonly PropertyName DATA_PROPERTY = "property";
        private static readonly PropertyName DATA_ROOT = "root";

        private static readonly Dictionary<PropertyKey, Dictionary<PropertyName, object>> Values;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        static PropertyEditor()
        {
            Values = new Dictionary<PropertyKey, Dictionary<PropertyName, object>>();
        }
        
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyKey propertyKey = new PropertyKey(property);
            Values[propertyKey] = new Dictionary<PropertyName, object>();

            VisualElement root = new VisualElement { userData = propertyKey };
            
            SetData(property, DATA_PROPERTY, property);
            SetData(property, DATA_ROOT, root);
            
            root.Add(this.OnCreate(property));
            
            root.RegisterCallback<AttachToPanelEvent>(this.OnAttach);
            root.RegisterCallback<DetachFromPanelEvent>(this.OnDetach);
            
            return root;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnAttach(AttachToPanelEvent eventAttach)
        {
            VisualElement root = eventAttach.target as VisualElement;
            if (root == null) return;
            
            PropertyKey propertyKey = (PropertyKey) root.userData;
            SerializedProperty property = GetData<SerializedProperty>(propertyKey, DATA_PROPERTY);
            
            this.OnEnable(root, property);
        }

        private void OnDetach(DetachFromPanelEvent eventDetach)
        {
            VisualElement root = eventDetach.target as VisualElement;
            
            this.OnDisable(root);
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected static T GetData<T>(SerializedProperty property, PropertyName key)
        {
            PropertyKey propertyKey = new PropertyKey(property);
            return GetData<T>(propertyKey, key);
        }

        protected static T GetData<T>(PropertyKey propertyKey, PropertyName key)
        {
            if (Values.TryGetValue(propertyKey, out Dictionary<PropertyName, object> values))
            {
                return values.TryGetValue(key, out object result) && result is T typeResult
                    ? typeResult
                    : default;
            }
            
            Debug.LogError(ERR_GET);
            return default;
        }

        protected static void SetData<T>(SerializedProperty property, PropertyName key, T value)
        {
            PropertyKey propertyKey = new PropertyKey(property);
            SetData(propertyKey, key, value);
        }
        
        protected static void SetData<T>(PropertyKey propertyKey, PropertyName key, T value)
        {
            if (Values.TryGetValue(propertyKey, out Dictionary<PropertyName, object> values))
            {
                values[key] = value;
                return;
            }

            Debug.LogError(ERR_SET);
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------
        
        protected virtual VisualElement OnCreate(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }

        protected virtual void OnEnable(VisualElement root, SerializedProperty property)
        { }
        
        protected virtual void OnDisable(VisualElement root)
        { }
    }
}