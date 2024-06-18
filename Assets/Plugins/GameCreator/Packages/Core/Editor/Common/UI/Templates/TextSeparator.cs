using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class TextSeparator : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/TextSeparator";

        private const string NAME_SEPARATOR = "GC-TextSeparator-Line";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly Label m_Label;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Text
        {
            get => this.m_Label.text;
            set => this.m_Label.text = value;
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public TextSeparator(string text)
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);

            VisualElement lineL = new VisualElement
            {
                name = NAME_SEPARATOR,
                style = { backgroundColor = ColorTheme.Get(ColorTheme.Type.TextLight) }
            };
            VisualElement lineR = new VisualElement
            {
                name = NAME_SEPARATOR,
                style = { backgroundColor = ColorTheme.Get(ColorTheme.Type.TextLight) }
            };
            
            this.m_Label = new Label { text = text };

            this.Add(lineL);
            this.Add(this.m_Label);
            this.Add(lineR);
        }
    }
}
