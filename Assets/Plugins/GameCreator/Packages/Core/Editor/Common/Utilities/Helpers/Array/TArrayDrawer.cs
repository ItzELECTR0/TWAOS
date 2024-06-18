using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TArrayDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Utilities/Helpers/Array/TArray";

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string PropertyArrayName { get; }
        protected abstract float ItemHeight { get; }

        // IMPLEMENT METHODS: ---------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return this.CreateArrayProperty(property);
        }

        protected VisualElement CreateArrayProperty(SerializedProperty property)
        {
            VisualElement root = new VisualElement { name = "GC-Array-Root" };
            VisualElement head = new VisualElement { name = "GC-Array-Head" };
            VisualElement body = new VisualElement { name = "GC-Array-Body" };
            VisualElement foot = new VisualElement { name = "GC-Array-Foot" };

            root.Add(head);
            root.Add(body);
            root.Add(foot);
            
            this.SetupStyleSheets(root);

            SerializedProperty list = property.FindPropertyRelative(this.PropertyArrayName);
            TArrayTool arrayTool = this.CreateArrayTool(list, this.ItemHeight);
            
            body.Add(arrayTool);

            return root;
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual TArrayTool CreateArrayTool(SerializedProperty propertyArray, float height)
        {
            return new TArrayTool(propertyArray, height);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SetupStyleSheets(VisualElement root)
        {
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }
        }
    }
}