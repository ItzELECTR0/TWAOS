using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    public class InstallerContentDetails : VisualElement
    {
        private const string NAME_INTERACTIONS = "GC-Install-Content-Details-Interactions";
        private const string NAME_SCROLLVIEW = "GC-Install-Content-Details-ScrollView";
        private const string NAME_CONTENT = "GC-Install-Content-Details-Content";

        private const string NAME_HEADER = "GC-Install-Content-Details-Header";
        private const string NAME_COMPLEXITY = "GC-Install-Content-Details-Complexity";
        private const string NAME_TITLE = "GC-Install-Content-Details-Title";
        private const string NAME_AUTHOR = "GC-Install-Content-Details-Author";
        private const string NAME_VERSION = "GC-Install-Content-Details-Version";
        private const string NAME_DESCRIPTION = "GC-Install-Content-Details-Description";
        private const string NAME_DEPENDENCIES = "GC-Install-Content-Details-Dependencies";
        
        private const string CLASS_DEPENDENCY = "gc-install-content-details-dependency";

        private const string CLASS_COMPLEXITY_START_HERE = "gc-install-complexity-start-here";
        private const string CLASS_COMPLEXITY_BEGINNER = "gc-install-complexity-beginner";
        private const string CLASS_COMPLEXITY_INTERMEDIATE = "gc-install-complexity-intermediate";
        private const string CLASS_COMPLEXITY_ADVANCED = "gc-install-complexity-advanced";
        private const string CLASS_COMPLEXITY_SKIN = "gc-install-complexity-skin";
        private const string CLASS_COMPLEXITY_NONE = "gc-install-complexity-none";
        
        private static readonly IIcon STATUS_IS_INSTALLED = new IconCircleSolid(ColorTheme.Type.Green);
        private static readonly IIcon STATUS_NEEDS_UPDATE = new IconCircleOutline(ColorTheme.Type.Yellow);
        private static readonly IIcon STATUS_NO_INSTALLED = new IconCircleOutline(ColorTheme.Type.Red);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly InstallerManagerWindow m_Window;
        
        private ScrollView m_ScrollView;
        private VisualElement m_Content;
        private VisualElement m_Toolbar;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public InstallerContentDetails(InstallerManagerWindow window)
        {
            this.m_Window = window;
        }

        internal void OnEnable()
        {
            this.m_Window.EventListSelect += this.OnChangeSelection;
            InstallManager.EventChange += this.OnChangeInstall;
        }

        internal void OnDisable()
        {
            this.m_Window.EventListSelect -= this.OnChangeSelection;
            InstallManager.EventChange -= this.OnChangeInstall;
        }
        
        internal void Refresh()
        {
            this.RefreshContent(this.m_Window.Selection != null
                ? this.m_Window.Selection.Data.ID
                : string.Empty
            );
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnChangeInstall(string assetID)
        {
            this.RefreshContent(assetID);
        }
        
        private void OnChangeSelection(string assetID)
        {
            this.RefreshContent(assetID);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshContent(string assetID)
        {
            this.Clear();

            Installer asset = this.m_Window.Selection;
            if (asset == null) return;
            
            this.m_ScrollView = new ScrollView(ScrollViewMode.Vertical) { name = NAME_SCROLLVIEW };
            this.Add(this.m_ScrollView);

            this.m_Content = new VisualElement { name = NAME_CONTENT };
            this.m_ScrollView.contentContainer.Add(this.m_Content);

            this.m_Toolbar = new VisualElement { name = NAME_INTERACTIONS };
            this.Add(this.m_Toolbar);

            this.SetupInformation(asset);

            bool isInstalled = InstallManager.IsInstalled(assetID);
            Version installedVersion = isInstalled 
                ? InstallManager.GetInstalledVersion(assetID) 
                : Version.Zero;
            
            Button buttonSelect = new Button { text = "Select" };
            buttonSelect.SetEnabled(isInstalled);
            buttonSelect.clicked += () =>
            {
                if (!isInstalled) return;

                EditorUtility.FocusProjectWindow();
                UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                    InstallManager.GetInstallPath(
                        assetID, 
                        InstallManager.GetInstalledVersion(assetID)
                    )
                );
                
                Selection.activeObject = folder;
                EditorGUIUtility.PingObject(folder);
            };

            Button buttonInstall = new Button
            {
                text = isInstalled
                    ? installedVersion.IsEqual(asset.Data.Version)
                        ? "Up to date"
                        : $"Update to {asset.Data.Version}"
                    : $"Install {asset.Data.Version}"
            };
            
            buttonInstall.AddToClassList("gc-bold");
            buttonInstall.SetEnabled(!isInstalled || !installedVersion.IsEqual(asset.Data.Version));
            buttonInstall.clicked += () => InstallManager.Install(asset);

            Button buttonRemove = new Button { text = "Delete" };
            buttonRemove.clicked += () => InstallManager.Delete(assetID);

            VisualElement spacer = new VisualElement();
            spacer.AddToClassList("gc-spacer");
            
            if (isInstalled) this.m_Toolbar.Add(buttonSelect);
            this.m_Toolbar.Add(spacer);
            this.m_Toolbar.Add(buttonInstall);
            if (isInstalled) this.m_Toolbar.Add(buttonRemove);
        }

        private void SetupInformation(Installer asset)
        {
            if (asset == null) return;

            VisualElement header = new VisualElement { name = NAME_HEADER };
            Label labelTitle = new Label
            {
                text = asset.Data.Name,
                name = NAME_TITLE
            };
            
            Label labelComplexity = new Label
            {
                text = TextUtils.Humanize(asset.Data.Complexity.ToString()),
                name = NAME_COMPLEXITY
            };
            
            labelComplexity.AddToClassList(asset.Data.Complexity switch
            {
                Install.ComplexityType.StartHere => CLASS_COMPLEXITY_START_HERE,
                Install.ComplexityType.Beginner => CLASS_COMPLEXITY_BEGINNER,
                Install.ComplexityType.Intermediate => CLASS_COMPLEXITY_INTERMEDIATE,
                Install.ComplexityType.Advanced => CLASS_COMPLEXITY_ADVANCED,
                Install.ComplexityType.None => CLASS_COMPLEXITY_NONE,
                Install.ComplexityType.Skin => CLASS_COMPLEXITY_SKIN,
                _ => throw new ArgumentOutOfRangeException()
            });

            header.Add(labelTitle);
            if (asset.Data.Complexity != Install.ComplexityType.None)
            {
                header.Add(labelComplexity);
            }
            
            this.m_Content.Add(header);

            Label labelAuthor = new Label
            {
                text = $"Made by {asset.Data.Author}",
                name = NAME_AUTHOR
            };

            string currentVersion = InstallManager.IsInstalled(asset.Data.ID)
                ? InstallManager.GetInstalledVersion(asset.Data.ID).ToString()
                : "Not installed";

            Label labelCurrentVersion = new Label
            {
                text = $"Installed version: {currentVersion}",
                name = NAME_VERSION
            };
            
            Label labelLatestVersion = new Label
            {
                text = $"Latest version: {asset.Data.Version}",
                name = NAME_VERSION
            };

            Label labelDescription = new Label
            {
                text = asset.Data.Description,
                name = NAME_DESCRIPTION
            };
            
            this.m_Content.Add(labelAuthor);
            this.m_Content.Add(labelCurrentVersion);
            this.m_Content.Add(labelLatestVersion);
            this.m_Content.Add(labelDescription);

            if (asset.Data.Dependencies.Length > 0)
            {
                Label labelDependencies = new Label
                {
                    text = "Dependencies:",
                    name = NAME_DEPENDENCIES
                };
                
                this.m_Content.Add(labelDependencies);

                foreach (Dependency dependency in asset.Data.Dependencies)
                {
                    VisualElement dependencyContent = new VisualElement();
                    dependencyContent.AddToClassList(CLASS_DEPENDENCY);

                    Image dependencyIcon = new Image
                    {
                        image = InstallManager.IsInstalled(dependency.ID)
                            ? InstallManager.GetInstalledVersion(dependency.ID)
                                .IsHigherOrEqual(dependency.MinVersion)
                                ? STATUS_IS_INSTALLED.Texture
                                : STATUS_NEEDS_UPDATE.Texture
                            : STATUS_NO_INSTALLED.Texture
                    };

                    Label dependencyTitle = new Label($"{dependency.ID} ({dependency.MinVersion})");
                    
                    dependencyContent.Add(dependencyIcon);
                    dependencyContent.Add(dependencyTitle);

                    this.m_Content.Add(dependencyContent);
                }
            }
        }
    }
}