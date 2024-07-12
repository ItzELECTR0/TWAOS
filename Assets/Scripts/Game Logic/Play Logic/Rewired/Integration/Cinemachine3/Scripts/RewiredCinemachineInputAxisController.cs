// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Integration.Cinemachine3 {

    /// <summary>
    /// Component to a Cinemachine Camera to control it with Rewired.
    /// </summary>
    [UnityEngine.ExecuteAlways]
    [Unity.Cinemachine.SaveDuringPlay]
    [UnityEngine.AddComponentMenu("Rewired/Integrations/Cinemachine 3/Rewired Cinemachine Input Axis Controller")]
    public sealed class RewiredCinemachineInputAxisController : RewiredCinemachineInputAxisControllerBase<RewiredCinemachineInputAxisControllerReader> {

        [UnityEngine.Tooltip("Rewired Player id. ")]
        [UnityEngine.SerializeField]
        private int _playerId = 0;

        /// <summary>
        /// Rewired Player id.
        /// </summary>
        public int playerId {
            get {
                return _playerId;
            }
            set {
                _playerId = value;
            }
        }

        protected override void Reset() {
            base.Reset();
            _playerId = 0;
        }

        public override int GetPlayerId() {
            return _playerId;
        }
    }
}
