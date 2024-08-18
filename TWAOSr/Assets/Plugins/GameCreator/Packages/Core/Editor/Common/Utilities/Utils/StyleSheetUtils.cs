using System.IO;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using UnityEditor;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    public static class StyleSheetUtils
    {
        private const string SHARED_STYLES_PATH = StyleSheetPaths.STYLESHEETS + "Shared/";
        
        private const string FMT_PATH = "{0}{1}.uss";

        private static readonly string[] THEMES =
        {
            "",
            "_Dark",
            "_Light",
        };

        private static readonly string[] SHARED_STYLES =
        {
            "CommonElements",
            "CommonValues",
            "CommonColors",
        };
        
        private static readonly Dictionary<int, StyleSheet> STYLESHEETS;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        static StyleSheetUtils()
        {
            STYLESHEETS = new Dictionary<int, StyleSheet>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static StyleSheet[] Load(params string[] paths)
        {
            List<StyleSheet> styleSheets = new List<StyleSheet>();
            styleSheets.AddRange(LoadShared());

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path)) continue;
                
                StyleSheet styleSheet = LoadStyleSheet(path);
                styleSheets.Add(styleSheet);
            }

            return styleSheets.ToArray();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static List<StyleSheet> LoadShared()
        {
            List<StyleSheet> styleSheets = new List<StyleSheet>();

            foreach (string style in SHARED_STYLES)
            {
                string path = PathUtils.Combine(SHARED_STYLES_PATH, style);
                styleSheets.Add(LoadStyleSheet(path));
            }

            return styleSheets;
        }

        private static string GetPath(string path, bool useTheme)
        {
            return string.Format(
                FMT_PATH, path,
                useTheme 
                    ? THEMES[0] 
                    : EditorGUIUtility.isProSkin ? THEMES[1] : THEMES[2]
            );
        }

        private static StyleSheet LoadStyleSheet(string path)
        {
            string completePath = GetPath(path, false);
            if (STYLESHEETS.TryGetValue(completePath.GetHashCode(), out StyleSheet styleSheet))
            {
                return styleSheet;
            }
            
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(completePath);
            if (!styleSheet)
            {
                completePath = GetPath(path, true);
                if (STYLESHEETS.TryGetValue(completePath.GetHashCode(), out styleSheet))
                {
                    return styleSheet;
                }
                
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(completePath);
            }

            Assert.IsNotNull(styleSheet, $"Null stylesheet at path {completePath}");
            STYLESHEETS.Add(completePath.GetHashCode(), styleSheet);
            
            return styleSheet;
        }
    }
}