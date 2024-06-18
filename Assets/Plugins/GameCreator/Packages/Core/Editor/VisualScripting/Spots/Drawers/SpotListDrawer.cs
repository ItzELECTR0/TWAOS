using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(SpotList))]
    public class SpotListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SpotListTool spotListTool = new SpotListTool(property);
            return spotListTool;
        }
    }
}