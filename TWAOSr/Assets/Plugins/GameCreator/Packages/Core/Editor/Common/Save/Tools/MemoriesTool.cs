using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class MemoriesTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Memories-Foot-Add";

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Memories-Head";
        protected override string ElementNameBody => "GC-Memories-Body";
        protected override string ElementNameFoot => "GC-Memories-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.COMMON + "Save/StyleSheets/Memories"
        };
        
        public override bool AllowReordering => true;
        public override bool AllowDuplicating => false;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => true;
        public override bool AllowCopyPaste => false;
        public override bool AllowInsertion => true;
        public override bool AllowBreakpoint => false;
        public override bool AllowDisable => true;
        public override bool AllowDocumentation => true;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MemoriesTool(SerializedProperty property) : base(property, "m_Memories")
        {
            this.SerializedObject.Update();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new MemoryTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementMemory(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}