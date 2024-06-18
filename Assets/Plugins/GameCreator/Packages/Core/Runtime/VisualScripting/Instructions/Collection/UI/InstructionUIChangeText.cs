using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Change Text")]
    [Category("UI/Change Text")]
    
    [Image(typeof(IconUIText), ColorTheme.Type.TextLight)]
    [Description("Changes the value of a Text or Text Mesh Pro component")]

    [Parameter("Text", "The Text or Text Mesh Pro component that changes its value")]
    [Parameter("Value", "The new value set")]

    [Serializable]
    public class InstructionUIChangeText : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Text = GetGameObjectInstance.Create();
        [SerializeField] private PropertyGetString m_Value = GetStringString.Create;

        public override string Title => $"Text {this.m_Text} = {this.m_Value}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return DefaultResult;

            Text text = gameObject.Get<Text>();
            if (text != null)
            {
                text.text = this.m_Value.Get(args);
                return DefaultResult;
            }

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            if (textTMP != null) textTMP.text = this.m_Value.Get(args);
            
            return DefaultResult;
        }
    }
}