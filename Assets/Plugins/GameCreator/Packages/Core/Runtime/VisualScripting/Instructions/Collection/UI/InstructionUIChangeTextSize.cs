using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Change Font Size")]
    [Category("UI/Change Font Size")]
    
    [Image(typeof(IconUIText), ColorTheme.Type.TextLight)]
    [Description("Changes the size of the Text or Text Mesh Pro component content")]

    [Parameter("Text", "The Text or Text Mesh Pro component that changes its font size")]
    [Parameter("Size", "The new text size, in pixels")]
    
    [Keywords("Text")]

    [Serializable]
    public class InstructionUIChangeTextSize : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Text = GetGameObjectInstance.Create();
        [SerializeField] private PropertyGetInteger m_Size = new PropertyGetInteger(12);

        public override string Title => $"Font Size {this.m_Text} = {this.m_Size}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return DefaultResult;

            Text text = gameObject.Get<Text>();
            if (text != null)
            {
                text.fontSize = Mathf.FloorToInt((float) this.m_Size.Get(args));
                return DefaultResult;
            }

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            if (textTMP != null) textTMP.fontSize = (float) this.m_Size.Get(args);
            
            return DefaultResult;
        }
    }
}