using GameCreator.Runtime.Characters.IK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(RigLookTo))]
    public class RigLookToDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty trackSpeed = property.FindPropertyRelative("m_TrackSpeed");
            SerializedProperty maxAngle = property.FindPropertyRelative("m_MaxAngle");
            SerializedProperty sections = property.FindPropertyRelative("m_Sections");

            VisualElement root = new VisualElement();
            
            root.Add(new PropertyField(trackSpeed));
            root.Add(new PropertyField(maxAngle));
            root.Add(new PropertyField(sections));

            return root;
        }
    }
}