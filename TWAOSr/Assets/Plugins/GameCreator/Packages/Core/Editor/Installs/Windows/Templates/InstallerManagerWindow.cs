using System;
using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    public class InstallerManagerWindow : EditorWindow
    {
        private const string MENU_ITEM = "Game Creator/" + InstallManager.NAME + "...";
        private const string MENU_TITLE = "Game Creator " + InstallManager.NAME;

        private const int MIN_WIDTH = 800;
        private const int MIN_HEIGHT = 600;

        private const string USS_PATH = EditorPaths.INSTALLS + "Windows/StyleSheets/InstallerWindow";
        
        private const string NAME_CONTENT = "GC-Install-Content";
        
        private static IIcon ICON_WINDOW;
        private static InstallerManagerWindow WINDOW;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public InstallerContent Content { get; private set; }
        
        internal Dictionary<string, List<Installer>> InstallAssetsMap { get; private set; }
        internal Installer Selection { get; private set; }
        
        // EVENTS: --------------------------------------------------------------------------------

        internal event Action<string> EventListSelect;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static void Open(Installer asset)
        {
            OpenWindow();
            WINDOW.SelectInstallAsset(asset);
        }

        [MenuItem(MENU_ITEM, priority = 31)]
        public static void OpenWindow()
        {
            if (WINDOW != null) WINDOW.Close();
            WINDOW = GetWindow<InstallerManagerWindow>();
            WINDOW.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
        }

        private void OnEnable()
        {
            ICON_WINDOW ??= new IconWindowInstaller(ColorTheme.Type.TextLight);
            this.titleContent = new GUIContent(MENU_TITLE, ICON_WINDOW.Texture);
            
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.rootVisualElement.styleSheets.Add(styleSheet);
            }
            
            this.Content = new InstallerContent(this) { name = NAME_CONTENT };
            this.rootVisualElement.Add(this.Content);
            
            this.Content.OnEnable();
            
            this.Refresh();
            EditorApplication.projectChanged += this.Refresh;
        }

        private void OnDisable()
        {
            this.Content?.OnDisable();
            EditorApplication.projectChanged -= this.Refresh;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void SelectInstallAsset(Installer asset)
        {
            if (asset == null)
            {
                this.Selection = null;
                this.EventListSelect?.Invoke(string.Empty);
            }
            else
            {
                this.Selection = asset;
                this.EventListSelect?.Invoke(asset.Data.ID);   
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Refresh()
        {
            this.GatherAssets();
            this.Content.Refresh();
        }
        
        private void GatherAssets()
        {
            this.InstallAssetsMap = new Dictionary<string, List<Installer>>();
            
            string[] assetsGUIDS = AssetDatabase.FindAssets($"t:{nameof(Installer)}");
            foreach (string assetGUID in assetsGUIDS)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                Installer asset = AssetDatabase.LoadAssetAtPath<Installer>(assetPath);
                
                if (asset == null) continue;
                
                if (string.IsNullOrEmpty(asset.Data?.Name)) continue; 
                if (string.IsNullOrEmpty(asset.Data?.Module)) continue;

                if (!this.InstallAssetsMap.ContainsKey(asset.Data.Module))
                {
                    this.InstallAssetsMap.Add(
                        asset.Data.Module, 
                        new List<Installer>()
                    );
                }
                
                this.InstallAssetsMap[asset.Data.Module].Add(asset);
            }

            foreach (KeyValuePair<string,List<Installer>> entry in this.InstallAssetsMap)
            {
                entry.Value.Sort(this.SortAssets);
            }
        }

        private int SortAssets(Installer a, Installer b)
        {
            int complexityA = (int) a.Data.Complexity;
            int complexityB = (int) b.Data.Complexity;
            
            return complexityA.CompareTo(complexityB);
        }
    }
}