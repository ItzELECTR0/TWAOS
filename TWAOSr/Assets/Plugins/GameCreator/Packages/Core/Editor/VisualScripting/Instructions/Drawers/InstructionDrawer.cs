﻿using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.VisualScripting;
using GameCreator.Editor.Common;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(Instruction))]
    public class InstructionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            SerializationUtils.CreateChildProperties(
                container,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );

            return container;
        }
    }
}