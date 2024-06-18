using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class VolumesTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Volumes-Foot-Add";

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Volumes-Head";
        protected override string ElementNameBody => "GC-Volumes-Body";
        protected override string ElementNameFoot => "GC-Volumes-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Volumes"
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

        public VolumesTool(SerializedProperty property) : base(property, "m_Volumes")
        {
            this.SerializedObject.Update();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new VolumeTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementVolume(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}