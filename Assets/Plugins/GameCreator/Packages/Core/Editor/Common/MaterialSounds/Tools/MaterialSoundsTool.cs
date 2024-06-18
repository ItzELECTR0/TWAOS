using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class MaterialSoundsTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-MaterialSounds-Foot-Add";
        
        private static readonly IIcon ICON_MATERIAL = new IconPlus(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------

        private Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-MaterialSounds-Head";
        protected override string ElementNameBody => "GC-MaterialSounds-Body";
        protected override string ElementNameFoot => "GC-MaterialSounds-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.COMMON + "MaterialSounds/StyleSheets/MaterialSounds"
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

        public MaterialSoundsTool(SerializedProperty property) : base(property, "m_MaterialSounds")
        {
            this.SerializedObject.Update();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new MaterialSoundTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();

            this.m_ButtonAdd = new Button(this.CreateMaterialSound)
            {
                name = NAME_BUTTON_ADD
            };
            
            this.m_ButtonAdd.Add(new Image { image = ICON_MATERIAL.Texture });
            this.m_ButtonAdd.Add(new Label("Add Material Sound"));

            this.m_Foot.Add(this.m_ButtonAdd);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void CreateMaterialSound()
        {
            this.InsertItem(this.PropertyList.arraySize, new MaterialSoundTexture());
        }
    }
}