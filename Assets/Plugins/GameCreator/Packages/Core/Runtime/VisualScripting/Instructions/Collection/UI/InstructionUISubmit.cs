using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Submit")]
    [Category("UI/Submit")]
    
    [Image(typeof(IconUIHoverEnter), ColorTheme.Type.TextLight)]
    [Description("Performs a submit action on a UI element")]

    [Keywords("Enter", "Press", "Confirm")]

    [Serializable]
    public class InstructionUISubmit : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Submit = GetGameObjectInstance.Create();
        
        public override string Title => $"Submit {this.m_Submit}";
        
        protected override Task Run(Args args)
        {
            GameObject submit = this.m_Submit.Get(args);
            if (submit == null) return DefaultResult;
            
            ISubmitHandler[] submitHandlers = submit.GetComponents<ISubmitHandler>();
            foreach (ISubmitHandler submitHandler in submitHandlers)
            {
                submitHandler?.OnSubmit(null);   
            }

            return DefaultResult;
        }
    }
}