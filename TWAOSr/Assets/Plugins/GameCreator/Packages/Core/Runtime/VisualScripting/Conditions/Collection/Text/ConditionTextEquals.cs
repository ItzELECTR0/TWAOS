using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Text Equals")]
    [Description("Returns true if two text Strings are equal")]

    [Category("Text/Text Equals")]
    
    [Parameter("Text 1", "The first text string to compare")]
    [Parameter("Text 2", "The second text string to compare")]

    [Keywords("String", "Char")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionTextEquals : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetString m_Text1 = new PropertyGetString();
        [SerializeField] private PropertyGetString m_Text2 = new PropertyGetString();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Text1} = {this.m_Text2}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            string text1 = this.m_Text1.Get(args);
            string text2 = this.m_Text2.Get(args);

            return text1 == text2;
        }
    }
}
