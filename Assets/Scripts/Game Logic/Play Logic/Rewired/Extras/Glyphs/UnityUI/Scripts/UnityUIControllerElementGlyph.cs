// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {

    /// <summary>
    /// Displays glyphs / text for a specific Action Element Map or Controller Element Identifier using Unity UI.
    /// This component cannot be used alone. It requires a script to set either
    /// <see cref="ControllerElementGlyph.actionElementMap"/> or
    /// the <see cref="ControllerElementGlyph.controllerElementIdentifier"/> and
    /// <see cref="ControllerElementGlyph.axisRange"/>. This class is mainly useful if you need to display a
    /// glyph for a specific controller element, such as when showing a list of glyphs for controller elements
    /// in a controller or when showing glyphs for controller elements currently contributing to an Action value.
    /// If you want to display a glyph for a controller element bound to an Action for a Player, use
    /// <see cref="UnityUIPlayerControllerElementGlyph"/> instead.
    /// </summary>
    [UnityEngine.AddComponentMenu("Rewired/Glyphs/Unity UI/Unity UI Controller Element Glyph")]
    public class UnityUIControllerElementGlyph : ControllerElementGlyph {

        /// <summary>
        /// Gets the default glyph or text prefab.
        /// </summary>
        /// <returns>The default glyph or text prefab.</returns>
        protected override UnityEngine.GameObject GetDefaultGlyphOrTextPrefab() {
            return UnityUIControllerElementGlyphBase.defaultGlyphOrTextPrefab;
        }
    }
}
