using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class HandleListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Handles-Foot-Add";

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Handles-Head";
        protected override string ElementNameBody => "GC-Handles-Body";
        protected override string ElementNameFoot => "GC-Handles-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Handles"
        };

        public override bool AllowReordering => true;
        public override bool AllowDuplicating => true;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => false;
        public override bool AllowCopyPaste => false;
        public override bool AllowInsertion => false;
        public override bool AllowBreakpoint => false;
        public override bool AllowDisable => false;
        public override bool AllowDocumentation => false;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public HandleListTool(SerializedProperty property) : base(property, "m_Handles")
        {
            this.SerializedObject.Update();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new HandleItemTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementHandle(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}