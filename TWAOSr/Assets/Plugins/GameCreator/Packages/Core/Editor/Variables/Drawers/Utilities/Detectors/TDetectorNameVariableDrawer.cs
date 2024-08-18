using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TDetectorNameVariableDrawer : PropertyDrawer
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
            
            ObjectField fieldVariable = new ObjectField(variables.displayName)
            {
                allowSceneObjects = this.AllowSceneReferences,
                objectType = this.AssetType,
                bindingPath = variables.propertyPath
            };
            
            fieldVariable.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);

            PropertyField fieldWhen = new PropertyField(when);
            
            head.Add(fieldVariable);
            head.Add(fieldWhen);
            
            fieldWhen.RegisterValueChangeCallback(_ =>
            {
                body.Clear();
                if (when.enumValueIndex == 1) // When.Name
                {
                    body.Add(this.Tool(fieldVariable, property));
                }
            });

            if (when.enumValueIndex == 1) // When.Name
            {
                body.Add(this.Tool(fieldVariable, property));
            }
            
            return root;
        }

        protected abstract Type AssetType { get; }
        protected abstract bool AllowSceneReferences { get; }
        
        protected abstract TNamePickTool Tool(ObjectField field, SerializedProperty property);

    }
}