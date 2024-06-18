using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Core
{
    [CustomEditor(typeof(StagingGizmos), true)]
    public class StagingGizmosEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            StyleSheet[] styleSheets = StyleSheetUtils.Load();
            foreach (StyleSheet styleSheet in styleSheets) root.styleSheets.Add(styleSheet);

            Button button = new Button(this.SelectAsset)
            {
                text = "Edit Asset",
                style = { height = new Length(25f, LengthUnit.Pixel) }
            };
            
            root.Add(new SpaceSmaller());
            root.Add(button);

            return root;
        }

        private void SelectAsset()
        {
            StagingGizmos component = this.target as StagingGizmos;
            if (component == null) return;

            component.SelectAsset();
        }
    }
}
