using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(RigAlignGround))]
    public class RigAlignGroundDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty smoothTime = property.FindPropertyRelative("m_SmoothTime");
            SerializedProperty maxAngle = property.FindPropertyRelative("m_MaxAngle");
            
            VisualElement root = new VisualElement();
            
            root.Add(new PropertyField(smoothTime));
            root.Add(new PropertyField(maxAngle));

            return root;
        }
    }
}