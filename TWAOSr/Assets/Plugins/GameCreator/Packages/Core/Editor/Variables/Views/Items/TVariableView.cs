using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TVariableView<T> : VisualElement where T : TVariable
    {
        protected const string LABEL_VALUE = "Value";

        private const ColorTheme.Type TITLE_COLOR = ColorTheme.Type.TextNormal;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        protected readonly T m_Variable;
        
        protected readonly VisualElement m_Head;
        protected readonly VisualElement m_Body;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string Title { get; }
        
        protected bool IsExpanded { get; private set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TVariableView(T variable)
        {
            this.m_Variable = variable;
            
            this.m_Head = new VisualElement();
            this.m_Body = new VisualElement();
            
            this.m_Head.AddToClassList(TPolymorphicItemTool.CLASS_HEAD);
            this.m_Body.AddToClassList(TPolymorphicItemTool.CLASS_BODY);

            List<string> styleSheetsPaths = new List<string>();
            styleSheetsPaths.Add(TPolymorphicItemTool.USS_HEAD_PATH);
            styleSheetsPaths.Add(TPolymorphicItemTool.USS_BODY_PATH);
            styleSheetsPaths.Add(TPolymorphicItemTool.USS_DROP_PATH);

            StyleSheet[] sheets = StyleSheetUtils.Load(styleSheetsPaths.ToArray());
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected void SetupHead()
        {
            Image image = new Image
            {
                image = this.m_Variable.Icon
            };
            
            Label label = new Label(this.Title);
            label.style.color = ColorTheme.Get(TITLE_COLOR);

            Button button = new Button(() =>
            {
                this.IsExpanded = !this.IsExpanded;
                this.SetupBody();
            });
            
            button.AddToClassList(TPolymorphicItemTool.CLASS_HEAD_INFO);

            button.Add(image);
            button.Add(label);
            
            this.m_Head.Add(button);
            this.Add(this.m_Head);
        }
        
        protected void SetupBody()
        {
            if (this.Contains(this.m_Body))
            {
                this.Remove(this.m_Body);
            }
            
            if (!this.IsExpanded) return;
            
            this.Add(this.m_Body);
            
            this.m_Body.Clear();
            this.m_Body.Add(this.MakeBody());
        }

        protected void GetFieldValue(VisualElement container)
        {
            if (this.m_Variable.Value is Object unityObject)
            {
                ObjectField objectField = new ObjectField(LABEL_VALUE)
                {
                    value = unityObject,
                    objectType = this.m_Variable.Type
                };

                objectField.SetEnabled(false);
                container.Add(objectField);
            }
            else
            {
                TextField textField = new TextField(LABEL_VALUE)
                {
                    value = this.m_Variable.Value.ToString()
                };
            
                textField.SetEnabled(false);
                container.Add(textField);
            }
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract VisualElement MakeBody();
    }
}