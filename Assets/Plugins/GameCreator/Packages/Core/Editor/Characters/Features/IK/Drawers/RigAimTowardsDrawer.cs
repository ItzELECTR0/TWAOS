using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(RigAimTowards))]
    public class RigAimTowardsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty smoothTime = property.FindPropertyRelative("m_SmoothTime");
            SerializedProperty bone = property.FindPropertyRelative("m_Bone");
            SerializedProperty from = property.FindPropertyRelative("m_From");
            
            VisualElement root = new VisualElement();
            
            root.Add(new PropertyField(smoothTime));
            root.Add(new PropertyField(bone));
            
            root.Add(new SpaceSmall());
            root.Add(new PropertyField(from));

            return root;
        }
    }
}