using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(NameList))]
    public class NameListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new NameListTool(property);
        }
    }
}