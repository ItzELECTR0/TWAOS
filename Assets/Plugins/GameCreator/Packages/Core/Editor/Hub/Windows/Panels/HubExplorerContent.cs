using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public class HubExplorerContent : VisualElement
    {
        private const int FIXED_PANEL_INDEX = 0;
        private const float FIXED_PANEL_WIDTH = 350f;

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly HubExplorerWindow m_Window;
        
        private TwoPaneSplitView m_SplitView;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public HubExplorerContentList ContentList { get; private set; }
        public HubExplorerContentDetails ContentDetails { get; private set; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public HubExplorerContent(HubExplorerWindow window)
        {
            this.m_Window = window;
        }
        
        public void OnEnable()
        {
            this.m_SplitView = new TwoPaneSplitView(
                FIXED_PANEL_INDEX,
                FIXED_PANEL_WIDTH,
                TwoPaneSplitViewOrientation.Horizontal
            );

            this.Add(this.m_SplitView);
            
            this.ContentList = new HubExplorerContentList(this.m_Window);
            this.ContentDetails = new HubExplorerContentDetails(this.m_Window);

            this.m_SplitView.Add(this.ContentList);
            this.m_SplitView.Add(this.ContentDetails);
            
            this.ContentList.OnEnable();
            this.ContentDetails.OnEnable();
        }
        
        public void OnDisable()
        {
            this.ContentList?.OnDisable();
            this.ContentDetails?.OnDisable();
        }
    }
}