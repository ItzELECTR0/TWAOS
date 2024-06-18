using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(Axonometry), true)]
    public class AxonometryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty reference = property.FindPropertyRelative("m_Axonometry");
            PropertyElement inputElement = new PropertyElement(
                reference,
                property.displayName, 
                false
            );
            
            root.Add(inputElement);
            inputElement.SetEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
            
            return root; 
        }
    }
}