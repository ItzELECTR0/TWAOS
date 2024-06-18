using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class RigLayersTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Rig-Layers-Foot-Add";

        // MEMBERS: -------------------------------------------------------------------------------

        private Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Rig-Layers-Head";
        protected override string ElementNameBody => "GC-Rig-Layers-Body";
        protected override string ElementNameFoot => "GC-Rig-Layers-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/RigLayers"
        };
        
        public override bool AllowReordering => true;
        public override bool AllowDuplicating => true;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => true;
        public override bool AllowCopyPaste => true;
        public override bool AllowInsertion => true;
        public override bool AllowBreakpoint => false;
        public override bool AllowDisable => true;
        public override bool AllowDocumentation => true;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public RigLayersTool(SerializedProperty property)
            : base(property, "m_Rigs")
        {
            this.SerializedObject.Update();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new RigLayerTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementRigLayer(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}