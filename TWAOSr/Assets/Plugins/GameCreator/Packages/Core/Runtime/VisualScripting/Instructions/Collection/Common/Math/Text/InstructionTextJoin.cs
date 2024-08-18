using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Join")]
    [Description("Joins two string values and stores them")]
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayPlus))]
    
    [Category("Math/Text/Join")]
    [Parameter("Text 1", "The source of the first text")]
    [Parameter("Text 2", "The source of the second text")]
    
    [Keywords("Concat", "Concatenate", "Together", "Mix")]

    [Serializable]
    public class InstructionTextJoin : TInstructionText
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetString m_Text1 = GetStringString.Create;
        [SerializeField] private PropertyGetString m_Text2 = GetStringString.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Set} = {this.m_Text1} + {this.m_Text2}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            string text1 = this.m_Text1.Get(args);
            string text2 = this.m_Text2.Get(args);
            
            this.m_Set.Set(text1 + text2, args);
            return DefaultResult;
        }
    }
}