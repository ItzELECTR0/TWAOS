// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using Rewired;

    [AddComponentMenu("")]
    public class InputFieldInfo : UIElementInfo {
        
        private int _actionElementMapId;
        private AxisRange _axisRange;

        public Rewired.Glyphs.UnityUI.UnityUIControllerElementGlyph glyphOrText { get; set; }
        public int actionId { get; set; }
        public AxisRange axisRange { 
            get {
                return _axisRange;
            }
            set {
                _axisRange = value;
                if (glyphOrText != null) {
                    glyphOrText.axisRange = value;
                }
            }
        }
        public int actionElementMapId {
            get {
                return _actionElementMapId;
            }
            set {
                _actionElementMapId = value;
                if (glyphOrText != null) {
                    glyphOrText.actionElementMap = ReInput.mapping.GetActionElementMap(value);
                }
            }
        }
        public ControllerType controllerType { get; set; }
        public int controllerId { get; set; }
    }
}