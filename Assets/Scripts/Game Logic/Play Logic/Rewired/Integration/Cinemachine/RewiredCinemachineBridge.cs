// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Integration.Cinemachine {
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Changes input source for Cinemachine to Rewired.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class RewiredCinemachineBridge : MonoBehaviour {

        [System.Serializable]
        private class PlayerMapping {
            [SerializeField]
            public int _playerId;
            [SerializeField]
            public List<ActionMapping> _actionMappings;
        }

        [System.Serializable]
        private class ActionMapping {
            [SerializeField]
            public string _cinemachineAxis;
            [SerializeField]
            public string _rewiredActionName;
            [SerializeField]
            public int _rewiredActionId = -1;
        }

        private class PlayerActionMapping {
            public int playerId;
            public int actionId;
        }

        private const float defaultabsoluteAxisSensitivity = 30f;

        [Tooltip("(Optional) Link the Rewired Input Manager here for easier access to Action ids, etc.")]
        [SerializeField]
        private InputManager_Base _rewiredInputManager;

        [Tooltip("The absolute sensitivity multipler. This is only applied to absolute axis sources (joystick axes, keyboard keys, etc.).")]
        [SerializeField]
        [Rewired.Utils.Attributes.FieldRange(0f, float.MaxValue)]
        private float _absoluteAxisSensitivity = defaultabsoluteAxisSensitivity;

        [Tooltip("If enabled, input values from absolute axis sources will be scaled based on the screen resolution. This makes joystick axes behave more consistently with mouse axes at different screen resolutions.")]
        [SerializeField]
        private bool _scaleAbsoluteAxesToScreen = false;

        [Tooltip("If enabled, the Cinemachine Bridge runs in edit mode. The Rewired Input Manager must also be set to run in Edit Mode for this to have any effect.")]
        [SerializeField]
        private bool _runInEditMode = false;

        [Tooltip("Cinemachine to Rewired Player Action mappings. Use this to map Cinemachine input Axes to Rewired Actions on specific Players.")]
        [SerializeField]
        private List<PlayerMapping> _playerMappings = new List<PlayerMapping>() {
            new PlayerMapping() { _playerId = 0, _actionMappings = new List<ActionMapping>() {
                new ActionMapping() { _cinemachineAxis = "Mouse X", _rewiredActionName = "Mouse X", _rewiredActionId = -1 },
                new ActionMapping() { _cinemachineAxis = "Mouse Y", _rewiredActionName = "Mouse Y", _rewiredActionId = -1 },
            }}
        };

        /// <summary>
        /// (Optional) Link the Rewired Input Manager here for easier access to Action ids, etc.
        /// </summary>
        public InputManager_Base rewiredInputManager {
            get { return _rewiredInputManager; }
            set { _rewiredInputManager = value; }
        }

        /// <summary>
        /// The absolute sensitivity multipler. This is only applied to absolute axis sources (joystick axes, keyboard keys, etc.).
        /// </summary>
        public float absoluteAxisSensitivity {
            get { return _absoluteAxisSensitivity; }
            set {
                if(value < 0f) value = 0f;
                _absoluteAxisSensitivity = value;
            }
        }

        /// <summary>
        /// If enabled, input values from absolute axis sources will be scaled based on the screen resolution.
        /// This makes joystick axes behave more consistently with mouse axes at different screen resolutions.
        /// </summary>
        public bool scaleAbsoluteAxesToScreen {
            get { return _scaleAbsoluteAxesToScreen; }
            set {
                _scaleAbsoluteAxesToScreen = value;
            }
        }

        /// <summary>
        /// If enabled, the Cinemachine Bridge runs in edit mode. The Rewired Input Manager must also be set to run in Edit Mode for this to have any effect.
        /// </summary>
#if UNITY_EDITOR
        new public bool runInEditMode
#else
        public bool runInEditMode
#endif
        {
            get { return _runInEditMode; }
            set {
                if(_runInEditMode == value) return;
                _runInEditMode = value;
                if(!Application.isPlaying) {
                    if(value) Initialize();
                    else Deinitialize();
                } 
            }
        }

        private readonly Dictionary<string, PlayerActionMapping> _mappings = new Dictionary<string, PlayerActionMapping>();
        [System.NonSerialized]
        private bool _initialized;

        private void Awake() {
            if(!Application.isPlaying && !_runInEditMode) return;
            Initialize();
        }

        private void OnDestroy() {
            Deinitialize();
        }

        private void Initialize() {
            Deinitialize();
            if(!ReInput.isReady) {
                Debug.LogError("You must have an enabled Rewired Input Manager in the scene to use the Cinemachine bridge.");
                return;
            }

            if(_instance != null) {
                Debug.LogError("You cannot have multiple Rewired Cinemachine Bridges enabled in the scene.");
                return;
            }
            _instance = this;

            if(_rewiredInputManager == null) _rewiredInputManager = GetComponent<InputManager_Base>();

            foreach(var m in _playerMappings) {
                if(ReInput.players.GetPlayer(m._playerId) == null) {
                    Debug.LogError("No Player exists for id " + m._playerId + ".");
                    continue;
                }
                foreach(var a in m._actionMappings) {
                    if(string.IsNullOrEmpty(a._cinemachineAxis)) continue;

                    InputAction action;
                    if(_rewiredInputManager != null) {
                        if(a._rewiredActionId < 0) continue;
                        action = ReInput.mapping.GetAction(a._rewiredActionId);
                    } else {
                        if(string.IsNullOrEmpty(a._rewiredActionName)) continue;
                        action = ReInput.mapping.GetAction(a._rewiredActionName);
                    }
                    if(action == null) {
                        Debug.LogWarning("The Action " + (_rewiredInputManager != null ? "Id " + a._rewiredActionId : "\"" + a._rewiredActionName + "\"") + " does not exist in the Rewired Input Manager.");
                        continue;
                    }
                    if(_mappings.ContainsKey(a._cinemachineAxis)) {
                        Debug.LogError("Duplicate Unity Axis found \"" + a._cinemachineAxis + "\". This is not allowed. All Unity Axes must be unique.");
                        continue;
                    }
                    _mappings.Add(a._cinemachineAxis, new PlayerActionMapping() { playerId = m._playerId, actionId = action.id });
                }
            }

            global::Cinemachine.CinemachineCore.GetInputAxis = GetAxis;

            _initialized = true;
        }

        private void Deinitialize() {
            if(_instance == this) _instance = null;
            if(_mappings != null) _mappings.Clear();
            _initialized = false;
        }

#if UNITY_EDITOR

        private void OnValidate() {
            if(!Application.isPlaying) {
                if(_runInEditMode) {
                    Initialize();
                } else {
                    Deinitialize();
                }
            }
        }

#endif

        private static RewiredCinemachineBridge _instance;

        private static float GetAxis(string name) {
            if(!ReInput.isReady || _instance == null || !_instance._initialized) return 0f;
            PlayerActionMapping mapping;
            if(!_instance._mappings.TryGetValue(name, out mapping)) {
                Debug.LogWarning("The Action \"" + name + "\" has not been mapped in the Rewired Cinemachine Bridge inspector.");
                return 0f;
            }
            Player player = ReInput.players.GetPlayer(mapping.playerId);
            if(player == null) return 0f;
            float value = player.GetAxis(mapping.actionId);
            if(value != 0f && player.GetAxisCoordinateMode(mapping.actionId) == AxisCoordinateMode.Absolute) {
                value *= _instance._absoluteAxisSensitivity * Time.unscaledDeltaTime;
                if(_instance._scaleAbsoluteAxesToScreen) value *= Screen.currentResolution.width / 1920f;
            }
            return value;
        }

        static RewiredCinemachineBridge() {
            // Force override in the static contstructor to prevent
            // Unity from throwing missing input Axis exceptions in edit mode
            // if the user is using axis names that don't exist in the Unity input manager.
            global::Cinemachine.CinemachineCore.GetInputAxis = GetAxis;
        }
    }
}