using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(NavAreaMask))]
    public class NavAreaMaskDrawer : TNavAreaDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty mask = property.FindPropertyRelative("m_Mask");
            
            MaskField maskField = new MaskField(
                property.displayName,
                new List<string>(Areas()),
                mask.intValue,
                PrintSelection,
                PrintListItem
            );

            maskField.RegisterValueChangedCallback(changeEvent =>
            {
                mask.serializedObject.Update();
                mask.intValue = changeEvent.newValue;
                
                SerializationUtils.ApplyUnregisteredSerialization(mask.serializedObject);
            });

            return maskField;
        }

        private static string PrintSelection(string name) => name;
        private static string PrintListItem(string name) => name;
    }
}