// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Glyphs {
    using System;

    [Serializable]
    public abstract class ControllerElementGlyphSelectorOptionsSOBase : UnityEngine.ScriptableObject {

		public abstract ControllerElementGlyphSelectorOptions options { get; }
    }
}
