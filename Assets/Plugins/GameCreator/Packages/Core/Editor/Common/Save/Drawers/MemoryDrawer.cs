using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(Memory), true)]
    public class MemoryDrawer : PropertyDrawer
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