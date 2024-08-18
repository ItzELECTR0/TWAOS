using System.Collections;
using System.Collections.Generic;
using GameCreator.Runtime.Cameras;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(TShotType), true)]
    public class TShotTypeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new TShotTypeElement(property);
        }
    }
}
