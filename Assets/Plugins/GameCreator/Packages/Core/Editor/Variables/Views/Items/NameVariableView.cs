using GameCreator.Runtime.Variables;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class NameVariableView : TVariableView<NameVariable>
    {
        private const string LABEL_NAME = "Name";

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Title => this.m_Variable.Title;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public NameVariableView(NameVariable variable) : base(variable)
        {
            this.SetupHead();
            this.SetupBody();
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------
        
        protected override VisualElement MakeBody()
        {
            VisualElement container = new VisualElement();
            container.Add(new TextField(LABEL_NAME)
            {
                value = this.m_Variable.Name
            });
            
            this.GetFieldValue(container);
            return container;
        }
    }
}