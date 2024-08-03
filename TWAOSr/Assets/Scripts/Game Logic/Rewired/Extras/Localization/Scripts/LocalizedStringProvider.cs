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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Component for handling string localization using a JSON file.
    /// </summary>
    [UnityEngine.AddComponentMenu("Rewired/Localization/Localized String Provider")]
    public class LocalizedStringProvider : LocalizedStringProviderBase {

        [UnityEngine.SerializeField]
#if UNITY_5_PLUS
        [UnityEngine.Tooltip("A JSON file containing localizied string key value pairs.")]
#endif
        private UnityEngine.TextAsset _localizedStringsFile;

        [NonSerialized]
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>();
        [NonSerialized]
        private bool _initialized;

        /// <summary>
        /// The the localized strings dictionary.
        /// </summary>
        protected virtual Dictionary<string, string> dictionary {
            get {
                return _dictionary;
            } 
            set {
                _dictionary = value;
            }
        }

        /// <summary>
        /// A JSON file containing localizied string key value pairs.
        /// When set, localized string data will be loaded from the JSON file and
        /// the new localized strings will be used by Rewired.
        /// </summary>
        public virtual UnityEngine.TextAsset localizedStringsFile {
            get {
                return _localizedStringsFile;
            }
            set {
                _localizedStringsFile = value;
                Reload();
            }
        }

        /// <summary>
        /// Determines if this class has been initialized.
        /// </summary>
        protected override bool initialized {
            get {
                return _initialized;
            }
        }

        /// <summary>
        /// Called when initialized.
        /// </summary>
        /// <returns>True if initialized, false if initialization failed.</returns>
        protected override bool Initialize() {
            _initialized = TryLoadLocalizedStringData();
            return _initialized;
        }

        /// <summary>
        /// Loads localized strings from the file.
        /// Call <see cref="Reload"/> to reload localized strings in Rewired after this function is called.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        protected virtual bool TryLoadLocalizedStringData() {
            _dictionary.Clear();
            if (_localizedStringsFile != null) {
                try {
                    _dictionary = Rewired.Utils.Libraries.TinyJson.JsonParser.FromJson<Dictionary<string, string>>(_localizedStringsFile.text);
                } catch (Exception ex) {
                    UnityEngine.Debug.LogError(ex);
                }
            }
            return _dictionary.Count > 0;
        }

        /// <summary>
        /// Gets a localized string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="result">The localized string.</param>
        /// <returns>True if the input string was localized, false if not.</returns>
        protected override bool TryGetLocalizedString(string key, out string result) {
            if (!_initialized) {
                result = null;
                return false;
            }
            return _dictionary.TryGetValue(key, out result);
        }

#if UNITY_EDITOR
        private Rewired.Utils.Classes.Data.InspectorValue<UnityEngine.TextAsset>_inspector_localizedStringsFile = new Rewired.Utils.Classes.Data.InspectorValue<UnityEngine.TextAsset>();

        protected override void CheckInspectorValues(ref System.Action actions) {
            base.CheckInspectorValues(ref actions);
            if(_inspector_localizedStringsFile.SetIfChanged(_localizedStringsFile)) {
                actions += () => localizedStringsFile = _localizedStringsFile;
            }
        }
#endif

    }
}
