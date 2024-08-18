using System.Collections.Generic;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    public class InstallerContentList : VisualElement
    {
        private const string LIST_NAME = "GC-Install-Content-List";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly InstallerManagerWindow m_Window;

        private Label m_SearchingLabel;
        private ProgressBar m_SearchingProgress;
        
        private ScrollView m_ScrollView;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public InstallerContentList(InstallerManagerWindow window)
        {
            this.m_Window = window;
        }

        internal void OnEnable()
        {
            this.m_ScrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                name = LIST_NAME
            };
            
            this.Add(this.m_ScrollView);
        }

        internal void OnDisable()
        {
            
        }
        
        internal void Refresh()
        {
            this.m_ScrollView.contentContainer.Clear();
            foreach (KeyValuePair<string,List<Installer>> entry in this.m_Window.InstallAssetsMap)
            {
                this.MakeModule(entry.Key, entry.Value);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void MakeModule(string name, List<Installer> assets)
        {
            InstallerElementModule module = new InstallerElementModule(
                this.m_Window,
                name, 
                assets
            );
            
            this.m_ScrollView.contentContainer.Add(module);
        }
    }
}