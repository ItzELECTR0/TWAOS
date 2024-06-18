using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class SettingsWindow : EditorWindow
    {
        private const string MENU_ITEM_CLEAR_EDITOR_PREFS = "Edit/Clear All EditorPrefs";
        private const string MENU_ITEM_OPEN = "Game Creator/Settings #%k";
        private const string MENU_TITLE = "Game Creator Settings";

        private const int MIN_WIDTH = 800;
        private const int MIN_HEIGHT = 600;
        
        private const int FIXED_PANEL_INDEX = 0;
        private const float FIXED_PANEL_WIDTH = 250f;

        private const string USS_PATH = EditorPaths.COMMON + "Settings/Stylesheets/SettingsWindow";
        
        private const string KEY_CACHE_INDEX = "gcset:cache-index";

        private static IIcon ICON_WINDOW;
        private static SettingsWindow WINDOW;

        public const int INIT_PRIORITY_HIGH = 0;
        public const int INIT_PRIORITY_DEFAULT = 1;
        public const int INIT_PRIORITY_LOW = 2;
        
        public static readonly List<InitRunner> InitRunners = new List<InitRunner>();

        // PROPERTIES: ----------------------------------------------------------------------------

        private static int CacheIndex
        {
            get => EditorPrefs.GetInt(KEY_CACHE_INDEX, 0);
            set => EditorPrefs.SetInt(KEY_CACHE_INDEX, value);
        }

        internal SettingsContentList ContentList { get; set; }
        internal SettingsContentDetails ContentDetails { get; set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int> EventChangeSelection;

        // INITIALIZERS: --------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            EditorApplication.delayCall += DeferredInitializeOnLoad;
        }

        private static void DeferredInitializeOnLoad()
        {
            Type[] types = TypeUtils.GetTypesDerivedFrom(typeof(TAssetRepository));
            foreach (Type type in types)
            {
                string[] foundGuids = AssetDatabase.FindAssets($"t:{type}");
                if (foundGuids.Length > 0) continue;

                TAssetRepository asset = CreateInstance(type) as TAssetRepository;
                if (asset == null) continue;
                
                DirectoryUtils.RequirePath(asset.AssetPath);
                string assetPath = PathUtils.Combine(
                    asset.AssetPath, 
                    $"{asset.RepositoryID}.asset"
                );
                
                DirectoryUtils.RequireFilepath(assetPath);
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
            
            InitRunners.Sort((a, b) => a.Order.CompareTo(b.Order));
            foreach (InitRunner initRunner in InitRunners)
            {
                if (!initRunner.CanRun()) continue;
                
                initRunner.Run();
                return;
            }
        }
        
        [MenuItem(MENU_ITEM_CLEAR_EDITOR_PREFS, false, 270)]
        private static void RevealPersistentDataFolder()
        {
            bool confirmation = EditorUtility.DisplayDialog(
                "Clear All EditorPrefs",
                "Are you sure you want to clear all PlayerPrefs? This action cannot be undone.",
                "Yes", "Cancel"
            );
            
            if (confirmation) EditorPrefs.DeleteAll();
        }

        [MenuItem(MENU_ITEM_OPEN, priority = 10)]
        public static void OpenWindow()
        {
            SetupWindow();
            WINDOW.ContentList.Index = CacheIndex;
        }

        public static void OpenWindow(string repositoryID)
        {
            SetupWindow();
            
            int index = WINDOW.ContentList.FindIndex(repositoryID);
            WINDOW.ContentList.Index = index >= 0 ? index : CacheIndex;
        }

        private static void SetupWindow()
        {
            if (WINDOW != null) WINDOW.Close();
            
            WINDOW = GetWindow<SettingsWindow>();
            WINDOW.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
        }

        private void OnEnable()
        {
            ICON_WINDOW ??= new IconWindowSettings(ColorTheme.Type.TextLight);
            this.titleContent = new GUIContent(MENU_TITLE, ICON_WINDOW.Texture);
            
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.rootVisualElement.styleSheets.Add(styleSheet);
            }

            TwoPaneSplitView splitView = new TwoPaneSplitView(
                FIXED_PANEL_INDEX,
                FIXED_PANEL_WIDTH,
                TwoPaneSplitViewOrientation.Horizontal
            );

            this.rootVisualElement.Add(splitView);
            
            this.ContentList = new SettingsContentList(this);
            this.ContentDetails = new SettingsContentDetails(this);

            splitView.Add(this.ContentList);
            splitView.Add(this.ContentDetails);
            
            this.ContentList.OnEnable();
            this.ContentDetails.OnEnable();
        }

        private void OnDisable()
        {
            this.ContentList?.OnDisable();
            this.ContentDetails?.OnDisable();
        }

        // CALLBACKS: -----------------------------------------------------------------------------

        public void OnChangeSelection(int index)
        {
            CacheIndex = index;
            this.EventChangeSelection?.Invoke(index);
        }
    }
}
