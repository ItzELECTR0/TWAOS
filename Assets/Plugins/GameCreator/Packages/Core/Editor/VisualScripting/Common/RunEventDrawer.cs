using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(RunEvent))]
    public class RunEventDrawer : PropertyDrawer
    {
        public const string PROP_EVENT = "m_Event";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty eventCall = property.FindPropertyRelative(PROP_EVENT);
            return new PropertyField(eventCall);
        }
    }
}