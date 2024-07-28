// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2020_PLUS
#endif

#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#pragma warning disable 0649

namespace Rewired.Localization {
    using Rewired;
    using Rewired.Interfaces;

    /// <summary>
    /// Base class for management of localized strings.
    /// Create a subclass of this class to manage localized strings.
    /// </summary>
    public abstract class LocalizedStringProviderBase : UnityEngine.MonoBehaviour, ILocalizedStringProvider {

        [UnityEngine.SerializeField]
#if UNITY_5_PLUS
        [UnityEngine.Tooltip("Determines if localized strings should be fetched immediately in bulk when available. If false, strings will be fetched when queried.")]
#endif
        private bool _prefetch;

        /// <summary>
        /// Determines if localized strings should be fetched immediately in bulk when available.
        /// If false, strings will be fetched when queried.
        /// </summary>
        public virtual bool prefetch {
            get {
                return _prefetch;
            }
            set {
                _prefetch = value;
                if (gameObject.activeInHierarchy && enabled && ReInput.isReady &&
                    ReInput.localization.localizedStringProvider == (ILocalizedStringProvider)this) {
                    ReInput.localization.prefetch = value;
                }
            }
        }

        /// <summary>
        /// Determines if this class has been initialized.
        /// </summary>
        protected abstract bool initialized { get; }

        protected virtual void OnEnable() {
            if (!initialized) Initialize(); // only init if needed to avoid reloading everything if re-enabled
            TrySetLocalizedStringProvider();
        }

        protected virtual void OnDisable() {
            if (ReInput.isReady && ReInput.localization.localizedStringProvider == (ILocalizedStringProvider)this) {
                ReInput.localization.localizedStringProvider = null;
            }
            ReInput.InitializedEvent -= TrySetLocalizedStringProvider;
        }

        protected virtual void Update() {
#if UNITY_EDITOR
            HandleInspectorValueChanges();
#endif
        }

        /// <summary>
        /// Sets this as the Localized String in Rewired.
        /// </summary>
        protected virtual void TrySetLocalizedStringProvider() {
            // Workaround to handle OnEnable script execution order bugs in various versions of Unity.
            // When recompiling in the editor in Play mode, OnEnable can be called on this script
            // before it is called on Rewired, so Rewired is not initialized by the time this runs.
            // This also has the side benefit of allowing the user to instantiate this object
            // before Rewired for whatever reason, and it will set itself as the provider
            // when Rewired initalizes.
            // This will also set the glyph provider again after calling ReInput.Reset or changing
            // some configuration setting that causes Rewired to reset.
            ReInput.InitializedEvent -= TrySetLocalizedStringProvider;
            ReInput.InitializedEvent += TrySetLocalizedStringProvider;
            if (!ReInput.isReady) return;
            if (!Rewired.Utils.UnityTools.IsNullOrDestroyed(ReInput.localization.localizedStringProvider)) {
                UnityEngine.Debug.LogWarning("A localized string provider is already set. Only one localized string provider can exist at a time.");
                return;
            }
            ReInput.localization.localizedStringProvider = this;
            ReInput.localization.prefetch = _prefetch;
        }

        /// <summary>
        /// Called when initialized.
        /// </summary>
        /// <returns>True if initialized, false if initialization failed.</returns>
        protected abstract bool Initialize();

        /// <summary>
        /// Clears localized string cache so all localized strings are reloaded.
        /// Subclasses should call this after making changes to the localized string collection.
        /// </summary>
        public virtual void Reload() {
            Initialize();
            if (gameObject.activeInHierarchy && enabled && ReInput.isReady &&
                ReInput.localization.localizedStringProvider == (ILocalizedStringProvider)this) {
                ReInput.localization.Reload();
            }
        }

        /// <summary>
        /// Gets a localized string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="result">The localized string.</param>
        /// <returns>True if the input string was localized, false if not.</returns>
        protected abstract bool TryGetLocalizedString(string key, out string result);

        bool ILocalizedStringProvider.TryGetLocalizedString(string key, out string result) {
            return TryGetLocalizedString(key, out result);
        }

#if UNITY_EDITOR
        [System.NonSerialized]
        private System.Action _inspectorActions;
        private Rewired.Utils.Classes.Data.InspectorValue<bool>_inspector_prefetch = new Rewired.Utils.Classes.Data.InspectorValue<bool>();

        protected virtual void OnValidate() {
            _inspectorActions = null; // prevent buffering of actions in edit mode
            CheckInspectorValues(ref _inspectorActions);
        }

        protected virtual void CheckInspectorValues(ref System.Action actions) {
            if(_inspector_prefetch.SetIfChanged(_prefetch)) {
                actions += () => prefetch = _prefetch;
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
