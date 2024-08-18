using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    public class BranchListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Branch-List-Foot-Add";
        
        private static readonly IIcon ICON_ADD = new IconBranch(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Branch-List-Head";
        protected override string ElementNameBody => "GC-Branch-List-Body";
        protected override string ElementNameFoot => "GC-Branch-List-Foot";
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Conditions/StyleSheets/Branch-List"
        };

        public override bool AllowReordering => true;
        public override bool AllowDuplicating => true;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => true;
        public override bool AllowCopyPaste => false;
        public override bool AllowInsertion => false;
        public override bool AllowBreakpoint => true;
        public override bool AllowDisable => true;
        public override bool AllowDocumentation => false;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public BranchListTool(SerializedProperty property)
            : base(property, "m_Branches")
        { }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new BranchItemTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();

            this.m_ButtonAdd = new Button(() =>
            {
                int insertIndex = this.PropertyList.arraySize;
                this.InsertItem(insertIndex, new Branch());
            });

            this.m_ButtonAdd.name = NAME_BUTTON_ADD;
            this.m_ButtonAdd.Add(new Image { image = ICON_ADD.Texture });
            this.m_ButtonAdd.Add(new Label { text = "Add Branch..." });

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}