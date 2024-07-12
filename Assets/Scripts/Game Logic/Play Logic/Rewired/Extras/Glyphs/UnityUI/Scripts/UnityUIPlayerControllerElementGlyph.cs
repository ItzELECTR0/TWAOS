// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {
    using Rewired;

    /// <summary>
    /// Displays the controller element glyph(s) for a particular Action for a particular Player.
    /// </summary>
    [UnityEngine.AddComponentMenu("Rewired/Glyphs/Unity UI/Unity UI Player Controller Element Glyph")]
    public class UnityUIPlayerControllerElementGlyph : UnityUIPlayerControllerElementGlyphBase {

        [UnityEngine.Tooltip("The Player id.")]
        [UnityEngine.SerializeField]
        private int _playerId;

        [UnityEngine.Tooltip("The Action name.")]
        [UnityEngine.SerializeField]
        private string _actionName;

        [System.NonSerialized]
        private int _actionId = -1;
        [System.NonSerialized]
        private bool _actionIdCached = false;

        /// <summary>
        /// The Player id.
        /// </summary>
        public override int playerId { get { return _playerId; } set { _playerId = value; } }

        /// <summary>
        /// The Action id.
        /// </summary>
        public override int actionId {
            get {
                if (!_actionIdCached) CacheActionId();
                return _actionId;
            }
            set {
                if (!ReInput.isReady) return;
                var action = ReInput.mapping.GetAction(value);
                if (action == null) {
                    UnityEngine.Debug.LogError("Invalid Action id: " + value);
                    return;
                }
                _actionName = action.name;
                CacheActionId();
            }
        }

        /// <summary>
        /// The Action name.
        /// </summary>
        public string actionName {
            get {
                return _actionName;
            }
            set {
                _actionName = value;
                CacheActionId();
            }
        }

        private void CacheActionId() {
            if (!ReInput.isReady) return;
            var action = ReInput.mapping.GetAction(_actionName);
            _actionId = action != null ? action.id : -1;
            _actionIdCached = true;
        }

#if UNITY_EDITOR

        private Rewired.Utils.Classes.Data.InspectorValue<string> _inspector_actionName = new Rewired.Utils.Classes.Data.InspectorValue<string>();

        protected override void CheckInspectorValues(ref System.Action actions) {
            base.CheckInspectorValues(ref actions);
            if (_inspector_actionName.SetIfChanged(_actionName)) {
                actions += () => actionName = _actionName;
            }
        }
#endif
    }
}
