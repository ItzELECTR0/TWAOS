using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    public class SpotListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Spot-List-Foot-Add";
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameHead => "GC-Spot-List-Head";
        protected override string ElementNameBody => "GC-Spot-List-Body";
        protected override string ElementNameFoot => "GC-Spot-List-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Spots/StyleSheets/Hotspot"
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

        public SpotListTool(SerializedProperty property)
            : base(property, "m_Spots")
        {
            this.SerializedObject.Update();

            EditorApplication.playModeStateChanged += this.OnChangePlayMode;

            this.OnChangePlayMode(EditorApplication.isPlaying
                ? PlayModeStateChange.EnteredPlayMode
                : PlayModeStateChange.ExitingPlayMode
            );
        }
        
        ~SpotListTool()
        {
            EditorApplication.playModeStateChanged -= this.OnChangePlayMode;
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new SpotItemTool(this, index);
        }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementSpot(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };

            this.m_Foot.Add(this.m_ButtonAdd);
        }
        
        protected void OnChangePlayMode(PlayModeStateChange state)
        { }
    }
}