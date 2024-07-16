// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {
    using Rewired.Glyphs;

    /// <summary>
    /// A component that allows you to define default Unity UI controller element glyph settings in the inspector.
    /// </summary>
    [UnityEngine.AddComponentMenu("Rewired/Glyphs/Unity UI/Unity UI Default Controller Element Glyph Settings")]
    public class UnityUIDefaultControllerElementGlyphSettings : DefaultControllerElementGlyphSettingsBase {

        protected override void SetDefaultGlyphOrTextPrefab() {
            UnityUIControllerElementGlyphBase.defaultGlyphOrTextPrefab = glyphOrTextPrefab;
        }
    }
}
