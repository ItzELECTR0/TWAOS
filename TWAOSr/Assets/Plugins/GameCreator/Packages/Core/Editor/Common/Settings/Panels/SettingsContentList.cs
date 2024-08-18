using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    internal class SettingsContentList : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "Settings/Stylesheets/SettingsList";

        private const string TOOLBAR_NAME = "GC-Settings-List-Toolbar";
        private const string LIST_NAME = "GC-Settings-List";
        private const int LIST_HEIGHT = 24;

        private const string ELEMENT_CLASS = "gc-settings-list-element";
        
        private const string ELEMENT_NAME_ICON = "GC-Settings-List-Element-Icon";
        private const string ELEMENT_NAME_TITLE = "GC-Settings-List-Element-Title";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly SettingsWindow m_Window;

        private Toolbar m_Toolbar;
        private ListView m_ListView;
        
        private TAssetRepository[] m_Assets = Array.Empty<TAssetRepository>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Index
        {
            get => this.m_ListView?.selectedIndex ?? -1;
            set => this.m_ListView.selectedIndex = value;
        }

        public TAssetRepository Asset
        {
            get
            {
                if (this.m_Assets.Length <= 0) return null;
                if (this.Index < 0) return null;
                if (this.Index >= this.m_Assets.Length) return null;

                return this.m_Assets[this.Index];
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public SettingsContentList(SettingsWindow window)
        {
            this.m_Window = window;
        }

        public void OnEnable()
        {
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.styleSheets.Add(styleSheet);
            }
            
            string[] repositoryGuids = AssetDatabase.FindAssets("t:TAssetRepository");
            this.m_Assets = new TAssetRepository[repositoryGuids.Length];
            
            for (int i = 0; i < repositoryGuids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(repositoryGuids[i]);
                this.m_Assets[i] = AssetDatabase.LoadAssetAtPath<TAssetRepository>(assetPath);
            }
            
            Array.Sort(this.m_Assets, CompareAssetRepositories);

            this.m_Toolbar = new Toolbar { name = TOOLBAR_NAME };
            this.m_Toolbar.Add(new Label("Settings"));
            this.Add(this.m_Toolbar);
            
            this.m_ListView = new ListView(this.m_Assets, LIST_HEIGHT, this.MakeItem, this.BindItem)
            {
                name = LIST_NAME,
                reorderable = false,
                focusable = true,
                selectionType = SelectionType.Single
            };

            this.m_ListView.selectionChanged += OnContentSelectItem;
            
            this.Add(this.m_ListView);
        }

        public void OnDisable()
        {
            this.m_ListView.selectionChanged -= OnContentSelectItem;
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnContentSelectItem(IEnumerable<object> list)
        {
            this.m_Window.OnChangeSelection(this.m_ListView.selectedIndex);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int FindIndex(string repositoryID)
        {
            for (int i = 0; i < this.m_Assets.Length; ++i)
            {
                if (this.m_Assets[i].RepositoryID == repositoryID) return i;
            }

            return -1;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private VisualElement MakeItem()
        {
            VisualElement element = new VisualElement();
            element.AddToClassList(ELEMENT_CLASS);
            
            element.Add(new Image { name = ELEMENT_NAME_ICON });
            element.Add(new Label { name = ELEMENT_NAME_TITLE });
            
            return element;
        }
        
        private void BindItem(VisualElement element, int index)
        {
            element.Q<Image>(ELEMENT_NAME_ICON).image = this.m_Assets[index].Icon.Texture;
            element.Q<Label>(ELEMENT_NAME_TITLE).text = this.m_Assets[index].Name;
        }
        
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------
        
        private static int CompareAssetRepositories(TAssetRepository x, TAssetRepository y)
        {
            if (x == null || y == null) return 0;
            return x.Priority.CompareTo(y.Priority);
        }
    }
}