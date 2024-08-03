// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SpriteGlyphSet : Rewired.Glyphs.GlyphSet {

        [UnityEngine.Tooltip("The list of glyphs.")]
        [UnityEngine.SerializeField]
        private List<Entry> _glyphs;

        /// <summary>
        /// The list of glyphs.
        /// </summary>
        public List<Entry> glyphs {
            get {
                return _glyphs;
            }
            set {
                _glyphs = value;
            }
        }

        /// <summary>
        /// The number of glyphs.
        /// </summary>
        public override int glyphCount {
            get {
                if (_glyphs == null) return 0;
                return _glyphs.Count;
            }
        }

        /// <summary>
        /// Gets the glyph entry at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The glyph entry at the specified index.</returns>
        public override EntryBase GetEntry(int index) {
            if (_glyphs == null) return null;
            if ((uint)index >= (uint)_glyphs.Count) throw new ArgumentOutOfRangeException("index");
            return _glyphs[index];
        }

        [Serializable]
        public class Entry : EntryBase<UnityEngine.Sprite> {
        }
    }
}
