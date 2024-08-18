using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    public class SpotItemTool : TPolymorphicItemTool
    {
        public SpotItemTool(IPolymorphicListTool parentTool, int index) 
            : base(parentTool, index)
        { }
        
        protected override object Value => this.m_Property.GetValue<Spot>();
    }
}
