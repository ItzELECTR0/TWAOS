using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Text Area")]
    [Category("Constants/Text Area")]
    
    [Image(typeof(IconTextArea), ColorTheme.Type.Yellow)]
    [Description("A string of characters which includes line breaks")]

    [Keywords("String", "Value")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringTextArea : PropertyTypeGetString
    {
        [SerializeField] protected TextAreaField m_Text = new TextAreaField();

        public override string Get(Args args) => this.m_Text.Text;
        public override string Get(GameObject gameObject) => this.m_Text.Text;

        public GetStringTextArea() : base()
        { }

        public GetStringTextArea(string text = "") : this()
        {
            this.m_Text = new TextAreaField(text);
        }

        public static PropertyGetString Create(string content = "") => new PropertyGetString(
            new GetStringTextArea(content)
        );
        
        public override string String
        {
            get
            {
                string text = this.m_Text.Text.Replace('\n', ' ');
                return string.IsNullOrEmpty(text) ? "<empty>" : text;
            }
        }

        public override string EditorValue => this.m_Text.Text.Replace('\n', ' ');
    }
}