using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(VolumeCapsule))]
    public class VolumeCapsuleDrawer : VolumeDrawer
    {
        protected override void CreateGUI(SerializedProperty property, VisualElement root)
        {
            SerializedProperty center = property.FindPropertyRelative("m_Center");
            SerializedProperty height = property.FindPropertyRelative("m_Height");
            SerializedProperty radius = property.FindPropertyRelative("m_Radius");
            SerializedProperty direction = property.FindPropertyRelative("m_Direction");

            PropertyField fieldCenter = new PropertyField(center);
            PropertyField fieldHeight = new PropertyField(height);
            PropertyField fieldRadius = new PropertyField(radius);
            PropertyField fieldDirection = new PropertyField(direction);

            root.Add(fieldCenter);
            root.Add(fieldHeight);
            root.Add(fieldRadius);
            root.Add(fieldDirection);
        }
    }
}