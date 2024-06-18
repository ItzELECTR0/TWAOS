using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    internal class InstallerElementInstall : VisualElement
    {
        private const string NAME_NAME = "GC-Install-Content-List-Install-Name";
        private const string NAME_TYPE = "GC-Install-Content-List-Install-Type";
        private const string NAME_VERS = "GC-Install-Content-List-Install-Vers";
        private const string NAME_STAT = "GC-Install-Content-List-Install-Stat";

        private const string CLASS_ACTIVE = "gc-install-content-list-install-active";
        private const string CLASS_INACTIVE = "gc-install-content-list-install-inactive";

        private const string TOOLTIP_SKIN = "Skin";
        private static readonly IIcon TYPE_SKIN = new IconLayers(ColorTheme.Type.Yellow);

        private static readonly IIcon STATUS_INSTALLED = new IconCheckmark(ColorTheme.Type.Green);
        private static readonly IIcon STATUS_UPDATE = new IconRotation(ColorTheme.Type.Blue);
        private static readonly IIcon STATUS_MISSING = new IconNone(ColorTheme.Type.TextLight);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly InstallerManagerWindow m_Window;
        private readonly Installer m_Asset;
        
        private readonly Label m_LabelName;
        private readonly Image m_ImageType;
        private readonly Label m_LabelVers;
        private readonly Image m_ImageStat;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstallerElementInstall(InstallerManagerWindow window, Installer asset)
        {
            this.m_Window = window;
            this.m_Asset = asset;
            
            this.m_LabelName = new Label { name = NAME_NAME };
            this.m_ImageType = new Image { name = NAME_TYPE };
            this.m_LabelVers = new Label { name = NAME_VERS };
            this.m_ImageStat = new Image { name = NAME_STAT };
            
            this.Add(this.m_LabelName);
            this.Add(this.m_ImageType);
            this.Add(this.m_LabelVers);
            this.Add(this.m_ImageStat);

            this.Refresh();

            this.RegisterCallback<ClickEvent>(this.OnClick);

            this.m_Window.EventListSelect += this.OnChangeSelection;
            InstallManager.EventChange += this.OnChangeInstall;

            string selectionID = this.m_Window.Selection != null 
                ? this.m_Window.Selection.Data.ID 
                : string.Empty;
            
            this.OnChangeSelection(selectionID);
        }

        ~InstallerElementInstall()
        {
            if (this.m_Window != null) this.m_Window.EventListSelect -= this.OnChangeSelection;
            InstallManager.EventChange -= this.OnChangeInstall;
        }

        // CALLBACKS: -----------------------------------------------------------------------------
        
        private void OnClick(ClickEvent clickEvent)
        {
            this.m_Window.SelectInstallAsset(this.m_Asset);
        }

        private void OnChangeSelection(string assetID)
        {
            if (assetID == this.m_Asset.Data.ID)
            {
                this.RemoveFromClassList(CLASS_INACTIVE);
                this.AddToClassList(CLASS_ACTIVE);
            }
            else
            {
                this.RemoveFromClassList(CLASS_ACTIVE);
                this.AddToClassList(CLASS_INACTIVE);
            }
        }
        
        private void OnChangeInstall(string assetID)
        {
            this.Refresh();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Refresh()
        {
            if (this.m_Asset == null) return;
            
            this.m_LabelName.text = this.m_Asset.Data.Name;

            this.m_LabelVers.text = InstallManager.IsInstalled(this.m_Asset.Data.ID)
                ? InstallManager.GetInstalledVersion(this.m_Asset.Data.ID).ToString()
                : string.Empty;

            this.m_ImageType.style.display = DisplayStyle.None;
            if (this.m_Asset.Data.Complexity == Install.ComplexityType.Skin)
            {
                this.m_ImageType.style.display = DisplayStyle.Flex;
                this.m_ImageType.image = TYPE_SKIN.Texture;
                this.m_ImageType.tooltip = TOOLTIP_SKIN;
            }

            if (InstallManager.IsInstalled(this.m_Asset.Data.ID))
            {
                Version inVersion = InstallManager.GetInstalledVersion(this.m_Asset.Data.ID);
                if (inVersion.IsLower(this.m_Asset.Data.Version))
                {
                    this.m_ImageStat.image = STATUS_UPDATE.Texture;
                    this.m_ImageStat.tooltip = "There is a new update for this asset";
                }
                else
                {
                    this.m_ImageStat.image = STATUS_INSTALLED.Texture;
                    this.m_ImageStat.tooltip = "This asset is installed and up to date";
                }
            }
            else
            {
                this.m_ImageStat.image = STATUS_MISSING.Texture;
                this.m_ImageStat.tooltip = "This asset not installed";
            }
        }
    }
}