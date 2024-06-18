using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(HandleItem))]
    public class HandleItemDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty conditions = property.FindPropertyRelative("m_Conditions");
            SerializedProperty bone = property.FindPropertyRelative("m_Bone");
            SerializedProperty position = property.FindPropertyRelative("m_LocalPosition");
            SerializedProperty rotation = property.FindPropertyRelative("m_LocalRotation");
            
            PropertyField fieldConditions = new PropertyField(conditions);
            root.Add(fieldConditions);
            
            PropertyField fieldBone = new PropertyField(bone);
            PropertyField fieldPosition = new PropertyField(position);
            PropertyField fieldRotation = new PropertyField(rotation);
            
            root.Add(new SpaceSmall());
            root.Add(fieldBone);
            root.Add(new SpaceSmaller());
            root.Add(fieldPosition);
            root.Add(new SpaceSmaller());
            root.Add(fieldRotation);
            
            return root;
        }
    }
}