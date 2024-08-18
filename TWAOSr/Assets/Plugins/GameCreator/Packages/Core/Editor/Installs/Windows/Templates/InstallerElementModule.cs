using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    internal class InstallerElementModule : VisualElement
    {
        private const string KEY_EXPANDED = "gc-installs:expanded:{0}";

        private const string NAME_ROOT = "GC-Install-Content-List-Module-Root";
        private const string NAME_HEAD = "GC-Install-Content-List-Module-Head";
        private const string NAME_BODY = "GC-Install-Content-List-Module-Body";
        
        private const string NAME_ICON = "GC-Install-Content-List-Module-Icon";
        private const string NAME_NAME = "GC-Install-Content-List-Module-Name";

        private static readonly IIcon DROP_RIGHT = new IconArrowDropRight(ColorTheme.Type.TextLight);
        private static readonly IIcon DROP_DOWN = new IconArrowDropDown(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly InstallerManagerWindow m_Window;
        
        private readonly VisualElement m_Root;
        private readonly VisualElement m_Head;
        private readonly VisualElement m_Body;
        
        private readonly string m_Name;
        private readonly List<Installer> m_Assets;
        
        private readonly Image m_ImageIcon;
        private readonly Label m_LabelName;

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool IsExpanded
        {
            get => EditorPrefs.GetBool(string.Format(KEY_EXPANDED, this.m_Name), true);
            set => EditorPrefs.SetBool(string.Format(KEY_EXPANDED, this.m_Name), value);
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstallerElementModule(InstallerManagerWindow window, string name, List<Installer> assets)
        {
            this.m_Window = window;
            
            this.m_Name = name;
            this.m_Assets = assets;

            this.m_Root = new VisualElement { name = NAME_ROOT };
            this.m_Head = new VisualElement { name = NAME_HEAD };
            this.m_Body = new VisualElement { name = NAME_BODY };
            
            this.Add(this.m_Root);
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);

            this.m_ImageIcon = new Image { name = NAME_ICON };
            this.m_LabelName = new Label { name = NAME_NAME };

            this.m_Head.Add(this.m_ImageIcon);
            this.m_Head.Add(this.m_LabelName);

            this.Refresh();

            this.m_Head.RegisterCallback<ClickEvent>(this.OnClick);

            this.m_Window.EventListSelect += this.OnChangeSelection;
        }

        ~InstallerElementModule()
        {
            if (this.m_Window != null) this.m_Window.EventListSelect -= this.OnChangeSelection;
        }

        // CALLBACKS: -----------------------------------------------------------------------------
        
        private void OnClick(ClickEvent clickEvent)
        {
            this.IsExpanded = !this.IsExpanded;
            this.Refresh();
        }
        
        private void OnChangeSelection(string assetID)
        {
            this.Refresh();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Refresh()
        {
            this.m_LabelName.text = this.m_Name;
            this.m_ImageIcon.image = this.IsExpanded ? DROP_DOWN.Texture : DROP_RIGHT.Texture;
            
            this.m_Body.Clear();
            foreach (Installer installerAsset in this.m_Assets)
            {
                if (installerAsset == null) continue;

                string selectionID = this.m_Window.Selection != null
                    ? this.m_Window.Selection.Data.ID
                    : string.Empty;
                
                if (selectionID != installerAsset.Data.ID && !this.IsExpanded) continue;

                InstallerElementInstall install = new InstallerElementInstall(
                    this.m_Window, 
                    installerAsset
                );
                
                this.m_Body.Add(install);
            }
        }
    }
}