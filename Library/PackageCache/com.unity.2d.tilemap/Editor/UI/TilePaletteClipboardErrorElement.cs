using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    [UxmlElement]
    public partial class TilePaletteClipboardErrorElement : VisualElement
    {
        private static readonly string ussClassName = "unity-tilepalette-clipboard-error-element";
        private static readonly string k_Name = L10n.Tr("Tile Palette Clipboard Error Element");

        static class Styles
        {
            public static readonly string emptyPaletteInfo = L10n.Tr("Drag Tile, Sprite or Texture (Sprite type) asset/s here.");
            public static readonly string invalidPaletteInfo = L10n.Tr("This is an invalid palette. Did you delete the palette asset?");
            public static readonly string invalidGridInfo = L10n.Tr("The palette has an invalid Grid. Did you add a Grid to the palette asset?");
            public static readonly string invalidDragAndDropInfo = L10n.Tr("You have dragged invalid items to the palette.");
        }

        private Label m_LabelElement;

        public TilePaletteClipboardErrorElement()
        {
            AddToClassList(ussClassName);

            name = k_Name;
            TilePaletteOverlayUtility.SetStyleSheet(this);

            m_LabelElement = new Label();
            Add(m_LabelElement);
        }

        public void SetEmptyPaletteText()
        {
            m_LabelElement.text = Styles.emptyPaletteInfo;
        }

        public void SetInvalidPaletteText()
        {
            m_LabelElement.text = Styles.invalidPaletteInfo;
        }

        public void SetInvalidGridText()
        {
            m_LabelElement.text = Styles.invalidGridInfo;
        }

        public void SetInvalidDragAndDropText()
        {
            m_LabelElement.text = Styles.invalidDragAndDropInfo;
        }

        public void ClearText()
        {
            m_LabelElement.text = null;
        }
    }
}
