using System;

namespace GameCreator.Runtime.Common
{
    /// <summary>
    /// Displays a text area as a property with an empty label
    /// </summary>
    [Serializable]
    public class TextAreaField : BaseTextArea
    {
        public TextAreaField() : base()
        { }
        
        public TextAreaField(string text) : base(text)
        { }
    }
}