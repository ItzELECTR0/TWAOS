using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class DocumentationPopup : EditorWindow
    {
        private static readonly Vector2 MIN_SIZE = new Vector2(400, 400);

        private const string USS_PATH = EditorPaths.COMMON + "Documentation/Styles/Popup";

        private const string NAME_CONTENT = "GC-Documentation-Window-Content";
        
        // STATIC PROPERTIES: ---------------------------------------------------------------------

        private static DocumentationPopup Window;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static void Open(Type itemType)
        {
            if (itemType == null) return;
            if (Window != null)
            {
                Window.Close();
                return;
            }

            Window = CreateInstance<DocumentationPopup>();
            Window.titleContent = new GUIContent("Documentation");
            Window.minSize = MIN_SIZE;

            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                Window.rootVisualElement.styleSheets.Add(styleSheet);
            }
            
            DocumentationComplete documentation = new DocumentationComplete(itemType);
            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.contentContainer.name = NAME_CONTENT;
            
            scrollView.Add(documentation);
            Window.rootVisualElement.Add(scrollView);
            
            Window.ShowAuxWindow();
        }

        private void OnDestroy()
        {
            Window = null;
        }
    }
}
