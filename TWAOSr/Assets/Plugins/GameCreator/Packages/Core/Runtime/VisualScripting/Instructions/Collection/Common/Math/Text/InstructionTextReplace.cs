using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Replace")]
    [Description("Replaces all occurrences of a string with another string")]
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    
    [Category("Math/Text/Replace")]
    [Parameter("Text", "The source of the text")]
    [Parameter("Old Text", "The text replaced")]
    [Parameter("New Text", "The text that replaces each occurrence")]
    
    [Keywords("Substitute", "Change")]

    [Serializable]
    public class InstructionTextReplace : TInstructionText
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetString m_Text = GetStringString.Create;

        [SerializeField] private PropertyGetString m_OldText = new PropertyGetString("Old Text");
        [SerializeField] private PropertyGetString m_NewText = new PropertyGetString("New Text");

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Set} = Replace on {this.m_Text}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string text = this.m_Text.Get(args);
            
            string oldText = this.m_OldText.Get(args);
            string newText = this.m_NewText.Get(args);

            this.m_Set.Set(text.Replace(oldText, newText), args);
            return DefaultResult;
        }
    }
}