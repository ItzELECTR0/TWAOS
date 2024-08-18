using System.Collections.Generic;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    public class MemoryTool : TPolymorphicItemTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.COMMON + "Save/StyleSheets/Memory-Head",
            EditorPaths.COMMON + "Save/StyleSheets/Memory-Body"
        };
        
        protected override object Value => this.m_Property.GetValue<Memory>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MemoryTool(IPolymorphicListTool parentTool, int index) 
            : base(parentTool, index)
        { }
    }
}