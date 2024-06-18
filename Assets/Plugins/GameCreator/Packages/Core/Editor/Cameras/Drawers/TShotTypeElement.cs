using System;
using System.Linq;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Cameras
{
    public sealed class TShotTypeElement : TypeSelectorValueElement
    {
        private const string USS_PATH = EditorPaths.CAMERAS + "StyleSheets/ShotType";

        private const string NAME_HEAD_BUTTON = "GC-Cameras-ShotType-Head-Btn";
        private const string NAME_HEAD_BUTTON_ICON = "GC-Cameras-ShotType-Head-BtnIcon";
        private const string NAME_HEAD_BUTTON_LABEL = "GC-Cameras-ShotType-Head-BtnLabel";
        private const string NAME_HEAD_BUTTON_ARROW = "GC-Cameras-ShotType-Head-BtnArrow";

        private static readonly IIcon ICON_ARROW = new IconDropdown(ColorTheme.Type.TextLight);
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private Button m_Button;
        
        private Image m_ButtonIcon;
        private Label m_ButtonLabel;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameRoot => "GC-Cameras-ShotType-Root";
        protected override string ElementNameHead => "GC-Cameras-ShotType-Head";
        protected override string ElementNameBody => "GC-Cameras-ShotType-Body";

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TShotTypeElement(SerializedProperty property) : base(property, false)
        {
            this.TypeSelector = new TypeSelectorFancyProperty(this.m_Property, this.m_Button);
            this.TypeSelector.EventChange += this.OnChange;

            this.SetupStyleSheets();
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
                image = ICON_ARROW.Texture,
                name = NAME_HEAD_BUTTON_ARROW
            });
            
            this.m_Head.Add(this.m_Button);
        }

        protected override void CreateBody()
        {
            this.m_Property.serializedObject.Update();
            SerializationUtils.CreateChildProperties(
                this.m_Body, 
                this.m_Property, 
                this.HideLabels
                    ? SerializationUtils.ChildrenMode.HideLabelsInChildren
                    : SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );
        }

        protected override void OnChange(Type prevType, Type newType)
        {
            base.OnChange(prevType, newType);
            this.RefreshHead();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SetupStyleSheets()
        {
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                this.styleSheets.Add(styleSheet);
            }
            
            this.m_Body.AddToClassList("gc-cameras-shot-type-body");
        }
        
        private void RefreshHead()
        {
            this.m_Property.serializedObject.Update();
            
            Type fieldType = TypeUtils.GetTypeFromProperty(this.m_Property, true);
            ImageAttribute iconAttribute = fieldType
                .GetCustomAttributes<ImageAttribute>()
                .FirstOrDefault();
            
            this.m_ButtonIcon.image = iconAttribute?.Image;
            this.m_ButtonLabel.text = TypeUtils.GetTitleFromType(fieldType);
        }
    }
}