using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class ConditionItemTool : TPolymorphicItemTool
    {
        private static readonly IIcon ICON_TOGGLE_ON = new IconToggleOn(ColorTheme.Type.Green);
        private static readonly IIcon ICON_TOGGLE_OFF = new IconToggleOff(ColorTheme.Type.Red);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly SerializedProperty m_PropertySign;
        
        private Button m_SignButton;
        private Image m_SignIcon;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Conditions/StyleSheets/Condition-Head",
            EditorPaths.VISUAL_SCRIPTING + "Conditions/StyleSheets/Condition-Body"
        };
        
        protected override object Value => this.m_Property.GetValue<Condition>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ConditionItemTool(IPolymorphicListTool parentTool, int index)
            : base(parentTool, index)
        {
            this.m_PropertySign = this.m_Property.FindPropertyRelative("m_Sign");
            this.UpdateSign();
        }
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void SetupHead()
        {
            base.SetupHead();

            this.m_SignButton = new Button(() =>
            {
                this.m_PropertySign.serializedObject.Update();
                this.m_PropertySign.boolValue = !this.m_PropertySign.boolValue;

                this.m_PropertySign.serializedObject.ApplyModifiedPropertiesWithoutUndo();

                this.UpdateSign();
                this.UpdateHead();
            })
            {
                name = "GC-Condition-List-Item-Sign"
            };

            this.m_SignIcon = new Image();
            
            this.m_SignButton.Add(this.m_SignIcon);
            this.m_Head.Insert(1, this.m_SignButton);
            
            this.UpdateHead();
        }

        private void UpdateSign()
        {
            this.m_PropertySign.serializedObject.Update();
            this.m_SignIcon.image = this.m_PropertySign.boolValue
                ? ICON_TOGGLE_ON.Texture
                : ICON_TOGGLE_OFF.Texture;
        }
    }
}
