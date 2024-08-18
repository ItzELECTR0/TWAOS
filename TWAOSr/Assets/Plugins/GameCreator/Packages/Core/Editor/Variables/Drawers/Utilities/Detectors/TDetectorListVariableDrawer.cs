using System;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TDetectorListVariableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            root.Add(head);
            root.Add(body);

            SerializedProperty variables = property.FindPropertyRelative("m_Variable");
            SerializedProperty when = property.FindPropertyRelative("m_When");
            SerializedProperty index = property.FindPropertyRelative("m_Index");

            PropertyField fieldVariable = new PropertyField(variables);
            PropertyField fieldWhen = new PropertyField(when);

            head.Add(fieldVariable);
            head.Add(fieldWhen);

            fieldWhen.RegisterValueChangeCallback(_ =>
            {
                body.Clear();
                if (when.enumValueIndex == 1) // When.SetIndex
                {
                    PropertyElement indexTool = new PropertyElement(index, index.displayName, true);
                    body.Add(indexTool);
                }
            });

            if (when.enumValueIndex == 1) // When.SetIndex
            {
                PropertyElement indexTool = new PropertyElement(index, index.displayName, true);
                body.Add(indexTool);
            }
            
            return root;
        }
    }
}