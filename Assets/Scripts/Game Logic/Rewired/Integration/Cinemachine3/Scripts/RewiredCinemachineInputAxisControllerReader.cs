// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Integration.Cinemachine3 {
    using System;

    /// <summary>
    /// Read an input value from Rewired Player.
    /// </summary>
    [Serializable]
    public sealed class RewiredCinemachineInputAxisControllerReader : RewiredCinemachineInputAxisControllerReaderBase {

        [UnityEngine.Tooltip("Rewired Action name.")]
        [UnityEngine.SerializeField]
        private string _actionName;

        [NonSerialized]
        private bool _cachedActionId;
        [NonSerialized]
        private int _actionId;

        /// <summary>
        /// Rewired Action name.
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

        public override int GetActionId() {
            if (!_cachedActionId) CacheActionId();
            return _actionId;
        }

        private void CacheActionId() {
            if (!ReInput.isReady) return;
            _actionId = ReInput.mapping.GetActionId(_actionName);
            _cachedActionId = true;
        }

#if UNITY_EDITOR
        public override void OnValidate() {
            base.OnValidate();
            CacheActionId();
        }
#endif
    }

}
