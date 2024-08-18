using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class IndexListTool : TPolymorphicListTool
    {
        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/IndexList";

        private const string PROP_TYPE_ID = "m_TypeID";
        
        private const string CLASS_HEAD_BUTTON = "gc-variables-index-button";
        private const string CLASS_HEAD_DROPDOWN = "gc-variables-index-dropdown";

        private const string NAME_DROPZONE = "GC-ListIndex-List-Head-DropZone";

        private static readonly IIcon ICON_DROPDOWN = new IconDropdown(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_COLLAPSE = new IconCollapse(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_ADD = new IconPlus(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_DROP = new IconSquareOutline(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly IndexList m_IndexList;
        private readonly SerializedProperty m_PropertyTypeID;

        private ListTypeElement m_ChangeTypeDropdown;
        private Button m_ChangeTypeButton;
        private Label m_ChangeTypeText;
        private Image m_ChangeTypeIcon;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameHead => "GC-ListIndex-List-Head";
        protected override string ElementNameBody => "GC-ListIndex-List-Body";
        protected override string ElementNameFoot => "GC-ListIndex-List-Foot";
        
        public IndexList IndexList => this.m_Property.GetValue<IndexList>();
        public VisualElement DropZone { get; private set; }
        
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

        public IndexListTool(SerializedProperty propertyRoot)
            : base(propertyRoot, "m_Source")
        {
            this.SerializedObject.Update();
            
            this.m_PropertyTypeID = propertyRoot.FindPropertyRelative(PROP_TYPE_ID);
            this.m_IndexList = propertyRoot.GetValue<IndexList>();

            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) this.styleSheets.Add(styleSheet);
        }
        
        protected override void SetupHead()
        {
            base.SetupHead();
            
            this.DropZone = new VisualElement { name = NAME_DROPZONE };
            this.DropZone.Add(new Image { image = ICON_DROP.Texture });
            
            ManipulatorDropToListVariables dropSelection = new ManipulatorDropToListVariables(this);
            this.DropZone.AddManipulator(dropSelection);
            
            Button btnToggle = new Button(this.Collapse);
            btnToggle.AddToClassList(CLASS_HEAD_BUTTON);
            btnToggle.Add(new Image { image = ICON_COLLAPSE.Texture });

            this.m_ChangeTypeButton = new Button();
            this.m_ChangeTypeButton.AddToClassList(CLASS_HEAD_DROPDOWN);
            
            this.m_ChangeTypeIcon = new Image();
            this.m_ChangeTypeText = new Label();

            this.m_ChangeTypeButton.Add(this.m_ChangeTypeIcon);
            this.m_ChangeTypeButton.Add(this.m_ChangeTypeText);
            this.m_ChangeTypeButton.Add(new FlexibleSpace());
            this.m_ChangeTypeButton.Add(new Image { image = ICON_DROPDOWN.Texture });
            
            this.RefreshChangeType();
            this.m_ChangeTypeDropdown = new ListTypeElement(
                this.m_ChangeTypeButton,
                this.ChangeValueType
            );

            Button btnAdd = new Button(() => { this.AddVariable(this.PropertyList.arraySize); });
            btnAdd.AddToClassList(CLASS_HEAD_BUTTON);
            btnAdd.Add(new Image { image = ICON_ADD.Texture });

            this.m_Head.Add(this.DropZone);
            this.m_Head.Add(btnToggle);
            this.m_Head.Add(this.m_ChangeTypeButton);
            this.m_Head.Add(btnAdd);
        }

        private void RefreshChangeType()
        {
            string stringTypeID = this.m_Property
                .FindPropertyRelative(PROP_TYPE_ID)
                .FindPropertyRelative(IdStringDrawer.NAME_STRING).stringValue;
            
            IdString typeID = new IdString(stringTypeID);
            Type type = TValue.GetType(typeID);
            
            this.m_ChangeTypeText.text = TypeUtils.GetTitleFromType(type);
            this.m_ChangeTypeIcon.image = type?.GetCustomAttributes<ImageAttribute>()
                .FirstOrDefault()?.Image;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            IndexVariableTool item = new IndexVariableTool(this, index);
            return item;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        private void AddVariable(int insertIndex)
        {
            IndexVariable variable = new IndexVariable(this.m_IndexList.TypeID);
            this.InsertItem(insertIndex, variable);
        }

        private void ChangeValueType(Type newType)
        {
            this.SerializedObject.Update();
            
            IdString newTypeID = TValue.GetTypeIDFromValueType(newType);
            this.m_PropertyTypeID
                .FindPropertyRelative(IdStringDrawer.NAME_STRING)
                .stringValue = newTypeID.String;
        
            int count = this.PropertyList.arraySize;
        
            for (int i = 0; i < count; ++i)
            {
                SerializedProperty propertyItem = this.PropertyList.GetArrayElementAtIndex(i);
                SerializedProperty propertyItemValue = propertyItem.FindPropertyRelative("m_Value");
        
                if (this.IndexList.Get(i).TypeID.Hash == newTypeID.Hash) continue;
                
                TValue value = TValue.CreateValue(newTypeID);
                propertyItemValue.SetValue(value);
            }
            
            SerializationUtils.ApplyUnregisteredSerialization(this.SerializedObject);
            this.Refresh();
            this.RefreshChangeType();
        }

        public void FillWith(UnityEngine.Object[] references)
        {
            if (references.Length <= 0) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            Type objectType = references[0]?.GetType();
            IdString typeID = TValue.GetTypeIDFromObjectType(objectType);
            if (typeID == ValueNull.TYPE_ID) return;

            Type valueType = TValue.GetType(typeID);
            this.ChangeValueType(valueType);
            
            List<object> variables = new List<object>();
            foreach (UnityEngine.Object reference in references)
            {
                TValue value = TValue.CreateValue(typeID, reference);
                if (value == null) continue;

                IndexVariable variable = new IndexVariable(value);
                variables.Add(variable);
            }
            
            this.FillItems(variables);
        }
    }
}