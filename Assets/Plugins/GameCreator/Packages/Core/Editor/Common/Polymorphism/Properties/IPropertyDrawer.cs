using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(IProperty), true)]
    public class IPropertyDrawer : PropertyDrawer
    {
        public const string PROPERTY_NAME = "m_Property";

        // PAINT METHOD: --------------------------------------------------------------------------

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty propertyItem = property.FindPropertyRelative(PROPERTY_NAME);

            SerializationUtils.CreateChildProperties(
                root, property, 
                propertyItem.HideLabelsInEditor()
                    ? SerializationUtils.ChildrenMode.HideLabelsInChildren
                    : SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true,
                PROPERTY_NAME
            );
            
            root.Add(new PropertyElement(
                propertyItem,
                property.displayName, 
                false
            ));

            return root;
        }
    }
}