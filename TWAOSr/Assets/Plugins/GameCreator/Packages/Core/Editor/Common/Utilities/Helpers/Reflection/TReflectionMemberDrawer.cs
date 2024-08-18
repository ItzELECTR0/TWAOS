using System;
using System.Collections.Generic;
using System.Reflection;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TReflectionMemberDrawer : PropertyDrawer
    {
        protected const BindingFlags BINDINGS = BindingFlags.Public    |
                                                BindingFlags.NonPublic |
                                                BindingFlags.Instance;
        
        private const string EMPTY_LABEL = " ";

        private const string USS_PATH = 
            EditorPaths.COMMON + 
            "Utilities/Helpers/Reflection/StyleSheets/ReflectionMember";
        
        private const string NAME_ROOT_NAME = "GC-ReflectionPick-Name";
        private const string NAME_DROPDOWN = "GC-ReflectionPick-Dropdown";
        
        private static readonly IIcon DROPDOWN = new IconDropdown(ColorTheme.Type.TextLight);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract Type AcceptType { get; }
        
        protected abstract bool DisableInPlaymode { get; }
        
        // PAINT METHODS: -------------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) root.styleSheets.Add(styleSheet);

            SerializedProperty component = property.FindPropertyRelative("m_Component");
            SerializedProperty member = property.FindPropertyRelative("m_Member");

            PropertyField fieldComponent = new PropertyField(component, EMPTY_LABEL);
            VisualElement fieldContent = new VisualElement { name = NAME_ROOT_NAME };
            
            TextField fieldText = new TextField(string.Empty)
            {
                bindingPath = member.propertyPath
            };
            
            fieldText.Bind(member.serializedObject);

            VisualElement fieldDropdown = new Image
            {
                image = DROPDOWN.Texture,
                name = NAME_DROPDOWN,
                focusable = true
            };
            
            fieldDropdown.AddManipulator(new MouseDropdownManipulator(context =>
            {
                Component reference = component.objectReferenceValue as Component;
                List<string> listNames = this.GetList(reference);

                foreach (string listName in listNames)
                {
                    context.menu.AppendAction(
                        listName,
                        menuAction =>
                        {
                            member.stringValue = menuAction.name;
                            SerializationUtils.ApplyUnregisteredSerialization(
                                member.serializedObject
                            );
                        },
                        menuAction => menuAction.name == member.stringValue
                            ? DropdownMenuAction.Status.Checked
                            : DropdownMenuAction.Status.Normal
                    );
                }
            }));

            bool isPlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
            fieldComponent.SetEnabled(!this.DisableInPlaymode || !isPlaymode);
            fieldContent.SetEnabled(!this.DisableInPlaymode || !isPlaymode);
            
            fieldContent.Add(new Label(" "));
            fieldContent.Add(fieldText);
            fieldContent.Add(fieldDropdown);

            AlignLabel.On(fieldContent);
            
            root.Add(fieldComponent);
            root.Add(fieldContent);
            
            return root;
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected abstract List<string> GetList(Component component);
    }
}