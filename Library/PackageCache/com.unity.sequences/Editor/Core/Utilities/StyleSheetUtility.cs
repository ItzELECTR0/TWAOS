using System.Collections.Generic;
using System.IO;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    internal static class StyleSheetUtility
    {
        static readonly string k_BasePath = Path.Combine("Packages", "com.unity.sequences", "Editor");

        static readonly string[] k_StyleSheetLookupPaths = new[]
        {
            Path.Combine(k_BasePath, "Core", "UI", "UIData", "StyleSheets"),
            Path.Combine(k_BasePath, "SequenceAssemblyWindow", "UIData", "StyleSheets")
        };

        static readonly string k_CommonFileName = "Common";
        static readonly string k_CommonLightFileName = "CommonLight";
        static readonly string k_CommonDarkFileName = "CommonDark";

        static StyleSheet commonStyleSheet => GetStyleSheet(k_CommonFileName);
        static StyleSheet commonLightStyleSheet => GetStyleSheet(k_CommonLightFileName);
        static StyleSheet commonDarkStyleSheet => GetStyleSheet(k_CommonDarkFileName);

        static StyleSheet GetStyleSheet(string name)
        {
            foreach (var lookupPath in k_StyleSheetLookupPaths)
            {
                var path = Path.Combine(lookupPath, $"{name}.uss");
                var asset = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (asset != null)
                    return asset;
            }

            return null;
        }

        static IEnumerable<StyleSheet> GetStyleSheets(
            string ussName = null,
            string ussDarkName = null,
            string ussLightName = null)
        {
            yield return commonStyleSheet;

            if (EditorGUIUtility.isProSkin)
                yield return commonDarkStyleSheet;
            else
                yield return commonLightStyleSheet;

            if (ussName == null)
                yield break;

            var customStyle = GetStyleSheet(ussName);
            if (customStyle != null)
                yield return customStyle;

            if (EditorGUIUtility.isProSkin)
            {
                var darkCustomStyle = GetStyleSheet(ussDarkName ?? ussName + "Dark");
                if (darkCustomStyle != null)
                    yield return darkCustomStyle;
            }
            else
            {
                var lightCustomStyle = GetStyleSheet(ussLightName ?? ussName + "Light");
                if (lightCustomStyle != null)
                    yield return lightCustomStyle;
            }
        }

        public static void SetStyleSheets(
            VisualElement visualElement,
            string ussName = null,
            string ussDarkName = null,
            string ussLightName = null)
        {
            foreach (var styleSheet in GetStyleSheets(ussName, ussDarkName, ussLightName))
            {
                visualElement.styleSheets.Add(styleSheet);
            }
        }
    }
}
