using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Animator Trigger")]
    [Description("Sets the value of a 'Trigger' Animator parameter")]
    
    [Image(typeof(IconAnimator), ColorTheme.Type.Green)]

    [Category("Animations/Set Animator Trigger")]
    
    [Parameter("Parameter Name", "The Animator parameter name modified")]

    [Keywords("Parameter", "Once", "Flag", "Notify")]
    [Serializable]
    public class InstructionAnimatorSetTrigger : TInstructionAnimator
    {
        [SerializeField] private PropertyGetString m_Parameter = new PropertyGetString("My Parameter");

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => 
            $"Set Animator Trigger {this.m_Parameter} on {this.m_Animator}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Animator.Get(args);
            if (gameObject == null) return DefaultResult;

            Animator animator = gameObject.Get<Animator>();
            if (animator == null) return DefaultResult;

            int parameter = Animator.StringToHash(this.m_Parameter.Get(args));
            
            animator.SetTrigger(parameter);
            return DefaultResult;
        }
    }
}