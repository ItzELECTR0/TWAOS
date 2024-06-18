using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    public abstract class TUnitDrawer : PropertyDrawer
    {
        private const string PATH_STYLES = EditorPaths.CHARACTERS + "StyleSheets/";

        private static readonly IIcon ICON_ARROW = new IconDropdown(ColorTheme.Type.TextLight);

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected VisualElement MakePropertyGUI(SerializedProperty property, string headTitle)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            root.Add(head);
            root.Add(body);

            string customUSS = PathUtils.Combine(PATH_STYLES, "Unit");
            StyleSheet[] styleSheets = StyleSheetUtils.Load(customUSS);

            foreach (StyleSheet sheet in styleSheets) root.styleSheets.Add(sheet);

            root.AddToClassList("gc-character-unit-root");
            head.AddToClassList("gc-character-unit-head");
            body.AddToClassList("gc-character-unit-body");

            this.BuildHead(head, body, property, headTitle);
            this.BuildBody(body, property);

            return root;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void BuildHead(VisualElement head, VisualElement body, SerializedProperty property,
            string headTitle)
        {
            head.Clear();

            Type typeFull = TypeUtils.GetTypeFromProperty(property, true);
            Type typeField = TypeUtils.GetTypeFromProperty(property, false);
            
            ImageAttribute iconAttr = typeFull?.GetCustomAttributes<ImageAttribute>().FirstOrDefault();

            Image image = new Image
            {
                image = this.UnitIcon.Texture
            };

            Button btnToggle = new Button
            {
                text = headTitle
            };

            btnToggle.clicked += () =>
            {
                property.isExpanded = !property.isExpanded;
                this.UpdateBodyState(property.isExpanded, body);
            };

            Label btnToggleRightLabel = new Label(TypeUtils.GetTitleFromType(typeFull));
            
            Button btnChangeType = new Button();
            btnChangeType.SetEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
            
            btnChangeType.clicked += () => TypeSelectorFancyPopup.Open(
                head, typeField,
                newType =>
                {
                    if (newType == null) return;
                    property.serializedObject.Update();

                    IUnitCommon unit = Activator.CreateInstance(newType) as IUnitCommon;
                    property.SetValue(unit);
                    
                    SerializationUtils.ApplyUnregisteredSerialization(property.serializedObject);

                    this.BuildHead(head, body, property, headTitle);
                    this.BuildBody(body, property);
                }
            );

            Image imageChangeType = new Image
            {
                image = iconAttr != null ? iconAttr.Image : Texture2D.whiteTexture
            };

            Image imageChevron = new Image
            {
                image = ICON_ARROW.Texture
            };

            imageChangeType.AddToClassList("gc-character-unit-head-image");
            imageChevron.AddToClassList("gc-character-unit-head-arrow");
            
            btnChangeType.Add(imageChangeType);
            btnChangeType.Add(imageChevron);

            image.AddToClassList("gc-character-unit-head-image");
            btnToggle.AddToClassList("gc-character-unit-head-btn__toggle");
            btnToggleRightLabel.AddToClassList("gc-character-unit-head-label__unit");
            btnChangeType.AddToClassList("gc-character-unit-head-btn__change");

            btnToggle.contentContainer.Add(btnToggleRightLabel);

            head.Add(image);
            head.Add(btnToggle);
            head.Add(btnChangeType);

            head.Bind(property.serializedObject);
            this.UpdateBodyState(property.isExpanded, body);

            this.OnBuildHead(head, property);
        }

        protected virtual void BuildBody(VisualElement body, SerializedProperty property)
        {
            body.Clear();

            SerializationUtils.CreateChildProperties(
                body,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );
            
            this.OnBuildBody(body, property);
        }
        
        // ABSTRACT PROPERTIES: -------------------------------------------------------------------
        
        protected abstract IIcon UnitIcon { get; }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual void OnBuildHead(VisualElement head, SerializedProperty property)
        { }
        
        protected virtual void OnBuildBody(VisualElement body, SerializedProperty property)
        { }

        // OTHER METHODS: -------------------------------------------------------------------------

        private void UpdateBodyState(bool state, VisualElement body)
        {
            body.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}