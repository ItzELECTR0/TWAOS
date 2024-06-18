using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(IndexList))]
    public class IndexListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new IndexListTool(property);
        }
    }
}