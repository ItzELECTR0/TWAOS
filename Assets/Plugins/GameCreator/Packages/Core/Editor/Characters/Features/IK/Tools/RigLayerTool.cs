using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    public class RigLayerTool : TPolymorphicItemTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/RigLayer"
        };

        protected override object Value => this.m_Property.GetValue<TRig>();
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public RigLayerTool(IPolymorphicListTool parentTool, int index)
            : base(parentTool, index)
        { }
    }
}