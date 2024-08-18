using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Editor.VisualScripting
{
    public class InstructionListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Instruction-List-Foot-Add";
        
        private const string CLASS_INSTRUCTION_RUNNING = "gc-list-item-head-running";

        private static readonly IIcon ICON_PASTE = new IconPaste(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_PLAY = new IconPlay(ColorTheme.Type.TextNormal);

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] protected Button m_ButtonAdd;
        [NonSerialized] protected Button m_ButtonPaste;
        [NonSerialized] protected Button m_ButtonPlay;

        [NonSerialized] private readonly BaseActions m_BaseActions;
        [NonSerialized] private IVisualElementScheduledItem m_UpdateScheduler;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Instruction-List-Head";
        protected override string ElementNameBody => "GC-Instruction-List-Body";
        protected override string ElementNameFoot => "GC-Instruction-List-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Instructions/StyleSheets/Instructions-List"
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

        public InstructionListTool(SerializedProperty property)
            : base(property, InstructionListDrawer.NAME_INSTRUCTIONS)
        {
            this.m_BaseActions = property.serializedObject.targetObject as BaseActions;
            
            this.RegisterCallback<AttachToPanelEvent>(this.OnAttachPanel);
            this.RegisterCallback<DetachFromPanelEvent>(this.OnDetachPanel);
        }
        
        // SCHEDULER METHODS: ---------------------------------------------------------------------

        private void OnAttachPanel(AttachToPanelEvent attachEvent)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (this.m_BaseActions == null) return;
            
            if (this.m_UpdateScheduler != null) return;
            this.m_UpdateScheduler = this.schedule.Execute(this.OnUpdate).Every(0);
        }

        private void OnDetachPanel(DetachFromPanelEvent detachEvent)
        {
            this.m_UpdateScheduler?.Pause();
        }
        
        private void OnUpdate()
        {
            if (this.m_Property.propertyPath != BaseActionsEditor.NAME_INSTRUCTIONS) return;
            
            foreach (VisualElement child in this.m_Body.Children())
            {
                child.RemoveFromClassList(CLASS_INSTRUCTION_RUNNING);
            }
            
            if (this.m_BaseActions == null) return;
            int index = this.m_BaseActions.IsRunning ? this.m_BaseActions.RunningIndex : -1;
            
            if (this.m_Body.childCount <= index || index < 0) return;
            this.m_Body[index].AddToClassList(CLASS_INSTRUCTION_RUNNING);
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new InstructionItemTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementInstruction(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };
            
            this.m_ButtonPaste = new Button(() =>
            {
                if (!CopyPasteUtils.CanSoftPaste(typeof(Instruction))) return;
                
                int pasteIndex = this.PropertyList.arraySize;
                this.InsertItem(pasteIndex, CopyPasteUtils.SourceObjectCopy);
            })
            {
                name = "GC-Instruction-List-Foot-Button"
            };
            
            this.m_ButtonPaste.Add(new Image
            {
                image = ICON_PASTE.Texture
            });
            
            this.m_ButtonPlay = new Button(this.RunInstructions)
            {
                name = "GC-Instruction-List-Foot-Button"
            };
            
            this.m_ButtonPlay.Add(new Image
            {
                image = ICON_PLAY.Texture
            });
            
            this.m_Foot.Add(this.m_ButtonAdd);
            this.m_Foot.Add(this.m_ButtonPaste);
            this.m_Foot.Add(this.m_ButtonPlay);

            this.m_ButtonPlay.SetEnabled(EditorApplication.isPlayingOrWillChangePlaymode);
            this.m_ButtonPlay.style.display = this.SerializedObject?.targetObject as BaseActions != null
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RunInstructions()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (this.m_BaseActions == null) return;
            
            this.m_BaseActions.Invoke(this.m_BaseActions.gameObject);
        }
    }
}