using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Text Contains")]
    [Description("Returns true if the second text string occurs in the first one")]

    [Category("Text/Text Contains")]
    
    [Parameter("Text", "The text string")]
    [Parameter("Substring", "The text string contained in Text")]

    [Keywords("String", "Char", "Sub")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    [Serializable]
    public class ConditionTextContains : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetString m_Text = new PropertyGetString();
        [SerializeField] private PropertyGetString m_Substring = new PropertyGetString();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Text} contains {this.m_Substring}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            string text = this.m_Text.Get(args);
            string substring = this.m_Substring.Get(args);

            return text.Contains(substring);
        }
    }
}
