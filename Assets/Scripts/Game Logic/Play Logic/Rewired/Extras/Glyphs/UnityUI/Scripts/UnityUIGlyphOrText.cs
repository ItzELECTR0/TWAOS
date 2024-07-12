// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Glyphs.UnityUI {

    public class UnityUIGlyphOrText : GlyphOrTextBase<UnityEngine.UI.Image, UnityEngine.Sprite, UnityEngine.UI.Text> {

        protected override string textString {
            get {
                return textComponent != null ? textComponent.text : string.Empty;
            }
            set {
                if (textComponent == null) return;
                textComponent.text = value;
            }
        }

        protected override UnityEngine.Sprite glyphGraphic {
            get {
                return glyphComponent != null ? glyphComponent.sprite : null;
            }
            set {
                if (glyphComponent == null) return;
                glyphComponent.sprite = value;
            }
        }
    }
}
