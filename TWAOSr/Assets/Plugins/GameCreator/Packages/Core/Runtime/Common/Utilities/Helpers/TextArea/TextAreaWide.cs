using System;

namespace GameCreator.Runtime.Common
{
    /// <summary>
    /// Displays a wide text area occupying the label and field space
    /// </summary>
    [Serializable]
    public class TextAreaWide : BaseTextArea
    {
        public TextAreaWide() : base()
        { }
        
        public TextAreaWide(string text) : base(text)
        { }
    }
}