using GameCreator.Runtime.Variables;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class IndexVariableView : TVariableView<IndexVariable>
    {
        private readonly int m_Index;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Title => $"{this.m_Index}: {this.m_Variable.Title}";
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public IndexVariableView(int index, IndexVariable variable) : base(variable)
        {
            this.m_Index = index;
            
            this.SetupHead();
            this.SetupBody();
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------
        
        protected override VisualElement MakeBody()
        {
            VisualElement container = new VisualElement();
            
            this.GetFieldValue(container);
            return container;
        }
    }
}