using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Common;
using GameCreator.Editor.Common;
using UnityEngine;

namespace GameCreator.Editor.VisualScripting
{
    public sealed class EventElement : TypeSelectorValueElement
    {
        private const string USS_PATH = EditorPaths.VISUAL_SCRIPTING + "Events/StyleSheets/Event";

        private const string NAME_HEAD_BUTTON = "GC-Event-Head-Btn";
        private const string NAME_HEAD_BUTTON_ICON = "GC-Event-Head-BtnIcon";
        private const string NAME_HEAD_BUTTON_LABEL = "GC-Event-Head-BtnLabel";
        private const string NAME_HEAD_BUTTON_ARROW = "GC-Event-Head-BtnArrow";

        private static readonly IIcon ICON_DROPDOWN = new IconDropdown(ColorTheme.Type.TextLight);
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private Button m_Button;
        
        private Image m_ButtonIcon;
        private Label m_ButtonLabel;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameRoot => "GC-Event-Root";
        protected override string ElementNameHead => "GC-Event-Head";
        protected override string ElementNameBody => "GC-Event-Body";

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public EventElement(SerializedProperty property) : base(property, false)
        {
            this.TypeSelector = new TypeSelectorFancyProperty(this.m_Property, this.m_Button);
            this.TypeSelector.EventChange += this.OnChange;

            this.LoadHeadStyleSheet();
            this.RefreshHead();
        }

        protected override void CreateHead()
        {
            base.CreateHead();
            
            this.m_Button = new Button { name = NAME_HEAD_BUTTON };
            this.m_ButtonIcon = new Image { name = NAME_HEAD_BUTTON_ICON };
            this.m_ButtonLabel = new Label { name = NAME_HEAD_BUTTON_LABEL };
            
            this.m_Button.Add(this.m_ButtonIcon);
            this.m_Button.Add(this.m_ButtonLabel);
            this.m_Button.Add(new Image
            {
                image = ICON_DROPDOWN.Texture,
                name = NAME_HEAD_BUTTON_ARROW
            });
            
            this.m_Head.Add(this.m_Button);
        }

        protected override void CreateBody()
        {
            this.m_Property.serializedObject.Update();

            bool anyProperties = SerializationUtils.CreateChildProperties(
                this.m_Body, 
                this.m_Property, 
                this.HideLabels
                    ? SerializationUtils.ChildrenMode.HideLabelsInChildren
                    : SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );

            this.m_Body.style.display = anyProperties
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        protected override void OnChange(Type prevType, Type newType)
        {
            base.OnChange(prevType, newType);
            this.RefreshHead();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void LoadHeadStyleSheet()
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets)
            {
                this.styleSheets.Add(styleSheet);
            }
        }
        
        private void RefreshHead()
        {
            this.m_Property.serializedObject.Update();
            
            Type fieldType = TypeUtils.GetTypeFromProperty(this.m_Property, true);
            ImageAttribute iconAttribute = fieldType?
                .GetCustomAttributes<ImageAttribute>()
                .FirstOrDefault();
            
            this.m_ButtonIcon.image = iconAttribute != null 
                ? iconAttribute.Image 
                : Texture2D.whiteTexture;
            
            this.m_ButtonLabel.text = TypeUtils.GetTitleFromType(fieldType);
        }
    }
}
