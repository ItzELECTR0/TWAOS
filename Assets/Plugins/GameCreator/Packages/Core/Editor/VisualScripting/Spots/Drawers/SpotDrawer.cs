using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(Spot), true)]
    public class SpotDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            SerializationUtils.CreateChildProperties(
                container,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );

            return container;
        }
    }
}