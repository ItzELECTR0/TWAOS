using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    public class HandleItemTool : TPolymorphicItemTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Handle-Head",
            EditorPaths.CHARACTERS + "StyleSheets/Handle-Body"
        };
        
        protected override object Value => this.m_Property.GetValue<HandleItem>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public HandleItemTool(IPolymorphicListTool parentTool, int index) 
            : base(parentTool, index)
        { }
    }
}