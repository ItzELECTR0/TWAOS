using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class SkeletonEditorPopup : EditorWindow
    {
        private static readonly Vector2 MIN_SIZE = new Vector2(400, 200);

        private const string USS_PATH = EditorPaths.CHARACTERS + "StyleSheets/SkeletonPopup";

        private const string NAME_CONTENT = "GC-Characters-Skeleton-Window-Content";
        
        // STATIC PROPERTIES: ---------------------------------------------------------------------

        private static SkeletonEditorPopup Window;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static void Open(Skeleton skeleton)
        {
            if (skeleton == null) return;
            if (Window != null)
            {
                Window.Close();
                return;
            }

            Window = CreateInstance<SkeletonEditorPopup>();
            Window.titleContent = new GUIContent("Skeleton Editor");
            Window.minSize = MIN_SIZE;

            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                Window.rootVisualElement.styleSheets.Add(styleSheet);
            }

            UnityEditor.Editor skeletonEditor = UnityEditor.Editor.CreateEditor(skeleton);
            VisualElement content = skeletonEditor.CreateInspectorGUI();
            content.Bind(new SerializedObject(skeleton));

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                contentContainer =
                {
                    name = NAME_CONTENT
                }
            };

            scrollView.Add(content);
            Window.rootVisualElement.Add(scrollView);
            
            Window.ShowAuxWindow();
        }

        private void OnDestroy()
        {
            Window = null;
        }
    }
}