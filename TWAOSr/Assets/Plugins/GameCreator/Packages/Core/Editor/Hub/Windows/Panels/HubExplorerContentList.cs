using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public class HubExplorerContentList : VisualElement
    {
        private const string LIST_NAME = "GC-Hub-Content-List";
        private const int LIST_HEIGHT = 24;

        private const int ANIMATION_REFRESH_RATE = 50;

        private const string SEARCH_NAME = "GC-Hub-Content-Search";
        private const string SEARCH_LABEL = "Searching...";
        
        private const string ELEMENT_CLASS = "gc-hub-content-element";
        
        private const string ELEMENT_NAME_ICON = "GC-Hub-Content-Element-Icon";
        private const string ELEMENT_NAME_TITLE = "GC-Hub-Content-Element-Title";
        private const string ELEMENT_NAME_VERSION = "GC-Hub-Content-Element-Version";
        private const string ELEMENT_NAME_STATUS = "GC-Hub-Content-Element-Status";

        private static readonly IIcon ICON_INSTRUCTION = new IconInstructions(ColorTheme.Type.Blue);
        private static readonly IIcon ICON_CONDITION = new IconConditions(ColorTheme.Type.Green);
        private static readonly IIcon ICON_EVENT = new IconTriggers(ColorTheme.Type.Yellow);
        private static readonly IIcon ICON_NONE = new IconNone(ColorTheme.Type.White);

        private static readonly IIcon ICON_CHECK = new IconCheckmark(ColorTheme.Type.Green);
        private static readonly IIcon ICON_UPDATE = new IconDownload(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_UPLOAD = new IconUpload(ColorTheme.Type.Green);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly HubExplorerWindow m_Window;
        
        private VisualElement m_SearchingTooltip;
        private IVisualElementScheduledItem m_SearchingSchedule;

        private Label m_SearchingLabel;
        private ProgressBar m_SearchingProgress;
        
        private ListView m_ListView;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Index
        {
            get => this.m_ListView?.selectedIndex ?? -1;
            set => this.m_ListView.selectedIndex = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public HubExplorerContentList(HubExplorerWindow window)
        {
            this.m_Window = window;
        }

        public void OnEnable()
        {
            this.SetupSearchTooltip();
            this.SetupListView();

            GameCreatorHub.EventChangeData += this.OnChangeData;
            Upload.EventAfterUpload += this.OnUploadPackage;
        }

        public void OnDisable()
        {
            Collection.EventIsFetching -= this.OnIsFetching;

            this.m_ListView.selectionChanged -= OnContentSelectItem;
            this.m_ListView.itemsChosen -= OnContentChooseItem;
            
            GameCreatorHub.EventChangeData -= this.OnChangeData;
            Upload.EventAfterUpload -= this.OnUploadPackage;
        }
        
        // PRIVATE SETUP METHODS: -----------------------------------------------------------------
        
        private void SetupSearchTooltip()
        {
            this.m_SearchingTooltip = new VisualElement { name = SEARCH_NAME };
            this.m_SearchingLabel = new Label(SEARCH_LABEL);
            this.m_SearchingProgress = new ProgressBar { value = 0 };

            this.m_SearchingSchedule = this.m_SearchingTooltip.schedule
                .Execute(state => this.m_SearchingProgress.value = state.now * 0.1f % 100f)
                .Every(ANIMATION_REFRESH_RATE);

            this.m_SearchingTooltip.Add(this.m_SearchingLabel);
            this.m_SearchingTooltip.Add(this.m_SearchingProgress);
            
            this.Add(this.m_SearchingTooltip);
            
            this.OnIsFetching(Collection.IsFetching);
            Collection.EventIsFetching += this.OnIsFetching;
        }
        
        private void SetupListView()
        {
            this.m_ListView = new ListView(GameCreatorHub.Data, LIST_HEIGHT, this.MakeItem, this.BindItem)
            {
                name = LIST_NAME,
                reorderable = false,
                focusable = true,
                selectionType = SelectionType.Single,
            };
            
            this.m_ListView.selectionChanged += OnContentSelectItem;
            this.m_ListView.itemsChosen += OnContentChooseItem;
            
            this.Add(this.m_ListView);
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnIsFetching(bool isFetching)
        {
            switch (isFetching)
            {
                case true: 
                    this.m_SearchingSchedule.Resume();
                    this.m_SearchingTooltip.style.display = DisplayStyle.Flex;
                    break;

                case false:
                    this.m_SearchingSchedule.Pause();
                    this.m_SearchingTooltip.style.display = DisplayStyle.None;
                    break;
            }
        }

        private void OnChangeData()
        {
            this.RefreshList();
        }
        
        private void OnUploadPackage()
        {
            _ = this.m_Window.Search();
        }

        private void OnContentSelectItem(IEnumerable<object> list)
        {
            this.m_Window.OnChangeSelection(this.m_ListView.selectedIndex);
        }
        
        private void OnContentChooseItem(IEnumerable<object> list)
        {
            this.m_Window.OnChangeSelection(this.m_ListView.selectedIndex);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private VisualElement MakeItem()
        {
            VisualElement element = new VisualElement();
            element.AddToClassList(ELEMENT_CLASS);
            
            element.Add(new Image { name = ELEMENT_NAME_ICON });
            element.Add(new Label { name = ELEMENT_NAME_TITLE });
            element.Add(new Label { name = ELEMENT_NAME_VERSION });
            element.Add(new Image { name = ELEMENT_NAME_STATUS });
            
            return element;
        }
        
        private void BindItem(VisualElement element, int index)
        {
            HitData package = GameCreatorHub.Data[index];
            IIcon icon = package.type switch
            {
                "instruction" => ICON_INSTRUCTION,
                "condition" => ICON_CONDITION,
                "event" => ICON_EVENT,
                _ => ICON_NONE
            };

            element.Q<Image>(ELEMENT_NAME_ICON).image = icon.Texture;
            element.Q<Label>(ELEMENT_NAME_TITLE).text = package.name;
            element.Q<Label>(ELEMENT_NAME_VERSION).text = package.version.ToString();
            
            element.Q<Image>(ELEMENT_NAME_STATUS).image = ICON_NONE.Texture;
            element.Q<Image>(ELEMENT_NAME_STATUS).tooltip = string.Empty;

            string candidatePath = Download.GetInstallationPath(
                package.type, 
                package.category,
                package.filename
            );
            
            TextAsset candidate = AssetDatabase.LoadAssetAtPath<TextAsset>(candidatePath);
            if (candidate == null) return;
            
            string typeName = Path.GetFileNameWithoutExtension(package.filename);
            Type candidateType = TypeUtils.GetTypeFromName(typeName);
                
            if (candidateType != null)
            {
                VersionAttribute candidateVersion = candidateType
                    .GetCustomAttributes<VersionAttribute>(true)
                    .FirstOrDefault();
                
                VersionAttribute serverVersion = new VersionAttribute(
                    package.version.x,
                    package.version.y,
                    package.version.z
                );

                IIcon statusIcon = null;
                string statusTooltip = string.Empty;
                
                switch (serverVersion.CompareTo(candidateVersion))
                {
                    case 0:
                        statusIcon = ICON_CHECK;
                        statusTooltip = "Package is up to date";
                        break;
                    
                    case 1:
                        statusIcon = ICON_UPDATE;
                        statusTooltip = "A new version is available";
                        break;
                    
                    case -1:
                        statusIcon = ICON_UPLOAD;
                        statusTooltip = "This package is newer";
                        break;
                }
                
                element.Q<Image>(ELEMENT_NAME_STATUS).image = statusIcon?.Texture;
                element.Q<Image>(ELEMENT_NAME_STATUS).tooltip = statusTooltip;
            }
        }
        
        private void RefreshList()
        {
            this.m_ListView.itemsSource = GameCreatorHub.Data;
            this.m_ListView.selectedIndex = 0;
            
            this.m_ListView.Rebuild();
        }
    }
}