using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;

namespace GameCreator.Editor.Variables
{
    public class IndexListView : TListView<ListVariableRuntime>
    {
        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/IndexList";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string USSPath => USS_PATH;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public IndexListView(ListVariableRuntime runtime) : base(runtime)
        {
            runtime.EventChange += this.OnChange;
        }

        private void OnChange(ListVariableRuntime.Change change, int index)
        {
            this.Refresh();
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------

        protected override void Refresh()
        {
            base.Refresh();
            if (this.m_Runtime?.GetEnumerator() == null) return;

            int index = 0;
            foreach (IndexVariable variable in this.m_Runtime)
            {
                this.Add(new IndexVariableView(index, variable));
                index += 1;
            }
        }
    }
}