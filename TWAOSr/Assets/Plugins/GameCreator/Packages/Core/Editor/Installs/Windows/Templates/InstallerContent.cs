using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    public class InstallerContent : VisualElement
    {
        private const int FIXED_PANEL_INDEX = 0;
        private const float FIXED_PANEL_WIDTH = 350f;

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly InstallerManagerWindow m_Window;
        
        private TwoPaneSplitView m_SplitView;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public InstallerContentList ContentList { get; private set; }
        public InstallerContentDetails ContentDetails { get; private set; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstallerContent(InstallerManagerWindow window)
        {
            this.m_Window = window;
        }
        
        internal void OnEnable()
        {
            this.m_SplitView = new TwoPaneSplitView(
                FIXED_PANEL_INDEX,
                FIXED_PANEL_WIDTH,
                TwoPaneSplitViewOrientation.Horizontal
            );

            this.Add(this.m_SplitView);
            
            this.ContentList = new InstallerContentList(this.m_Window);
            this.ContentDetails = new InstallerContentDetails(this.m_Window);

            this.m_SplitView.Add(this.ContentList);
            this.m_SplitView.Add(this.ContentDetails);
            
            this.ContentList.OnEnable();
            this.ContentDetails.OnEnable();
        }
        
        internal void OnDisable()
        {
            this.ContentList?.OnDisable();
            this.ContentDetails?.OnDisable();
        }
        
        internal void Refresh()
        {
            this.ContentList?.Refresh();
            this.ContentDetails?.Refresh();
        }
    }
}