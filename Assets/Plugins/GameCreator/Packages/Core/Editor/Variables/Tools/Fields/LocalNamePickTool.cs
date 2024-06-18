using System.Collections.Generic;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class LocalNamePickTool : TNamePickTool
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        private TextField m_NameField;
        private VisualElement m_NameDropdown;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override Object Asset
        {
            get
            {
                return this.m_PropertyVariable.propertyType switch
                {
                    SerializedPropertyType.ObjectReference =>
                        this.m_PropertyVariable.objectReferenceValue,
                    
                    SerializedPropertyType.ManagedReference =>
                        this.m_PropertyVariable.managedReferenceValue is PropertyGetGameObject
                            property && property.EditorValue != null
                            ? property.EditorValue.GetComponent<LocalNameVariables>()
                            : null,
                    
                    _ => null
                };
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public LocalNamePickTool(SerializedProperty property)
            : base(property, true, false, ValueNull.TYPE_ID)
        { }
        
        public LocalNamePickTool(SerializedProperty property, IdString typeID, bool allowCast)
            : base(property, false, allowCast, typeID)
        { }

        protected override void RefreshPickList(Object asset)
        {
            base.RefreshPickList(asset);
            
            this.m_NameField = new TextField(string.Empty)
            {
                bindingPath = this.m_PropertyName.propertyPath
            };
            
            this.m_NameField.Bind(this.m_Property.serializedObject);

            this.m_NameDropdown = new Image
            {
                image = ICON_DROPDOWN.Texture,
                name = NAME_DROPDOWN,
                focusable = true
            };
            
            this.m_NameDropdown.SetEnabled(asset != null);
            this.m_NameDropdown.AddManipulator(new MouseDropdownManipulator(context =>
            {
                Dictionary<string, bool> listNames = this.GetVariablesList(asset);
                foreach (KeyValuePair<string, bool> entry in listNames)
                {
                    context.menu.AppendAction(
                        entry.Key,
                        menuAction =>
                        {
                            this.m_PropertyName.stringValue = menuAction.name;
                            SerializationUtils.ApplyUnregisteredSerialization(
                                this.m_Property.serializedObject
                            );
                        },
                        menuAction =>
                        {
                            if (menuAction.name != this.m_PropertyName.stringValue)
                            {
                                return entry.Value
                                    ? DropdownMenuAction.Status.Normal
                                    : DropdownMenuAction.Status.Disabled;
                            }
                            
                            return DropdownMenuAction.Status.Checked;
                        }
                    );
                }
            }));

            VisualElement nameContainer = new VisualElement { name = NAME_ROOT_NAME };
            
            nameContainer.Add(new Label(" "));
            nameContainer.Add(this.m_NameField);
            nameContainer.Add(this.m_NameDropdown);

            AlignLabel.On(nameContainer);
            
            this.Add(nameContainer);
        }

        private Dictionary<string, bool> GetVariablesList(Object asset)
        {
            Dictionary<string, bool> list = new Dictionary<string, bool> {{ string.Empty, false }};
            LocalNameVariables variable = asset as LocalNameVariables;
            
            if (variable == null) return list;

            NameVariableRuntime runtime = variable.GetType()
                .GetField("m_Runtime", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(variable) as NameVariableRuntime;
            
            NameList names = runtime?.GetType()
                .GetField("m_List", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(runtime) as NameList;

            return this.FilterNames(names);
        }
    }
}