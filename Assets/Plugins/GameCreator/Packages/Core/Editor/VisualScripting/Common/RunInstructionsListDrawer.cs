using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(RunInstructionsList))]
    public class RunInstructionsListDrawer : PropertyDrawer
    {
        public const string PROP_INSTRUCTIONS = "m_Instructions";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty instructions = property.FindPropertyRelative(PROP_INSTRUCTIONS);
            return new PropertyField(instructions);
        }
    }
}