// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

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

#if UNITY_5_4_PLUS
#define SUPPORTS_SCENE_MANAGEMENT
#endif

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired {

    [UnityEngine.AddComponentMenu("Rewired/Input Manager")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class InputManager : InputManager_Base {

        private bool ignoreRecompile;

        protected override void OnInitialized() {
            SubscribeEvents();
        }

        protected override void OnDeinitialized() {
            UnsubscribeEvents();
        }

        protected override void DetectPlatform() {
            // Set the editor and platform versions

#if UNITY_EDITOR
            // Do not check for recompile if using "Recompile After Finish Playing" mode or Rewired will be disabled and never reinitialize due to a bug in EditorApplication.isCompiling
            ignoreRecompile = (ScriptChangesDuringPlayOptions)UnityEditor.EditorPrefs.GetInt("ScriptCompilationDuringPlay", 0) == ScriptChangesDuringPlayOptions.RecompileAfterFinishedPlaying;
#endif

            scriptingBackend = Rewired.Platforms.ScriptingBackend.Mono;
            scriptingAPILevel = Rewired.Platforms.ScriptingAPILevel.Net20;
            editorPlatform = Rewired.Platforms.EditorPlatform.None;
            platform = Rewired.Platforms.Platform.Unknown;
            webplayerPlatform = Rewired.Platforms.WebplayerPlatform.None;
            isEditor = false;
            string deviceName = UnityEngine.SystemInfo.deviceName ?? string.Empty;
            string deviceModel = UnityEngine.SystemInfo.deviceModel ?? string.Empty;

#if UNITY_EDITOR
            isEditor = true;
            editorPlatform = Rewired.Platforms.EditorPlatform.Unknown;
#endif

#if UNITY_EDITOR_WIN
            editorPlatform = Rewired.Platforms.EditorPlatform.Windows;
#endif

#if UNITY_EDITOR_LINUX
            editorPlatform = Rewired.Platforms.EditorPlatform.Linux;
#endif

#if UNITY_EDITOR_OSX
            editorPlatform = Rewired.Platforms.EditorPlatform.OSX;
#endif

#if UNITY_EDITOR && REWIRED_DEBUG_MOCK_BUILD_PLAYER
            UnityEngine.Debug.LogWarning("Rewired is mocking the build player in the editor");
            isEditor = false;
            editorPlatform = Rewired.Platforms.EditorPlatform.None;
#endif

#if UNITY_STANDALONE_OSX
            platform = Rewired.Platforms.Platform.OSX;
#endif

#if UNITY_DASHBOARD_WIDGET

#endif

#if UNITY_STANDALONE_WIN
            platform = Rewired.Platforms.Platform.Windows;
#endif

#if UNITY_STANDALONE_LINUX
            platform = Rewired.Platforms.Platform.Linux;
#endif

#if UNITY_ANDROID
            platform = Rewired.Platforms.Platform.Android;
#if !UNITY_EDITOR
            // Handle special Android platforms
            if(CheckDeviceName("OUYA", deviceName, deviceModel)) {
                platform = Rewired.Platforms.Platform.Ouya;
            } else if(CheckDeviceName("Amazon AFT.*", deviceName, deviceModel)) {
                platform = Rewired.Platforms.Platform.AmazonFireTV;
            } else if(CheckDeviceName("razer Forge", deviceName, deviceModel)) {
#if REWIRED_OUYA && REWIRED_USE_OUYA_SDK_ON_FORGETV
                platform = Rewired.Platforms.Platform.Ouya;
#else
                platform = Rewired.Platforms.Platform.RazerForgeTV;
#endif
            }
#endif
#endif

#if UNITY_BLACKBERRY
            platform = Rewired.Platforms.Platform.Blackberry;
#endif

#if UNITY_IPHONE || UNITY_IOS
            platform = Rewired.Platforms.Platform.iOS;
#endif

#if UNITY_TVOS
            platform = Rewired.Platforms.Platform.tvOS;
#endif

#if UNITY_PS3
            platform = Rewired.Platforms.Platform.PS3;
#endif

#if UNITY_PS4
            platform = Rewired.Platforms.Platform.PS4;
#endif

#if UNITY_PS5
            platform = Rewired.Platforms.Platform.PS5;
#endif

#if UNITY_PSP2
            platform = Rewired.Platforms.Platform.PSVita;
#endif

#if UNITY_PSM
            platform = Rewired.Platforms.Platform.PSMobile;
#endif

#if UNITY_XBOX360
            platform = Rewired.Platforms.Platform.Xbox360;
#endif

#if UNITY_GAMECORE_XBOXONE
            platform = Rewired.Platforms.Platform.GameCoreXboxOne;
#elif UNITY_XBOXONE
            platform = Rewired.Platforms.Platform.XboxOne;
#endif

#if UNITY_GAMECORE_SCARLETT
            platform = Rewired.Platforms.Platform.GameCoreScarlett;
#endif

#if UNITY_WII
            platform = Rewired.Platforms.Platform.Wii;
#endif

#if UNITY_WIIU
            platform = Rewired.Platforms.Platform.WiiU;
#endif

#if UNITY_N3DS
            platform = Rewired.Platforms.Platform.N3DS;
#endif

#if UNITY_SWITCH
            platform = Rewired.Platforms.Platform.Switch;
#endif

#if UNITY_FLASH
            platform = Rewired.Platforms.Platform.Flash;
#endif

#if UNITY_METRO || UNITY_WSA || UNITY_WSA_8_0
            platform = Rewired.Platforms.Platform.WindowsAppStore;
#endif

#if UNITY_WSA_8_1
            platform = Rewired.Platforms.Platform.Windows81Store;
#endif

            // Windows 8.1 Universal
#if UNITY_WINRT_8_1 && !UNITY_WSA_8_1 // this seems to be the only way to detect this
            platform = Rewired.Platforms.Platform.Windows81Store;
#endif

            // Windows Phone overrides Windows Store -- this is not set when doing Universal 8.1 builds
#if UNITY_WP8 || UNITY_WP8_1 || UNITY_WP_8 || UNITY_WP_8_1 // documentation error on format of WP8 defines, so include both
            platform = Rewired.Platforms.Platform.WindowsPhone8;
#endif

#if UNITY_WSA_10_0
            platform = Rewired.Platforms.Platform.WindowsUWP;
#endif

#if UNITY_WEBGL
            platform = Rewired.Platforms.Platform.WebGL;
#endif

            // Check if Webplayer
#if UNITY_WEBPLAYER

            webplayerPlatform = Rewired.Utils.UnityTools.DetermineWebplayerPlatformType(platform, editorPlatform); // call this BEFORE you change the platform so we still know what base system this is
            platform = Rewired.Platforms.Platform.Webplayer;

#endif

#if ENABLE_MONO
            scriptingBackend = Rewired.Platforms.ScriptingBackend.Mono;
#endif

#if ENABLE_DOTNET
            scriptingBackend = Rewired.Platforms.ScriptingBackend.DotNet;
#endif

#if ENABLE_IL2CPP
            scriptingBackend = Rewired.Platforms.ScriptingBackend.IL2CPP;
#endif

#if NET_2_0
            scriptingAPILevel = Rewired.Platforms.ScriptingAPILevel.Net20;
#endif

#if NET_2_0_SUBSET
            scriptingAPILevel = Rewired.Platforms.ScriptingAPILevel.Net20Subset;
#endif

#if NET_4_6
            scriptingAPILevel = Rewired.Platforms.ScriptingAPILevel.Net46;
#endif

#if NET_STANDARD_2_0
            scriptingAPILevel = Rewired.Platforms.ScriptingAPILevel.NetStandard20;
#endif
        }

        protected override void CheckRecompile() {
#if UNITY_EDITOR
            if(ignoreRecompile) return;

            // Destroy system if recompiling
            if(UnityEditor.EditorApplication.isCompiling) { // editor is recompiling
                if(!isCompiling) { // this is the first cycle of recompile
                    isCompiling = true; // flag it
                    RecompileStart();
                }
                return;
            }

            // Check for end of compile
            if(isCompiling) { // compiling is done
                isCompiling = false; // flag off
                RecompileEnd();
            }
#endif
        }

        protected override Rewired.Utils.Interfaces.IExternalTools GetExternalTools() {
            return new Rewired.Utils.ExternalTools();
        }

        private bool CheckDeviceName(string searchPattern, string deviceName, string deviceModel) {
            return System.Text.RegularExpressions.Regex.IsMatch(deviceName, searchPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                System.Text.RegularExpressions.Regex.IsMatch(deviceModel, searchPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private void SubscribeEvents() {
            UnsubscribeEvents();
#if SUPPORTS_SCENE_MANAGEMENT
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }

        private void UnsubscribeEvents() {
#if SUPPORTS_SCENE_MANAGEMENT
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }

#if SUPPORTS_SCENE_MANAGEMENT
      
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
            OnSceneLoaded();
        }

#else
        private void OnLevelWasLoaded(int index) {
            OnSceneLoaded();
        }

#endif

#if UNITY_EDITOR
        private enum ScriptChangesDuringPlayOptions {
            RecompileAndContinuePlaying,
            RecompileAfterFinishedPlaying,
            StopPlayingAndRecompile
        }
#endif
    }
}