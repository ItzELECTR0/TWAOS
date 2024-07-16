// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;

    /// <summary>
    /// Base class of a component that allows you to define default controller element glyph settings in the inspector.
    /// </summary>
    public abstract class DefaultControllerElementGlyphSettingsBase : UnityEngine.MonoBehaviour {

        [UnityEngine.Tooltip("The Controller element glyph options.")]
        [UnityEngine.SerializeField]
        private ControllerElementGlyphSelectorOptions _options;

        [UnityEngine.Tooltip("The prefab used for each glyph or text object.")]
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject _glyphOrTextPrefab;

        /// <summary>
        /// The Controller element glyph options.
        /// </summary>
        public ControllerElementGlyphSelectorOptions options {
            get {
                return _options;
            }
            set {
                _options = value;
                SetDefaults();
            }
        }

        /// <summary>
        /// The prefab used for each glyph or text object.
        /// </summary>
        public UnityEngine.GameObject glyphOrTextPrefab {
            get {
                return _glyphOrTextPrefab;
            }
            set {
                _glyphOrTextPrefab = value;
                SetDefaults();
            }
        }

        protected virtual void OnEnable() {
            SetDefaults();
        }

        protected virtual void OnDisable() {
        }

        protected virtual void SetDefaults() {
            SetDefaultOptions();
            SetDefaultGlyphOrTextPrefab();
        }

        protected virtual void SetDefaultOptions() {
            ControllerElementGlyphSelectorOptions.defaultOptions = options;
        }

        protected abstract void SetDefaultGlyphOrTextPrefab();

#if UNITY_EDITOR

        [NonSerialized]
        private bool _inspectorValueChanged;

        protected virtual void OnValidate() {
            _inspectorValueChanged = true;
        }

        protected virtual void Update() {
            if (_inspectorValueChanged) {
                _inspectorValueChanged = false;
                SetDefaults();
            }
        }

#endif

    }
}
