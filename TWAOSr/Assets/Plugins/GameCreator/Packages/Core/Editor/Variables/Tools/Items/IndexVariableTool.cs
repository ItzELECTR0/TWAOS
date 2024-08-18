using System.Collections.Generic;
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
    public class IndexVariableTool : TPolymorphicItemTool
    {
        private const string PROP_VALUE = "m_Value";
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"{this.Index}: {this.Variable?.Title}";
        private TVariable Variable
        {
            get
            {
                IndexListTool parentTool = this.ParentTool as IndexListTool;
                return parentTool?.IndexList.Get(this.Index);
            }
        }

        protected override object Value => this.m_Property.GetValue<IndexVariable>();
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public IndexVariableTool(IPolymorphicListTool parentTool, int propertyIndex)
            : base(parentTool, propertyIndex)
        { }
        
        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override void SetupBody()
        {
            this.m_Property.serializedObject.Update();
            SerializedProperty propertyValue = this.m_Property.FindPropertyRelative(PROP_VALUE); 
            
            PropertyField fieldValue = new PropertyField(propertyValue);
            fieldValue.Bind(propertyValue.serializedObject);
            
            fieldValue.RegisterValueChangeCallback(_ => this.UpdateHead());

            this.m_Body.Add(fieldValue);
            this.UpdateBody(false);
        }

        protected override Texture2D GetIcon()
        {
            this.m_Property.serializedObject.Update();
            
            TValue instance = this.Variable.GetType()
                .GetField(PROP_VALUE, BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(this.Variable) as TValue;

            IEnumerable<ImageAttribute> iconAttrs = instance?.GetType()
                .GetCustomAttributes<ImageAttribute>();
            Texture2D icon = iconAttrs?.FirstOrDefault()?.Image;

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
