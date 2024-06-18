using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(LookSection))]
    public class LookSectionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty bone = property.FindPropertyRelative("m_Bone");
            SerializedProperty euler = property.FindPropertyRelative("m_Euler");
            SerializedProperty weight = property.FindPropertyRelative("m_Weight");
            
            VisualElement root = new VisualElement();
            
            root.Add(new SpaceSmaller());
            root.Add(new PropertyField(bone));
            root.Add(new PropertyField(euler));
            root.Add(new PropertyField(weight));
            root.Add(new SpaceSmaller());

            return root;
        }
    }
}