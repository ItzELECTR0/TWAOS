// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Integration.Cinemachine3 {
    using System;

    /// <summary>
    /// Read an input value from Rewired Player.
    /// </summary>
    [Serializable]
    public abstract class RewiredCinemachineInputAxisControllerReaderBase :
        Unity.Cinemachine.IInputAxisReader,
        IRewiredCinemachineInputAxisControllerReader {

        [UnityEngine.Tooltip("Final absolute axis input value is multiplied by this value. (An absolute axis value is a value that remains over multiple frames when the axis is engaged, such as a joystick axis.")]
        [UnityEngine.SerializeField]
        private float _absoluteAxisGain = 1f;

        [UnityEngine.Tooltip("Final relative axis input value is multiplied by this value. (A relative axis value is a delta, such as mouse axis input.)")]
        [UnityEngine.SerializeField]
        private float _relativeAxisGain = 1f;

        /// <summary>
        /// Rewired Action id.
        /// </summary>
        public abstract int GetActionId();

        /// <summary>
        /// Final absolute axis input value is multiplied by this value.
        /// (An absolute axis value is a value that remains over multiple frames when the axis is engaged, such as a joystick axis.
        /// </summary>
        public float absoluteAxisGain {
            get {
                return _absoluteAxisGain;
            }
            set {
                _absoluteAxisGain = value;
            }
        }

        /// <summary>
        /// Final relative axis input value is multiplied by this value.
        /// (A relative axis value is a delta, such as mouse axis input.)
        /// </summary>
        public float relativeAxisGain {
            get {
                return _relativeAxisGain;
            }
            set {
                _relativeAxisGain = value;
            }
        }

        /// <summary>
        /// Gets the input value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="hints">Hints. (Not used)</param>
        /// <returns>Input value.</returns>
        public virtual float GetValue(UnityEngine.Object context, Unity.Cinemachine.IInputAxisOwner.AxisDescriptor.Hints hints) {
            IRewiredCinemachineInputAxisController c = context as IRewiredCinemachineInputAxisController;
            if (c == null) return 0f;
            return GetAxis(c.GetPlayerId(), GetActionId(), _absoluteAxisGain, _relativeAxisGain, c.ignoreTimeScale);
        }

#if UNITY_EDITOR
        public virtual void OnValidate() {
        }
#endif

        /// <summary>
        /// Gets the axis value for the Action from the Player.
        /// </summary>
        /// <param name="playerId">The player id.</param>
        /// <param name="actionId">The action id.</param>
        /// <param name="absoluteGain">A value that is multiplied by the returned input value for absolute axis values.</param>
        /// <param name="relativeGain">A value that is multiplied by the returned input value for relative axis values.</param>
        /// <param name="ignoreTimeScale">Determines if time scale should be ignored.</param>
        /// <returns></returns>
        private static float GetAxis(int playerId, int actionId, float absoluteGain, float relativeGain, bool ignoreTimeScale) {
            if (!ReInput.isReady || actionId < 0) return 0f;
            Player player = ReInput.players.GetPlayer(playerId);
            if (player == null) return 0f;
            float value = player.GetAxis(actionId);
            if (value != 0f) {
                var coordinateMode = player.GetAxisCoordinateMode(actionId);
                switch (coordinateMode) {
                    case AxisCoordinateMode.Relative: {
                            value *= relativeGain;
                            // Cinemachine always multiplies input by Time.deltaTime... Really?? >_<
                            // Reverse this for relative axes because they should never be multiplied by Time.deltaTime.
                            // This is ridiculous.
                            float deltaTime = ignoreTimeScale ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
                            if (deltaTime > 0f) value /= deltaTime;
                        }
                        break;
                    case AxisCoordinateMode.Absolute:
                        value *= absoluteGain;
                        break;
                }
            }
            return value;
        }
    }
}
