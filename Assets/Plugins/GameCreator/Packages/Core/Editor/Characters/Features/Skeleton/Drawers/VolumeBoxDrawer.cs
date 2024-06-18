using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(VolumeBox))]
    public class VolumeBoxDrawer : VolumeDrawer
    {
        protected override void CreateGUI(SerializedProperty property, VisualElement root)
        {
            SerializedProperty center = property.FindPropertyRelative("m_Center");
            SerializedProperty size = property.FindPropertyRelative("m_Size");

            PropertyField fieldCenter = new PropertyField(center);
            PropertyField fieldSize = new PropertyField(size);

            root.Add(fieldCenter);
            root.Add(fieldSize);
        }
    }
}