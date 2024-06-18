using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    public class InstructionItemTool : TPolymorphicItemTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Instructions/StyleSheets/Instruction-Head",
            EditorPaths.VISUAL_SCRIPTING + "Instructions/StyleSheets/Instruction-Body"
        };

        protected override object Value => this.m_Property.GetValue<Instruction>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstructionItemTool(IPolymorphicListTool parentTool, int index)
            : base(parentTool, index)
        { }
    }
}
