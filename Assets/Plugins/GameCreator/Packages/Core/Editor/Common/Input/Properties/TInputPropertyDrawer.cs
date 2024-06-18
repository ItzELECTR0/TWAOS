using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TInputPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty reference = property.FindPropertyRelative(this.InputReference);
            PropertyElement inputElement = new PropertyElement(
                reference,
                property.displayName, 
                true
            );
            
            root.Add(inputElement);
            return root;
        }
        
        protected abstract string InputReference { get; }
    }
}
