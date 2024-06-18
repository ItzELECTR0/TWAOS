using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class BaseTextAreaDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Utilities/Helpers/TextArea/TextArea";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            TextField textArea = new TextField
            {
                label = this.Label(property),
                multiline = true,
                tripleClickSelectsLine = true,
                name = "GC-TextArea",
                bindingPath = property.FindPropertyRelative("m_Text").propertyPath
            };
            
            textArea.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);

            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);

            root.Add(textArea);
            return root;
        }

        protected abstract string Label(SerializedProperty property);
    }
}