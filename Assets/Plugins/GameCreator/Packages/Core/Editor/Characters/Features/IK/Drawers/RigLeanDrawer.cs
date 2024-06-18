using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(RigLean))]
    public class RigLeanDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty inclineSpine = property.FindPropertyRelative("m_InclineSpine");
            SerializedProperty inclineLowerChest = property.FindPropertyRelative("m_InclineLowerChest");
            SerializedProperty inclineUpperChest = property.FindPropertyRelative("m_InclineUpperChest");
            
            SerializedProperty declineSpine = property.FindPropertyRelative("m_DeclineSpine");
            SerializedProperty declineLowerChest = property.FindPropertyRelative("m_DeclineLowerChest");
            SerializedProperty declineUpperChest = property.FindPropertyRelative("m_DeclineUpperChest");
            
            SerializedProperty rollSpine = property.FindPropertyRelative("m_RollSpine");
            SerializedProperty rollLowerChest = property.FindPropertyRelative("m_RollLowerChest");
            SerializedProperty rollUpperChest = property.FindPropertyRelative("m_RollUpperChest");
            
            VisualElement root = new VisualElement();
            
            root.Add(new PropertyField(inclineSpine));
            root.Add(new PropertyField(inclineLowerChest));
            root.Add(new PropertyField(inclineUpperChest));
            
            root.Add(new SpaceSmaller());
            root.Add(new PropertyField(declineSpine));
            root.Add(new PropertyField(declineLowerChest));
            root.Add(new PropertyField(declineUpperChest));
            
            root.Add(new SpaceSmaller());
            root.Add(new PropertyField(rollSpine));
            root.Add(new PropertyField(rollLowerChest));
            root.Add(new PropertyField(rollUpperChest));

            return root;
        }
    }
}