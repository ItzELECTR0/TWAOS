using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class BranchItemTool : TPolymorphicItemTool
    {
        private static readonly IIcon ICON_EDIT = new IconEdit(ColorTheme.Type.TextNormal);
        private const string TIP_EDIT = "Edit the Branch description";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Button m_HeadEdit;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Conditions/StyleSheets/Branch-Head",
            EditorPaths.VISUAL_SCRIPTING + "Conditions/StyleSheets/Branch-Body"
        };

        protected override object Value => this.m_Property.GetValue<Branch>();
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public BranchItemTool(IPolymorphicListTool parentTool, int index)
            : base(parentTool, index)
        { }
        
        // SETUP METHODS: -------------------------------------------------------------------------

        protected override void SetupHeadExtras()
        {
            this.m_HeadEdit = new Button(() =>
            {
                InputDropdownText.Open("Description", this.m_Head, result =>
                {
                    this.m_Property.serializedObject.Update();
                    this.m_Property.FindPropertyRelative("m_Description").stringValue = result;
                    SerializationUtils.ApplyUnregisteredSerialization(this.m_Property.serializedObject);
                    this.ParentTool.Refresh();
                }, this.m_Property.FindPropertyRelative("m_Description").stringValue);
            });
            
            this.m_HeadEdit.Add(new Image
            {
                image = ICON_EDIT.Texture
            });
            
            this.m_HeadEdit.AddToClassList(CLASS_HEAD_BUTTON);
            this.m_HeadEdit.tooltip = TIP_EDIT;
            this.m_Head.Add(this.m_HeadEdit);
        }
    }
}
