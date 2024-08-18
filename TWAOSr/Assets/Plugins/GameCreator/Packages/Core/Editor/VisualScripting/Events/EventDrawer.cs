using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(GameCreator.Runtime.VisualScripting.Event), true)]
    public class EventDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new EventElement(property);
        }
    }
}