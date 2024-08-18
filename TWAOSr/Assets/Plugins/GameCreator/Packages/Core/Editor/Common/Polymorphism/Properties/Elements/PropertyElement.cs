using System;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    public class PropertyElement : TypeSelectorValueElement
    {
        private const string USS_PATH =
            EditorPaths.COMMON + "Polymorphism/Properties/Styles/PropertyElement";

        private static readonly IIcon ICON_DROP = new IconDropdown(ColorTheme.Type.TextLight);
        
        private const string CLASS_BODY_CONTENT = "gc-property-element-body-content";
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Button m_Button;
        [NonSerialized] private Label m_Label;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameRoot => "GC-PropertyElement-Root";
        protected override string ElementNameHead => "GC-PropertyElement-Head";
        protected override string ElementNameBody => "GC-PropertyElement-Body";

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Type, Type> EventChangeType;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public PropertyElement(SerializedProperty property, string label, bool hideLabels) 
            : base(property, hideLabels)
        {
            AlignLabel.On(this);
            
            this.TypeSelector = new TypeSelectorFancyProperty(this.m_Property, this.m_Button);
            this.TypeSelector.EventChange += this.OnChange;
            
            this.m_Label.text = label;

            this.RefreshButton();
            this.LoadHeadStyleSheet(this.m_Root);
        }
        
        protected override void CreateHead()
        {
            base.CreateHead();
            this.m_Head.AddToClassList("unity-base-field");
            
            this.m_Button = new Button();
            this.m_Button.AddToClassList("unity-base-field__input");
            
            this.m_Button.Add(new FlexibleSpace());
            this.m_Button.Add(new Image { image = ICON_DROP.Texture });

            this.m_Label = new Label();
            this.m_Label.AddToClassList("unity-base-field__label");
            this.m_Label.AddToClassList("unity-label");
            this.m_Label.AddToClassList("unity-property-field__label");
            this.m_Label.AddToClassList(AlignLabel.CLASS_UNITY_INSPECTOR_ELEMENT);
            
            this.m_Head.Add(this.m_Label);
            this.m_Head.Add(this.m_Button);
        }

        protected override void CreateBody()
        {
            bool hasAnyProperties = SerializationUtils.CreateChildProperties(
                this.m_Body, 
                this.m_Property,
                this.HideLabels
                    ? SerializationUtils.ChildrenMode.HideLabelsInChildren
                    : SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );
            
            if (hasAnyProperties)
            {
                this.m_Body.AddToClassList(CLASS_BODY_CONTENT);
            }
            else
            {
                this.m_Body.RemoveFromClassList(CLASS_BODY_CONTENT);
            }
        }

        protected override void OnChange(Type prevType, Type newType)
        {
            base.OnChange(prevType, newType);
            this.RefreshButton();
            
            this.EventChangeType?.Invoke(prevType, newType);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void LoadHeadStyleSheet(VisualElement element)
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) element.styleSheets.Add(sheet);
        }
        
        private void RefreshButton()
        {
            this.m_Property.serializedObject.Update();
            
            Type fieldType = TypeUtils.GetTypeFromProperty(this.m_Property, true);
            this.m_Button.text = TypeUtils.GetTitleFromType(fieldType);
        }
    }
}
