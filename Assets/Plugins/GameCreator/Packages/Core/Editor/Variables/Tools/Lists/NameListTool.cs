using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class NameListTool : TPolymorphicListTool
    {
        private const string DEFAULT_INPUT_TEXT = "Type variable name...";
        
        private const string CLASS_HEAD_BUTTON = "gc-variables-name-button";
        private const string CLASS_HEAD_TEXTFIELD = "gc-variables-name-textfield";

        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/NameList";
        
        private static readonly IIcon ICON_COLLAPSE = new IconCollapse(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_ADD = new IconPlus(ColorTheme.Type.TextNormal);

        public const string NAME_HEAD = "GC-ListName-List-Head";
        public const string NAME_BODY = "GC-ListName-List-Body";
        public const string NAME_FOOT = "GC-ListName-List-Foot";

        // MEMBERS: -------------------------------------------------------------------------------

        private TextField m_TextFieldAdd;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameHead => NAME_HEAD;
        protected override string ElementNameBody => NAME_BODY;
        protected override string ElementNameFoot => NAME_FOOT;

        public NameList NameList => this.m_Property.GetValue<NameList>();
        
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

        public NameListTool(SerializedProperty propertyRoot)
            : base(propertyRoot, "m_Source")
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) this.styleSheets.Add(styleSheet);
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            NameVariableTool item = new NameVariableTool(this, index);
            return item;
        }

        protected override void SetupHead()
        {
            base.SetupHead();

            Button btnToggle = new Button();
            btnToggle.AddToClassList(CLASS_HEAD_BUTTON);
            btnToggle.Add(new Image { image = ICON_COLLAPSE.Texture });
            btnToggle.clicked += this.Collapse;

            this.m_TextFieldAdd = new TextField { value = DEFAULT_INPUT_TEXT };
            this.m_TextFieldAdd.AddToClassList(CLASS_HEAD_TEXTFIELD);
            this.m_TextFieldAdd.RegisterValueChangedCallback(changeEvent =>
            {
                this.m_TextFieldAdd.value = TextUtils.ProcessID(changeEvent.newValue);
            });

            Button btnAdd = new Button(() => { this.AddVariable(this.PropertyList.arraySize); });
            btnAdd.AddToClassList(CLASS_HEAD_BUTTON);
            btnAdd.Add(new Image { image = ICON_ADD.Texture });

            this.m_Head.Add(btnToggle);
            this.m_Head.Add(this.m_TextFieldAdd);
            this.m_Head.Add(btnAdd);

            this.m_TextFieldAdd.RegisterCallback((KeyDownEvent keyDownEvent) =>
            {
                if (keyDownEvent.keyCode != KeyCode.Return) return;
                if (string.IsNullOrEmpty(this.m_TextFieldAdd.value)) return;

                this.AddVariable(this.PropertyList.arraySize);
                this.m_TextFieldAdd.Focus();
            });
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        private void AddVariable(int insertIndex)
        {
            NameVariable variable = new NameVariable(
                TextUtils.ProcessID(this.m_TextFieldAdd?.value),
                new ValueNull()
            );

            this.InsertItem(insertIndex, variable);
        }
    }
}