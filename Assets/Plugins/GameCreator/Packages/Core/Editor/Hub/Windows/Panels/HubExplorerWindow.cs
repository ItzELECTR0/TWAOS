using System;
using System.Threading.Tasks;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public class HubExplorerWindow : EditorWindow
    {
        private const string MENU_ITEM = "Game Creator/Game Creator Hub #%h";
        private const string MENU_TITLE = "Game Creator Hub";

        private const int MIN_WIDTH = 800;
        private const int MIN_HEIGHT = 600;

        private const string USS_PATH = EditorPaths.HUB + "Windows/Stylesheets/HubExplorerWindow";
        
        private const string NAME_TOOLBAR = "GC-Hub-Toolbar";
        private const string NAME_CONTENT = "GC-Hub-Content";

        private const string KEY_CACHE_PAYLOAD = "gchub:cache-payload";
        private const string KEY_CACHE_INDEX = "gchub:cache-index";
        private const string KEY_CACHE_QUERY = "gchub:cache-query";
        
        private static IIcon ICON_WINDOW;
        private static HubExplorerWindow WINDOW;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public HubExplorerToolbar Toolbar { get; private set; }
        public HubExplorerContent Content { get; private set; }

        public string CacheQuery => EditorPrefs.GetString(KEY_CACHE_QUERY, string.Empty);
        
        public int CacheIndex
        {
            get => EditorPrefs.GetInt(KEY_CACHE_INDEX, 0);
            private set => EditorPrefs.SetInt(KEY_CACHE_INDEX, value);
        }

        public HitPayload CachePayload
        {
            get
            {
                string json = EditorPrefs.GetString(KEY_CACHE_PAYLOAD, string.Empty);
                if (string.IsNullOrEmpty(json)) return null;
                
                HitPayload payload = new HitPayload();
                EditorJsonUtility.FromJsonOverwrite(json, payload);
                return payload;
            }
        }
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int> EventChangeSelection;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        [MenuItem(MENU_ITEM, priority = 30)]
        public static void OpenWindow()
        {
            if (WINDOW != null) WINDOW.Close();
            WINDOW = GetWindow<HubExplorerWindow>();
            WINDOW.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
        }

        private void OnEnable()
        {
            ICON_WINDOW ??= new IconWindowHub(ColorTheme.Type.TextLight);
            this.titleContent = new GUIContent(MENU_TITLE, ICON_WINDOW.Texture);
            
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.rootVisualElement.styleSheets.Add(styleSheet);
            }
            
            this.Build();
            this.RestoreFromCache();
        }

        private void OnDisable()
        {
            this.Toolbar?.OnDisable();
            this.Content?.OnDisable();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void Build()
        {
            this.Toolbar = new HubExplorerToolbar(this) { name = NAME_TOOLBAR };
            this.Content = new HubExplorerContent(this) { name = NAME_CONTENT };

            this.rootVisualElement.Add(this.Toolbar);
            this.rootVisualElement.Add(this.Content);
            
            this.Toolbar.OnEnable();
            this.Content.OnEnable();
        }
        
        private void RestoreFromCache()
        {
            HitPayload cachePayload = this.CachePayload;
            int cacheIndex = this.CacheIndex;
            string cacheQuery = this.CacheQuery;

            if (cachePayload != null && cachePayload.values.Length > 0)
            {
                GameCreatorHub.Data = cachePayload.values;
                this.Content.ContentList.Index = cacheIndex;
                this.Toolbar.SearchField.SetValueWithoutNotify(cacheQuery);
            }
            else
            {
                _ = this.Latest();
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Rebuild()
        {
            this.Toolbar?.OnDisable();
            this.Content?.OnDisable();
            
            this.rootVisualElement.Clear();
            this.Build();
        }

        public void SetCache(string query, int index, HitPayload payload)
        {
            if (payload == null) return;
            
            EditorPrefs.SetString(KEY_CACHE_QUERY, query);
            EditorPrefs.SetInt(KEY_CACHE_INDEX, index);
            EditorPrefs.SetString(KEY_CACHE_PAYLOAD, EditorJsonUtility.ToJson(payload));
        }
        
        public async Task<HitPayload> Search()
        {
            string query = this.Toolbar.SearchField.value;
            return string.IsNullOrEmpty(query)
                ? await Collection.Latest()
                : await Collection.Search(query);
        }

        public async Task<HitPayload> Latest()
        {
            this.Toolbar.SearchField.SetValueWithoutNotify(string.Empty);
            return await Collection.Latest();
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        public void OnChangeSelection(int index)
        {
            this.CacheIndex = index;
            this.EventChangeSelection?.Invoke(index);
        }
    }
}
