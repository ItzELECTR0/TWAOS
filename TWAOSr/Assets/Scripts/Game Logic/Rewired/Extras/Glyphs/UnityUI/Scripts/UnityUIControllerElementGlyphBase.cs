// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2022_2 || UNITY_2022_3 || UNITY_2022_4 || UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2022_2_PLUS
#endif

#if UNITY_2018 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2022_2_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif

#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif

#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif

#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif

#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif

#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif

#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif

#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif

#if UNITY_2022_2_PLUS
#define REWIRED_DEFAULT_FONT_IS_LEGACYRUNTIME
#endif

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {
	using Rewired.Glyphs;

    /// <summary>
    /// Base class for a Unity UI Controller Element Glyph component.
    /// </summary>
    public abstract class UnityUIControllerElementGlyphBase : ControllerElementGlyphBase {

        /// <summary>
        /// Gets the default glyph or text prefab.
        /// </summary>
        /// <returns>The default glyph or text prefab.</returns>
        protected override UnityEngine.GameObject GetDefaultGlyphOrTextPrefab() {
            return defaultGlyphOrTextPrefab;
        }

        // Static

        private static UnityEngine.GameObject s_defaultGlyphOrTextPrefab;
        /// <summary>
        /// The default prefab that will be used to instantiate glyph or text objects.
        /// Set this to override the default prefab.
        /// </summary>
        public static UnityEngine.GameObject defaultGlyphOrTextPrefab {
            get {
                return s_defaultGlyphOrTextPrefab != null ? s_defaultGlyphOrTextPrefab : s_defaultGlyphOrTextPrefab = CreateDefaultGlyphOrTextPrefab();
            }
            set {
                s_defaultGlyphOrTextPrefab = value;
            }
        }
        
        private static System.Func<UnityEngine.GameObject> s_defaultGlyphOrTextPrefabProvider;
        
        /// <summary>
        /// The  provider of the default glyph or text prefab.
        /// This is for internal use only. Do not set this value.
        /// </summary>
        public static System.Func<UnityEngine.GameObject> defaultGlyphOrTextPrefabProvider {
          get {
              return s_defaultGlyphOrTextPrefabProvider;
          }
          set {
              s_defaultGlyphOrTextPrefabProvider = value;
          }
        }

        private static UnityEngine.GameObject CreateDefaultGlyphOrTextPrefab() {

            // Get default prefab from provider if available
            if (s_defaultGlyphOrTextPrefabProvider != null) {
                return s_defaultGlyphOrTextPrefabProvider();
            }

            // Warning: Do not make any changes to this without also making equivalent
            // changes to TMPro version if they are meant to be in sync.

            UnityEngine.GameObject prefab = new UnityEngine.GameObject("Glyph or text prefab");
            prefab.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(prefab);

            UnityUIGlyphOrText glyphOrText = prefab.AddComponent<UnityUIGlyphOrText>();

            // Add layout group
            UnityEngine.UI.VerticalLayoutGroup layoutGroup = prefab.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
#if UNITY_2017_PLUS
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
#endif
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childForceExpandWidth = true;

            {
                UnityEngine.GameObject go = new UnityEngine.GameObject("Glyph");
                go.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
                go.transform.SetParent(prefab.transform, false);
                UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
                image.preserveAspect = true;
                glyphOrText.glyphComponent = image;
            }

            {
                const int minFontSize = 10;
                const int maxFontSize = 32;

                UnityEngine.GameObject go = new UnityEngine.GameObject("Text");
                go.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
                go.transform.SetParent(prefab.transform, false);
                var text = go.AddComponent<UnityEngine.UI.Text>();
                text.alignment = UnityEngine.TextAnchor.MiddleCenter;
                text.fontSize = maxFontSize;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = minFontSize;
                text.resizeTextMaxSize = maxFontSize;
#if REWIRED_DEFAULT_FONT_IS_LEGACYRUNTIME
                const string defaultFontName = "LegacyRuntime.ttf";
#else
                const string defaultFontName = "Arial.ttf";
#endif
                text.font = UnityEngine.Resources.GetBuiltinResource<UnityEngine.Font>(defaultFontName);
#if UNITY_5_2_PLUS
                text.raycastTarget = false;
#endif
                glyphOrText.textComponent = text;
            }

            return prefab;
        }
    }
}
