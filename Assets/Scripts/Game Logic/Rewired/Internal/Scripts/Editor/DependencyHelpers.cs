// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2023_PLUS
#endif

#if UNITY_2018 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2018_PLUS
#define REWIRED_SUPPORTS_TMPRO
#endif

//#define REWIRED_DEBUG_TEXTMESHPRODETECTOR

namespace Rewired.Editor {
#if REWIRED_SUPPORTS_TMPRO
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.Build;
#endif

#if REWIRED_SUPPORTS_TMPRO

    [InitializeOnLoad]
    internal class TextMeshProDetector : IActiveBuildTargetChanged {

        private const string playerPrefsKey_checkTMProInstalled = "Rewired_Dependency_Check_TMPro";

        private const string define_isTMProInstalled = "REWIRED_TMPRO_INSTALLED";
        private const string define_tmProAssetV1_1_0 = "REWIRED_TMPRO_ASSET_V_1_1_0_PLUS";
        private static string[] define_allSymbols = new string[] {
            define_isTMProInstalled,
            define_tmProAssetV1_1_0
        };

        // Static

        private const string menuRoot = "Window/Rewired/Setup/Dependencies/";
        
        [MenuItem(menuRoot + "Check for Text Mesh Pro")]
        private static void ManualCheckForTextMeshPro() {
            CheckForTextMeshPro();
        }

#if REWIRED_DEBUG_TEXTMESHPRODETECTOR
        [MenuItem(menuRoot + "Clear Detect Text Mesh Pro data")]
        private static void ClearTextMeshProSymbols() {
            RemoveAllScriptingSymbols();
            UnityEngine.PlayerPrefs.DeleteKey(playerPrefsKey_checkTMProInstalled);
        }
#endif

        private static BuildTarget[] __s_buildTarget_values;
        private static BuildTarget[] s_buildTarget_values {
            get {
                return __s_buildTarget_values != null ? __s_buildTarget_values : (__s_buildTarget_values = (BuildTarget[])Enum.GetValues(typeof(BuildTarget)));
            }
        }

        private static BuildTargetGroup[] __s_buildTargetGroupForBuildTarget;
        private static BuildTargetGroup[] s_buildTargetGroupForBuildTarget {
            get {
                if (__s_buildTargetGroupForBuildTarget != null) return __s_buildTargetGroupForBuildTarget;
                __s_buildTargetGroupForBuildTarget = new BuildTargetGroup[s_buildTarget_values.Length];
                for (int i = 0; i < s_buildTarget_values.Length; i++) {
                    __s_buildTargetGroupForBuildTarget[i] = BuildPipeline.GetBuildTargetGroup(s_buildTarget_values[i]);
                }
                return __s_buildTargetGroupForBuildTarget;
            }
        }

        private static BuildTargetGroup[] __s_supportedBuildTargetGroups;
        private static BuildTargetGroup[] s_supportedBuildTargetGroups {
            get {
                if (__s_supportedBuildTargetGroups != null) return __s_supportedBuildTargetGroups;

                var buildTargetGroups = s_buildTargetGroupForBuildTarget;
                var buildTargets = s_buildTarget_values;
                int count = buildTargets.Length; // both are the same length

                List<BuildTargetGroup> groups = new List<BuildTargetGroup>();

                for (int i = 0; i < count; i++) {
                    if (groups.Contains(buildTargetGroups[i])) continue;
                    if (BuildPipeline.IsBuildTargetSupported(buildTargetGroups[i], buildTargets[i])) {
                        groups.Add(buildTargetGroups[i]);
                    }
                }
                __s_supportedBuildTargetGroups = groups.ToArray();
                return __s_supportedBuildTargetGroups;
            }
        }

        static TextMeshProDetector() {
            DebugLog("Debugging TextMeshProDetector");
            TextMeshProDetector detector = new TextMeshProDetector();
        }

        private static void CheckForTextMeshPro() {
            DebugLog("Rewired: Checking if Text Mesh Pro is installed.");
            if (IsTextMeshProInstalled()) {
                AddScriptingSymbol(define_isTMProInstalled);
                DebugLog("Rewired: Detected Text Mesh Pro installed. Added scripting symbol: " + define_isTMProInstalled);
                if (TextMeshProUsesAssetV1_1_0()) {
                    AddScriptingSymbol(define_tmProAssetV1_1_0);
                    DebugLog("Rewired: Detected Text Mesh Pro Asset 1.1.0+ installed. Added scripting symbol: " + define_tmProAssetV1_1_0);
                }
            }

            // Store the version of Unity that was installed when last checked
            UnityEngine.PlayerPrefs.SetString(playerPrefsKey_checkTMProInstalled, UnityEngine.Application.unityVersion);
        }

        private static bool IsTextMeshProInstalled() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies == null) return false;
            for (int i = 0; i < assemblies.Length; i++) {
                if (assemblies[i].FullName.StartsWith("Unity.TextMeshPro")) {
                    return true;
                }
            }
            return false;
        }

        private static bool TextMeshProUsesAssetV1_1_0() {
            try {
                Assembly assembly = null;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (assemblies == null) return false;
                for (int i = 0; i < assemblies.Length; i++) {
                    if (assemblies[i].FullName.StartsWith("Unity.TextMeshPro")) {
                        assembly = assemblies[i];
                        break;
                    }
                }
                if (assembly == null) return false;

                System.Type type;

                // TMPro.TMP_SpriteCharacter
                {
                    type = assembly.GetType("TMPro.TMP_SpriteCharacter");
                    if (type == null) throw new ArgumentNullException("type");
                    var spriteChararcter = System.Activator.CreateInstance(type);
                    if (spriteChararcter == null) throw new ArgumentNullException("TMPro.TMP_SpriteCharacter.ctor");
                    if (type.GetProperty("glyph", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("glyph");
                    if (type.GetProperty("unicode", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("unicode");
                    if (type.GetProperty("name", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("name");
                    if (type.GetProperty("scale", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("scale");
                    if (type.GetProperty("glyphIndex", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("glyphIndex");
                }

                // TMPro.TMP_SpriteGlyph
                {
                    type = assembly.GetType("TMPro.TMP_SpriteGlyph");
                    if (type == null) throw new ArgumentNullException("type");
                    if (System.Activator.CreateInstance(type) == null) throw new ArgumentNullException("TMPro.TMP_SpriteGlyph.ctor");
                    if (type.GetField("sprite", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("sprite");
                }

                // TMPro.TMP_SpriteAsset
                {
                    type = assembly.GetType("TMPro.TMP_SpriteAsset");
                    if (type == null) throw new ArgumentNullException("type");
                    if (type.GetProperty("version", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("version");
                    if (type.GetProperty("spriteCharacterTable", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("spriteCharacterTable");
                    if (type.GetProperty("spriteGlyphTable", BindingFlags.Public | BindingFlags.Instance) == null) throw new ArgumentNullException("spriteGlyphTable");
                }

                return true;

            } catch {
                return false;
            }
        }

        private static void AddScriptingSymbol(string symbol) {
            for (int i = 0; i < s_supportedBuildTargetGroups.Length; i++) {
                AddScriptingSymbol(s_supportedBuildTargetGroups[i], symbol);
            }
        }

        private static void AddScriptingSymbol(BuildTargetGroup group, string symbol) {
            if (string.IsNullOrEmpty(symbol)) return;
            if (!IsBuildTargetGroupSupported(group)) return;
            if (ContainsScriptingSymbol(group, symbol)) return;
#if UNITY_2023_PLUS
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);
            string symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget) ?? string.Empty;
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols + ";" + symbol);
#else
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group) ?? string.Empty;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, symbols + ";" + symbol);
#endif
        }

        private static void RemoveScriptingSymbol(string symbol) {
            for (int i = 0; i < s_supportedBuildTargetGroups.Length; i++) {
                RemoveScriptingSymbol(s_supportedBuildTargetGroups[i], symbol);
            }
        }
        private static void RemoveScriptingSymbol(BuildTargetGroup group, string symbol) {
            if (string.IsNullOrEmpty(symbol)) return;
            if (!IsBuildTargetGroupSupported(group)) return;
            if (!ContainsScriptingSymbol(group, symbol)) return;
#if UNITY_2023_PLUS
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);
            string symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget) ?? string.Empty;
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, System.Text.RegularExpressions.Regex.Replace(symbols, "[;]*" + symbol, ""));
#else
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group) ?? string.Empty;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, System.Text.RegularExpressions.Regex.Replace(symbols, "[;]*" + symbol, ""));
#endif
        }

        private static void RemoveAllScriptingSymbols() {
            foreach (string symbol in define_allSymbols) {
                RemoveScriptingSymbol(symbol);
            }
        }

        private static bool ContainsScriptingSymbol(BuildTargetGroup group, string symbol) {
            if (string.IsNullOrEmpty(symbol)) return false;
            if (!IsBuildTargetGroupSupported(group)) return false;
#if UNITY_2023_PLUS
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);
            string symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget) ?? string.Empty;
#else
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group) ?? string.Empty;
#endif
            string[] split = symbols.Split(';');
            for (int i = 0; i < split.Length; i++) {
                if (split[i].Trim().Equals(symbol)) {
                    return true;
                }
            }
            return false;
        }

        private static bool AllContainScriptingSymbol(string symbol) {
            for (int i = 0; i < s_supportedBuildTargetGroups.Length; i++) {
                if (!ContainsScriptingSymbol(s_supportedBuildTargetGroups[i], symbol)) return false;
            }
            return true;
        }

        private static bool IsBuildTargetGroupSupported(BuildTargetGroup buildTargetGroup) {
            int count = s_supportedBuildTargetGroups.Length;
            for (int i = 0; i < count; i++) {
                if (s_supportedBuildTargetGroups[i] == buildTargetGroup) return true;
            }
            return false;
        }

        // Instance

        public TextMeshProDetector() { // must be public for IActiveBuildTargetChanged

            // Detect if Text Mesh Pro is installed and set a scripting define symbol
            // Note: This will not remove symbols if TMPro is uninstalled to save wasted
            // CPU time checking every domain reload.

#if !REWIRED_TMPRO_INSTALLED && !REWIRED_DEBUG_TEXTMESHPRODETECTOR
            // Check once per Unity version to avoid wasting resources
            {
                string unityVersion = UnityEngine.PlayerPrefs.HasKey(playerPrefsKey_checkTMProInstalled) ? UnityEngine.PlayerPrefs.GetString(playerPrefsKey_checkTMProInstalled) : null;
                if (!string.Equals(UnityEngine.Application.unityVersion, unityVersion)) {
                    CheckForTextMeshPro();
                }
            }
#endif
        }

        int IOrderedCallback.callbackOrder { get { return 0; } }

        // Handle newly installed build targets.
        // Add symbols for the new built target on build target change.
        void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) {
            if (previousTarget == newTarget) return;
#if REWIRED_TMPRO_INSTALLED
            if (!ContainsScriptingSymbol(BuildPipeline.GetBuildTargetGroup(newTarget), define_isTMProInstalled)) {
                CheckForTextMeshPro();
            }
#endif
        }

        [System.Diagnostics.Conditional("REWIRED_DEBUG_TEXTMESHPRODETECTOR")]
        private static void DebugLog(object o) {
            UnityEngine.Debug.Log(o);
        }
    }
#endif
}
