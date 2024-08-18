using System;

namespace GameCreator.Runtime.Common
{
    /// <summary>
    /// Displays a text area with a label name
    /// </summary>
    [Serializable]
    public class TextAreaLabel : BaseTextArea
    {
        public TextAreaLabel() : base()
        { }
        
        public TextAreaLabel(string text) : base(text)
        { }
    }
}