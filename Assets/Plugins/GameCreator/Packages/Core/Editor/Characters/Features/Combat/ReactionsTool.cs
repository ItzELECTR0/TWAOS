using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class ReactionsTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Reactions-Foot-Add";
        private static readonly IIcon ICON_ADD = new IconReaction(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Reactions-Head";
        protected override string ElementNameBody => "GC-Reactions-Body";
        protected override string ElementNameFoot => "GC-Reactions-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Reactions"
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

        public ReactionsTool(SerializedProperty property) : base(property, "m_List")
        {
            this.SerializedObject.Update();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new ReactionTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();

            this.m_ButtonAdd = new Button { name = NAME_BUTTON_ADD };

            this.m_ButtonAdd.Add(new Image { image = ICON_ADD.Texture });
            this.m_ButtonAdd.Add(new Label { text = "Add Reaction..." });

            this.m_ButtonAdd.clicked += () =>
            {
                this.SerializedObject.Update();
            
                int insertIndex = this.PropertyList.arraySize;
                this.PropertyList.InsertArrayElementAtIndex(insertIndex);
                this.PropertyList
                    .GetArrayElementAtIndex(insertIndex)
                    .SetValue(new ReactionItem());

                SerializationUtils.ApplyUnregisteredSerialization(this.SerializedObject);

                int size = this.PropertyList.arraySize;
                this.ExecuteEventChangeSize(size);
            
                using ChangeEvent<int> changeEvent = ChangeEvent<int>.GetPooled(size, size);
                changeEvent.target = this;
                this.SendEvent(changeEvent);
            
                this.Refresh();
            };

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}