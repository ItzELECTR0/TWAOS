using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Open Web Page")]
    [Description("Opens the specified URL with the default web browser")]

    [Category("Application/Open Web Page")]
    
    [Parameter("URL", "The route link to open. Must include the protocol prepended (http or https)")]

    [Keywords("Site", "Internet")]
    [Image(typeof(IconWeb), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionAppOpenWeb : Instruction
    {
        private const string DEF = "https://gamecreator.io";
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetString m_URL = new PropertyGetString(DEF);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Open Browser URL: {this.m_URL}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            string url = this.m_URL.Get(args);
            Application.OpenURL(url);

            return DefaultResult;
        }
    }
}