using System.Linq;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace GameCreator.Editor.Variables
{
    public class NameVariableTool : TPolymorphicItemTool
    {
        private const string PROP_NAME = "m_Name";
        private const string PROP_VALUE = "m_Value";
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => this.Variable?.Title;

        private TVariable Variable
        {
            get
            {
                NameListTool parentTool = this.ParentTool as NameListTool;
                return parentTool?.NameList.Get(this.Index);
            }
        }

        protected override object Value => this.m_Property.GetValue<NameVariable>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public NameVariableTool(IPolymorphicListTool parentTool, int propertyIndex)
            : base(parentTool, propertyIndex)
        { }

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override void SetupBody()
        {
            this.m_Property.serializedObject.Update();

            PropertyField field = new PropertyField(this.m_Property);
            field.Bind(this.m_Property.serializedObject);
            
            field.RegisterValueChangeCallback(_ =>
            {
                this.m_Property.serializedObject.Update();
                this.UpdateHead();
            });

            this.m_Body.Add(field);
            this.UpdateBody(false);
        }

        protected override Texture2D GetIcon()
        {
            this.m_Property.serializedObject.Update();
            
            TValue instance = this.Variable.GetType()
                .GetField(PROP_VALUE, BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(this.Variable) as TValue;

            ImageAttribute iconAttrs = instance?.GetType()
                .GetCustomAttributes<ImageAttribute>()
                .FirstOrDefault();
            
            Texture2D icon = iconAttrs?.Image;
            return icon != null ? icon : Texture2D.whiteTexture;
        }

        protected override void OnChangePlayMode(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    this.m_Body?.SetEnabled(false);
                    break;
                
                default:
                    this.m_Body?.SetEnabled(true);
                    break;
            }
        }
    }
}