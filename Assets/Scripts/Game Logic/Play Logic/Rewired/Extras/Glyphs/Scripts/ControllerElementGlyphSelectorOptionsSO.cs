// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;

    [Serializable]
    public class ControllerElementGlyphSelectorOptionsSO : ControllerElementGlyphSelectorOptionsSOBase {

        [UnityEngine.SerializeField]
        private ControllerElementGlyphSelectorOptions _options;

        public override ControllerElementGlyphSelectorOptions options {
            get {
                return _options;
            }
        }
    }
}
