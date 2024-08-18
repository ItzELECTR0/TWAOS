using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Text")]
    [Description("Changes the value of a string")]
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    
    [Category("Math/Text/Set Text")]
    [Parameter("Text", "The source of the text")]

    [Serializable]
    public class InstructionTextSetString : TInstructionText
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetString m_Text = GetStringString.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Set} = {this.m_Text}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string text = this.m_Text.Get(args);
            this.m_Set.Set(text, args);
            
            return DefaultResult;
        }
    }
}