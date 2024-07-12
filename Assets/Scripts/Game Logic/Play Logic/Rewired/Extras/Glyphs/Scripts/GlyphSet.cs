// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;

    /// <summary>
    /// A list of controller element glyphs.
    /// </summary>
    [Serializable]
    public abstract class GlyphSet : UnityEngine.ScriptableObject {

        [UnityEngine.Tooltip(
            "A list of base keys. Final keys will be composed of base key + glyph key. " +
            "Setting multiple base keys allows one glyph set to apply to multiple controllers, for example."
        )]
        [UnityEngine.SerializeField]
        private string[] _baseKeys;

        /// <summary>
        /// A list of base keys.
        /// Final keys will be composed of base key + glyph key.
        /// Setting multiple base keys allows one glyph set to apply to multiple controllers, for example.
        /// </summary>
        public string[] baseKeys {
            get {
                return _baseKeys;
            }
            set {
                _baseKeys = value;
            }
        }

        /// <summary>
        /// The number of glyphs.
        /// </summary>
        public abstract int glyphCount { get; }
        /// <summary>
        /// Gets the glyph entry at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The glyph entry at the specified index.</returns>
        public abstract EntryBase GetEntry(int index);

        [Serializable]
        public abstract class EntryBase {

            [UnityEngine.SerializeField]
            private string _key;

            public string key {
                get {
                    return _key;
                }
                set {
                    _key = value;
                }
            }

            public abstract object GetValue();
        }

        [Serializable]
        public abstract class EntryBase<TValue> : EntryBase {

            [UnityEngine.SerializeField]
            private TValue _value;

            public TValue value {
                get {
                    return _value;
                }
                set {
                    _value = value;
                }
            }

            public override object GetValue() {
                return _value;
            }
        }
    }
}
