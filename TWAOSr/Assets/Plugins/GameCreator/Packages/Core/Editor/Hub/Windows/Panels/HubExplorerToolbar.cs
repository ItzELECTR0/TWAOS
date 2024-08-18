using System;
using GameCreator.Runtime.Common;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public class HubExplorerToolbar : VisualElement
    {
        private const int SEARCH_DEBOUNCE_MS = 500;

        private const string CLASS_SEARCH_FIELD = "gc-toolbar-searchfield";
        private const string CLASS_BUTTON_ICON = "gc-toolbar-button-icon";
        
        private const string TOOLTIP_BUTTON_REFRESH = "Refresh";
        private const string TOOLTIP_BUTTON_LATEST = "Latest packages";
        
        private static readonly IIcon ICON_LATEST = new IconHome(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_REFRESH = new IconRefresh(ColorTheme.Type.TextNormal);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly HubExplorerWindow m_Window;
        
        private Toolbar m_Toolbar;
        private ToolbarMenu m_MenuAccount;
        private ToolbarButton m_ButtonLatest;
        private ToolbarButton m_ButtonRefresh;
        private ToolbarSearchField m_SearchField;
        
        private IVisualElementScheduledItem m_ScheduleSearch;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public ToolbarSearchField SearchField => this.m_SearchField;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public HubExplorerToolbar(HubExplorerWindow window)
        {
            this.m_Window = window;
        }

        public void OnEnable()
        {
            this.m_Toolbar = new Toolbar();

            this.SetupAccountDropdown();
            this.SetupSpacer();
            this.SetupLatestButton();
            this.SetupSearchField();
            this.SetupRefreshButton();

            this.Add(this.m_Toolbar);

            Collection.EventIsFetching += this.OnIsFetching;
            Collection.EventFetch += this.OnFetch;
        }

        public void OnDisable()
        {
            this.m_SearchField.UnregisterValueChangedCallback(this.OnChangeSearch);
            
            Collection.EventIsFetching -= this.OnIsFetching;
            Collection.EventFetch -= this.OnFetch;
        }

        // PRIVATE SETUP METHODS: -----------------------------------------------------------------
        
        private void SetupAccountDropdown()
        {
            this.m_MenuAccount = new ToolbarMenu { text = "Account" };

            this.m_MenuAccount.menu.AppendAction(
                "Settings",
                action => HubSettingsWindow.OpenWindow()
            );

            this.m_MenuAccount.menu.AppendSeparator();

            this.m_MenuAccount.menu.AppendAction(
                "Go to my account",
                action => Auth.GoToMyAccount()
            );

            this.m_MenuAccount.menu.AppendAction(
                "Create an account",
                action => Auth.GoToLoginAccount(), 
                Auth.IsAuthenticated 
                    ? DropdownMenuAction.Status.Disabled
                    : DropdownMenuAction.Status.Normal
            );

            this.m_Toolbar.Add(this.m_MenuAccount);
        }

        private void SetupSpacer()
        {
            VisualElement spacer = new VisualElement();
            spacer.AddToClassList("gc-spacer");
            
            this.m_Toolbar.Add(spacer);
        }
        
        private void SetupSearchField()
        {
            this.m_SearchField = new ToolbarSearchField();
            this.m_SearchField.AddToClassList(CLASS_SEARCH_FIELD);
            this.m_SearchField.RegisterValueChangedCallback(this.OnChangeSearch);
            
            this.m_Toolbar.Add(this.m_SearchField);
        }

        private void SetupLatestButton()
        {
            this.m_ButtonLatest = new ToolbarButton(this.Latest)
            {
                tooltip = TOOLTIP_BUTTON_LATEST
            };
            
            this.m_ButtonLatest.AddToClassList(CLASS_BUTTON_ICON);
            this.m_ButtonLatest.Add(new Image { image = ICON_LATEST.Texture });
            
            this.m_Toolbar.Add(this.m_ButtonLatest);
        }

        private void SetupRefreshButton()
        {
            this.m_ButtonRefresh = new ToolbarButton(this.Search)
            {
                tooltip = TOOLTIP_BUTTON_REFRESH
            };
            
            this.m_ButtonRefresh.AddToClassList(CLASS_BUTTON_ICON);
            this.m_ButtonRefresh.Add(new Image { image = ICON_REFRESH.Texture });
            
            this.m_Toolbar.Add(this.m_ButtonRefresh);
        }

        // CALLBACKS: -----------------------------------------------------------------------------
        
        private void OnChangeSearch(ChangeEvent<string> changeEvent)
        {
            this.m_ScheduleSearch ??= this.schedule.Execute(this.Search);
            this.m_ScheduleSearch.ExecuteLater(SEARCH_DEBOUNCE_MS);
        }
        
        private void OnIsFetching(bool isFetching)
        {
            this.m_ButtonLatest.SetEnabled(!isFetching);
            this.m_ButtonRefresh.SetEnabled(!isFetching);
        }
        
        private void OnFetch(string query, HitPayload payload)
        {
            this.m_Window.SetCache(query, 0, payload);
            GameCreatorHub.Data = payload?.values ?? Array.Empty<HitData>();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Search() => _ = this.m_Window.Search();
        private void Latest() => _ = this.m_Window.Latest();
    }
}