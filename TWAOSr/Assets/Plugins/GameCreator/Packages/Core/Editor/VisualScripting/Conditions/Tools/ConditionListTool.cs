using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    public class ConditionListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Condition-List-Foot-Add";

        private static readonly IIcon ICON_PASTE = new IconPaste(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_PLAY = new IconPlay(ColorTheme.Type.TextNormal);

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;
        protected Button m_ButtonPaste;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Condition-List-Head";
        protected override string ElementNameBody => "GC-Condition-List-Body";
        protected override string ElementNameFoot => "GC-Condition-List-Foot";
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Conditions/StyleSheets/Condition-List"
        };
        
        public override bool AllowReordering => true;
        public override bool AllowDuplicating => true;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => true;
        public override bool AllowCopyPaste => true;
        public override bool AllowInsertion => true;
        public override bool AllowBreakpoint => true;
        public override bool AllowDisable => true;
        public override bool AllowDocumentation => true;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ConditionListTool(SerializedProperty property)
            : base(property, ConditionListDrawer.NAME_CONDITIONS)
        { }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new ConditionItemTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementCondition(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };
            
            this.m_ButtonPaste = new Button(() =>
            {
                if (!CopyPasteUtils.CanSoftPaste(typeof(Condition))) return;
                
                int pasteIndex = this.PropertyList.arraySize;
                this.InsertItem(pasteIndex, CopyPasteUtils.SourceObjectCopy);
            })
            {
                name = "GC-Condition-List-Foot-Button"
            };
            
            this.m_ButtonPaste.Add(new Image
            {
                image = ICON_PASTE.Texture
            });

            this.m_Foot.Add(this.m_ButtonAdd);
            this.m_Foot.Add(this.m_ButtonPaste);
        }
    }
}