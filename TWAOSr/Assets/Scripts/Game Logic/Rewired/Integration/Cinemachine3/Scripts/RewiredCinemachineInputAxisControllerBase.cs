// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Integration.Cinemachine3 {

    /// <summary>
    /// Base class of a component to a Cinemachine Camera to control it with Rewired.
    /// </summary>
    [UnityEngine.ExecuteAlways]
    [Unity.Cinemachine.SaveDuringPlay]
    public abstract class RewiredCinemachineInputAxisControllerBase<T> :
        Unity.Cinemachine.InputAxisControllerBase<T>,
        IRewiredCinemachineInputAxisController
        where T : Unity.Cinemachine.IInputAxisReader, new() {

        bool IRewiredCinemachineInputAxisController.ignoreTimeScale { get { return IgnoreTimeScale; } }

        protected override void Reset() {
            base.Reset();
        }

        protected virtual void Update() {
            if (ReInput.isReady) {
                UpdateControllers();
            }
        }

        /// <summary>
        /// Rewired Player id.
        /// </summary>
        public abstract int GetPlayerId();

        /// <summary>
        /// Initializes default values when the component is added or is reset in the inspector.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="controller">The controller</param>
        protected override void InitializeControllerDefaultsForAxis(in Unity.Cinemachine.IInputAxisOwner.AxisDescriptor axis, Controller controller) {
            if (SetControlDefaults != null) {
                SetControlDefaults(axis, controller);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            if (Controllers != null) {
                foreach (var c in Controllers) {
                    RewiredCinemachineInputAxisControllerReaderBase tC = c.Input as RewiredCinemachineInputAxisControllerReaderBase;
                    if (tC == null) continue;
                    tC.OnValidate();
                }
            }
        }
#endif

        /// <summary>
        /// This function exists so the inspector can set default values on reset.
        /// Do not call this function.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="controller">The controller.</param>
        public delegate void SetControlDefaultsForAxis(in Unity.Cinemachine.IInputAxisOwner.AxisDescriptor axis, Controller controller);
        /// <summary>
        /// This property exists so the inspector can set default values on reset.
        /// Do not set this property.
        /// </summary>
        public static SetControlDefaultsForAxis SetControlDefaults { get; set; }
    }
}
