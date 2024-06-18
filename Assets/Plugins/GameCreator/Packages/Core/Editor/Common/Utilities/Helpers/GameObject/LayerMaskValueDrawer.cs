using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(LayerMaskValue))]
    public class LayerMaskValueDrawer : PropertyDrawer
    {
        private static readonly List<int> OPTIONS = new List<int>
        {
            00, 01, 02, 03, 04, 05, 06, 07,
            08, 09, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29, 30, 31
        };

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative("m_Value");
            
            PopupField<int> maskField = new PopupField<int>(
                property.displayName,
                new List<int>(OPTIONS), value.intValue,
                this.PrintSelection,
                this.PrintListItem
            );

            maskField.RegisterValueChangedCallback(changeEvent =>
            {
                value.serializedObject.Update();
                value.intValue = changeEvent.newValue;
                
                SerializationUtils.ApplyUnregisteredSerialization(value.serializedObject);
            });
            
            maskField.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);
            AlignLabel.On(maskField);

            return maskField;
        }

        private string PrintSelection(int index)
        {
            string name = LayerMask.LayerToName(index);
            return !string.IsNullOrEmpty(name) 
                ? name
                : string.Empty;
        }
        
        private string PrintListItem(int index)
        {
            string name = LayerMask.LayerToName(index);
            return !string.IsNullOrEmpty(name) 
                ? $"{index}: {name}"
                : string.Empty;
        }
    }
}