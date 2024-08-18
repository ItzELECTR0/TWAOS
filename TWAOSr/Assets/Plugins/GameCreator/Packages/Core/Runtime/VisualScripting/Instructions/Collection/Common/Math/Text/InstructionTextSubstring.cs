using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Substring")]
    [Description("Extracts a substring based on an index and length")]
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayBar))]
    
    [Category("Math/Text/Substring")]
    [Parameter("Text", "The source of the text")]
    [Parameter("Index", "Starting index of the substring")]
    [Parameter("Length", "Amount of characters extracted")]

    [Serializable]
    public class InstructionTextSubstring : TInstructionText
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetString m_Text = GetStringString.Create;

        [SerializeField] private PropertyGetInteger m_Index = GetDecimalInteger.Create(0);
        [SerializeField] private PropertyGetInteger m_Length = GetDecimalInteger.Create(5);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Set} = Substring of {this.m_Text}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string text = this.m_Text.Get(args);
            int index = (int) this.m_Index.Get(args);
            int length = (int) this.m_Length.Get(args);
            
            this.m_Set.Set(text.Substring(index, length), args);
            return DefaultResult;
        }
    }
}