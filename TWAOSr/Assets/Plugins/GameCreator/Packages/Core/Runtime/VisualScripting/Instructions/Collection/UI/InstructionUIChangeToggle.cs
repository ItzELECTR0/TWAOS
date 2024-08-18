using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Change Toggle")]
    [Category("UI/Change Toggle")]
    
    [Image(typeof(IconUIToggle), ColorTheme.Type.TextLight)]
    [Description("Changes the value of a Toggle component")]

    [Parameter("Toggle", "The Toggle component that changes its value")]
    [Parameter("Value", "The new value set")]

    [Serializable]
    public class InstructionUIChangeToggle : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Toggle = GetGameObjectInstance.Create();
        [SerializeField] private PropertyGetBool m_Value = GetBoolValue.Create(true);

        public override string Title => $"Toggle {this.m_Toggle} = {this.m_Value}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Toggle.Get(args);
            if (gameObject == null) return DefaultResult;

            Toggle toggle = gameObject.Get<Toggle>();
            if (toggle == null) return DefaultResult;
            
            toggle.isOn = this.m_Value.Get(args);
            return DefaultResult;
        }
    }
}