// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2018 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2018_PLUS
#endif

#if UNITY_2018_PLUS
#define REWIRED_SUPPORTS_TMPRO
#endif

#if REWIRED_SUPPORTS_TMPRO

namespace Rewired.Glyphs.UnityUI {

    public class UnityUIGlyphOrTextTMPro : GlyphOrTextBase<UnityEngine.UI.Image, UnityEngine.Sprite, TMPro.TMP_Text> {

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

#endif
