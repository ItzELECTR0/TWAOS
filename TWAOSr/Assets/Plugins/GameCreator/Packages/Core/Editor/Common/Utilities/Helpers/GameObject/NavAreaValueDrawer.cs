using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(NavAreaValue))]
    public class NavAreaValueDrawer : TNavAreaDrawer
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
            SerializedProperty index = property.FindPropertyRelative("m_Index");
            
            PopupField<int> maskField = new PopupField<int>(
                property.displayName,
                new List<int>(OPTIONS), index.intValue,
                PrintSelection,
                PrintListItem
            );

            maskField.RegisterValueChangedCallback(changeEvent =>
            {
                index.serializedObject.Update();
                index.intValue = changeEvent.newValue;
                
                SerializationUtils.ApplyUnregisteredSerialization(index.serializedObject);
            });
            
            maskField.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);
            AlignLabel.On(maskField);

            return maskField;
        }

        private static string PrintSelection(int index)
        {
            string name = IndexToName(index);
            return !string.IsNullOrEmpty(name) 
                ? name
                : string.Empty;
        }
        
        private static string PrintListItem(int index)
        {
            string name = IndexToName(index);
            return !string.IsNullOrEmpty(name) 
                ? $"{index}: {name}"
                : string.Empty;
        }
    }
}