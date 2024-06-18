using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    public class ReactionTool : TPolymorphicItemTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Reaction-Head",
            EditorPaths.CHARACTERS + "StyleSheets/Reaction-Body"
        };
        
        protected override object Value => this.m_Property.GetValue<ReactionItem>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ReactionTool(IPolymorphicListTool parentTool, int index) 
            : base(parentTool, index)
        { }
    }
}