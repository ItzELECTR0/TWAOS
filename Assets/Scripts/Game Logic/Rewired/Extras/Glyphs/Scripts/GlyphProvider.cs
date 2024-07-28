// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using Rewired;
    using Rewired.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages glyphs.
    /// </summary>
    public class GlyphProvider : UnityEngine.MonoBehaviour, IGlyphProvider {

        [UnityEngine.SerializeField]
        [UnityEngine.Tooltip("Determines if glyphs should be fetched immediately in bulk when available. If false, glyphs will be fetched when queried.")]
        private bool _prefetch;
        [UnityEngine.SerializeField]
        [UnityEngine.Tooltip("A list of glyph set collections. At least one collection must be assigned.")]
        private List<GlyphSetCollection> _glyphSetCollections;

        [NonSerialized]
        private readonly Dictionary<string, object> _glyphs = new Dictionary<string, object>();
        [NonSerialized]
        private bool _initialized;

        /// <summary>
        /// Determines if glyphs should be fetched immediately in bulk when available.
        /// If false, glyphs will be fetched when queried.
        /// </summary>
        public bool prefetch {
            get {
                return _prefetch;
            }
            set {
                _prefetch = value;
                if (isActiveAndEnabled && ReInput.isReady && ReInput.glyphs.glyphProvider == (IGlyphProvider)this) {
                    ReInput.glyphs.prefetch = value;
                }
            }
        }

        /// <summary>
        /// The list of glyph set collections.
        /// You should not make changes to the list directly, but instead set the list.
        /// Otherwise, the changes will not be reflected in Rewired for glyphs that
        /// have already been cached. If you do modify the list directly, call <see cref="Reload"/> after making changes.
        /// When setting the array, <see cref="Reload"/> will be called automatically.
        /// </summary>
        public List<GlyphSetCollection> glyphSetCollections {
            get {
                return _glyphSetCollections;
            }
            set {
                _glyphSetCollections = value;
                 Reload();
            }
        }

        /// <summary>
        /// The cached glyphs.
        /// </summary>
        protected Dictionary<string, object> glyphs {
            get {
                return _glyphs;
            }
        }

        protected virtual void OnEnable() {
            if (!_initialized) Initialize(); // only init if needed to avoid reloading everything if re-enabled
            TrySetGlyphProvider();
        }

        protected virtual void OnDisable() {
            if (ReInput.isReady && ReInput.glyphs.glyphProvider == (IGlyphProvider)this) {
                ReInput.glyphs.glyphProvider = null;
            }
            ReInput.InitializedEvent -= TrySetGlyphProvider;
        }

        protected virtual void Update() {
#if UNITY_EDITOR
            HandleInspectorValueChanges();
#endif
        }

        /// <summary>
        /// Sets this as the Glyph Provider in Rewired.
        /// </summary>
        protected virtual void TrySetGlyphProvider() {
            // Workaround to handle OnEnable script execution order bugs in various versions of Unity.
            // When recompiling in the editor in Play mode, OnEnable can be called on this script
            // before it is called on Rewired, so Rewired is not initialized by the time this runs.
            // This also has the side benefit of allowing the user to instantiate this object
            // before Rewired for whatever reason, and it will set itself as the provider
            // when Rewired initalizes.
            // This will also set the glyph provider again after calling ReInput.Reset or changing
            // some configuration setting that causes Rewired to reset.
            ReInput.InitializedEvent -= TrySetGlyphProvider;
            ReInput.InitializedEvent += TrySetGlyphProvider;
            if (!ReInput.isReady) return;
            if (!Rewired.Utils.UnityTools.IsNullOrDestroyed(ReInput.glyphs.glyphProvider)) {
                UnityEngine.Debug.LogWarning("Rewired: A glyph provider is already set. Only one glyph provider can exist at a time.");
                return;
            }
            ReInput.glyphs.glyphProvider = this;
            ReInput.glyphs.prefetch = _prefetch;
        }

        /// <summary>
        /// Called when initialized.
        /// Will be initialized on enable and when localized strings are set.
        /// </summary>
        /// <returns>True if initialized, false if initialization failed.</returns>
        protected virtual bool Initialize() {
            _initialized = false;
            if (_glyphSetCollections == null) return false;

            // Create a dictionary of all glyhps in all collections
            
            _glyphs.Clear();

            const char keyPathSeparator = '/';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string key;
            GlyphSetCollection collection;
            GlyphSet.EntryBase entry;
            int baseKeysCount;
            int entryCount;
            int i, j, k;

            for (i = 0; i < _glyphSetCollections.Count; i++) {
                collection = _glyphSetCollections[i];
                if (collection == null) continue;
                foreach(var set in collection.IterateSetsRecursively()) {
                    if (set == null) continue;
                    if (set.baseKeys != null) {
                        baseKeysCount = set.baseKeys.Length;
                        for (j = 0; j < baseKeysCount; j++) {
                            if (string.IsNullOrEmpty(set.baseKeys[j])) continue;
                            entryCount = set.glyphCount;
                            for (k = 0; k < entryCount; k++) {
                                entry = set.GetEntry(k);
                                if (entry == null) continue;
                                if (string.IsNullOrEmpty(entry.key)) continue;
                                if (entry.GetValue() == null) continue;
                                // Concatenate base key and glyph key
                                sb.Append(set.baseKeys[j]);
                                sb.Append(keyPathSeparator);
                                sb.Append(entry.key);
                                key = sb.ToString();
                                sb.Length = 0;
                                if (_glyphs.ContainsKey(key)) {
                                    UnityEngine.Debug.LogError("Rewired: Duplicate glyph key found: " + key);
                                    continue;
                                }
                                _glyphs.Add(key, entry.GetValue());
                            }
                        }
                    }
                }
            }

            _initialized = true;

            return true;
        }

        /// <summary>
        /// Clears glyph cache so all glyhps are reloaded.
        /// Subclasses should call this after making changes to the glyph collection.
        /// </summary>
        public void Reload() {
            Initialize();
            if (isActiveAndEnabled && ReInput.isReady && ReInput.glyphs.glyphProvider == (IGlyphProvider)this) {
                ReInput.glyphs.Reload();
            }
        }

        /// <summary>
        /// Try to get the glyph for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="result">The localized string.</param>
        /// <returns>True if the glyph was found, false if not.</returns>
        bool IGlyphProvider.TryGetGlyph(string key, out object result) {
            if (!_initialized) {
                result = null;
                return false;
            }
            return _glyphs.TryGetValue(key, out result);
        }

#if UNITY_EDITOR
        [NonSerialized]
        private System.Action _inspectorActions;
        private Rewired.Utils.Classes.Data.InspectorValue<bool>_inspector_prefetch = new Rewired.Utils.Classes.Data.InspectorValue<bool>();
        private Rewired.Utils.Classes.Data.InspectorListValue<GlyphSetCollection> _inspector_glyphSetCollections = new Rewired.Utils.Classes.Data.InspectorListValue<GlyphSetCollection>();

        protected virtual void OnValidate() {
            _inspectorActions = null; // prevent buffering of actions in edit mode
            CheckInspectorValues(ref _inspectorActions);
        }

        protected virtual void CheckInspectorValues(ref System.Action actions) {
            if(_inspector_prefetch.SetIfChanged(_prefetch)) {
                actions += () => prefetch = _prefetch;
            }
            if (_inspector_glyphSetCollections.SetIfChanged(_glyphSetCollections)) {
                actions += () => glyphSetCollections = _glyphSetCollections;
            }
        }

        protected virtual void HandleInspectorValueChanges() {
            System.Action actions = _inspectorActions;
            _inspectorActions = null;
            if (actions != null) actions();
        }
#endif
    }
}
