using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TTitleDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Utilities/Helpers/Title/TitleDrawer";

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string Title { get; }

        protected virtual string ExtraStyleSheetPath => string.Empty;
        
        // IMPLEMENT METHODS: ---------------------------------------------------------------------
        
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            root.Add(head);
            root.Add(body);
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH, this.ExtraStyleSheetPath);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }

            Label labelTitle = new Label(this.Title);
            labelTitle.AddToClassList("gc-label-title");
            
            head.Add(labelTitle);
            this.CreateContent(root, property);

            return root;
        }

        protected virtual void CreateContent(VisualElement body, SerializedProperty property)
        {
            SerializationUtils.CreateChildProperties(
                body,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );
        }
    }
}
