using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class SelectTypeElement : TypeSelectorValueElement
    {
        private const string USS_PATH = 
            EditorPaths.COMMON + 
            "Utilities/Helpers/TypeReference/StyleSheets/SelectType";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Button m_Button;
        private Label m_Label;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string ElementNameRoot => "GC-SelectType-Root";
        protected override string ElementNameHead => "GC-SelectType-Head";
        protected override string ElementNameBody => "GC-SelectType-Body";

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SelectTypeElement(SerializedProperty property, Type typeBase, string label) 
            : base(property, true)
        {
            this.TypeSelector = new TypeSelectorFancyType(this.m_Property, typeBase, this.m_Button);
            this.TypeSelector.EventChange += this.OnChange;

            this.m_Label.text = label;
            this.RefreshButton();
        }
        
        protected override void CreateHead()
        {
            base.CreateHead();
            this.m_Head.AddToClassList("unity-base-field");
            
            this.m_Button = new Button();
            this.m_Button.AddToClassList("unity-base-field__input");

            this.m_Label = new Label();
            this.m_Label.AddToClassList("unity-base-field__label");
            this.m_Label.AddToClassList("unity-label");
            this.m_Label.AddToClassList("unity-property-field__label");
            this.m_Label.AddToClassList(AlignLabel.CLASS_UNITY_INSPECTOR_ELEMENT);
            
            this.m_Head.Add(this.m_Label);
            this.m_Head.Add(this.m_Button);

            AlignLabel.On(this.m_Head);
            this.LoadHeadStyleSheet(this.m_Head);
        }

        protected override void OnChange(Type prevType, Type newType)
        {
            if (this.m_Property == null) return;

            base.OnChange(prevType, newType);
            this.RefreshButton();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void LoadHeadStyleSheet(VisualElement element)
        {
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                element.styleSheets.Add(styleSheet);
            }
        }
        
        private void RefreshButton()
        {
            if (this.m_Property == null) return;

            Type type = Type.GetType(this.m_Property.stringValue, false);
            this.m_Button.text = type != null ? TextUtils.Humanize(type.Name) : "(none)";
        }
    }
}