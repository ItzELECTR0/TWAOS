// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;
    using Rewired;

    /// <summary>
    /// Base class for displaying glyphs / text for a specific Action Element Map or Controller Element Identifier.
    /// This component cannot be used alone. It requires a script to set either
    /// <see cref="ControllerElementGlyph.actionElementMap"/> or
    /// the <see cref="ControllerElementGlyph.controllerElementIdentifier"/> and
    /// <see cref="ControllerElementGlyph.axisRange"/>. This class is mainly useful if you need to display a
    /// glyph for a specific controller element, such as when showing a list of glyphs for controller elements
    /// in a controller or when showing glyphs for controller elements currently contributing to an Action value.
    /// </summary>
    public abstract class ControllerElementGlyph : ControllerElementGlyphBase {

        [NonSerialized]
        private ActionElementMap _actionElementMap;
        [NonSerialized]
        private ControllerElementIdentifier _controllerElementIdentifier;
        [NonSerialized]
        private AxisRange _axisRange;

        /// <summary>
        /// The Action Element Map for which glyphs / text will be displayed.
        /// If Action Element Map is set, it takes precedence over Controller Element Identifier.
        /// </summary>
        public ActionElementMap actionElementMap {
            get {
                return _actionElementMap;
            }
            set {
                _actionElementMap = value;
            }
        }

        /// <summary>
        /// The Controller Element Identifier for which glyphs / text will be displayed.
        /// You must also set <see cref="axisRange"/>.
        /// If <see cref="actionElementMap"/> set, it takes precedence over Controller Element Identifier and this will be ignored.
        /// </summary>
        public ControllerElementIdentifier controllerElementIdentifier {
            get {
                return _controllerElementIdentifier;
            }
            set {
                _controllerElementIdentifier = value;
            }
        }

        /// <summary>
        /// The axis range of the Controller Element Identifier for which glyphs / text will be displayed.
        /// You must also set <see cref="controllerElementIdentifier"/>.
        /// </summary>
        public AxisRange axisRange {
            get {
                return _axisRange;
            }
            set {
                _axisRange = value;
            }
        }

        protected override void Update() {
            base.Update();
            if (!ReInput.isReady) return;
            if (_actionElementMap == null && controllerElementIdentifier == null) {
                Hide();
                return;
            }
            if (actionElementMap != null) ShowGlyphsOrText(_actionElementMap);
            else if (controllerElementIdentifier != null) ShowGlyphsOrText(_controllerElementIdentifier, axisRange);
            EvaluateObjectVisibility();
        }
    }
}
