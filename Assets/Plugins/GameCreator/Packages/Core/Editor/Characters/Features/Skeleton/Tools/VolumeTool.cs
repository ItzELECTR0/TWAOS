using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.Audio;

namespace GameCreator.Editor.Characters
{
    public class VolumeTool : TPolymorphicItemTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Volume-Head",
            EditorPaths.CHARACTERS + "StyleSheets/Volume-Body"
        };
        
        protected override object Value => this.m_Property.GetValue<Volume>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public VolumeTool(IPolymorphicListTool parentTool, int index) 
            : base(parentTool, index)
        { }
    }
}