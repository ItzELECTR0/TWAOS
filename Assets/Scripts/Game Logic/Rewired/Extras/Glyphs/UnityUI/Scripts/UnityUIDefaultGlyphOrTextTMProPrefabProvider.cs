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

#if UNITY_2018_PLUS
#define REWIRED_SUPPORTS_TMPRO
#endif

#if REWIRED_SUPPORTS_TMPRO

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {

	internal class UnityUIDefaultGlyphOrTextTMProPrefabProvider {

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            UnityUIControllerElementGlyphBase.defaultGlyphOrTextPrefabProvider = CreateDefaultGlyphOrTextPrefab;
        }

        private static UnityEngine.GameObject CreateDefaultGlyphOrTextPrefab() {

            // Warning: Do not make any changes to this without also making equivalent
            // changes to non-TMPro version if they are meant to be in sync.

            UnityEngine.GameObject prefab = new UnityEngine.GameObject("Glyph or text prefab");
            prefab.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(prefab);

            UnityUIGlyphOrTextTMPro glyphOrText = prefab.AddComponent<UnityUIGlyphOrTextTMPro>();

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
                var text = go.AddComponent<TMPro.TextMeshProUGUI>();
                text.alignment = TMPro.TextAlignmentOptions.Center;
                text.fontSize = maxFontSize;
                text.enableAutoSizing = true;
                text.fontSizeMin = minFontSize;
                text.fontSizeMax = maxFontSize;
                text.raycastTarget = false;
                glyphOrText.textComponent = text;
            }

            return prefab;
        }
    }
}

#endif
