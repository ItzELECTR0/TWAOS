using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;

namespace GameCreator.Editor.Variables
{
    public class NameListView : TListView<NameVariableRuntime>
    {
        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/NameList";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string USSPath => USS_PATH;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public NameListView(NameVariableRuntime runtime) : base(runtime)
        {
            runtime.EventChange += this.OnChange;
        }

        private void OnChange(string name)
        {
            this.Refresh();
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------

        protected override void Refresh()
        {
            base.Refresh();
            if (this.m_Runtime?.GetEnumerator() == null) return;
            
            foreach (NameVariable variable in this.m_Runtime)
            {
                this.Add(new NameVariableView(variable));
            }
        }
    }
}