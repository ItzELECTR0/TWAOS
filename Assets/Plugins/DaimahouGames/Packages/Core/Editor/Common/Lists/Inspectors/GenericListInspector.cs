using System;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class GenericListInspector<T> : ListInspector where T : class
    {
        //============================================================================================================||
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        
        public bool AllowTypeDuplicate { get; set; }
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        
        public event Action<T> EventElementInserted;

        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public GenericListInspector(SerializedProperty property) : base(property)
        {
            EventItemInserted += o => EventElementInserted?.Invoke((T) o);
        }
        
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|

        protected override Button CreateElementButton()
        {
            return new ListTypeSelector<T>(m_Property, this);
        }

        protected override GenericInspector MakeItemInspector(int index)
        {
            var inspector = new GenericInspector(this, index)
            {
                IsGeneric = true
            };
            return inspector;
        }

        protected override bool ValidateInsertion(object value)
        {
            if (AllowTypeDuplicate) return true;
            
            for (var i = 0; i < m_Property.arraySize; i++)
            {
                var objectType = value.GetType();
                var fullTypeName = $"{objectType.Assembly.GetName().Name} {objectType.FullName}";
                var propertyFullTypeName = m_Property.GetArrayElementAtIndex(i).managedReferenceFullTypename;

                if (fullTypeName != propertyFullTypeName) continue;

                Debug.LogWarning($"Duplication of type not allowed: {TypeUtils.GetNiceName(objectType)}");
                
                return false;
            }

            return true;
        }

        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}