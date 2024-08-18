using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Animator Boolean")]
    [Description("Sets the value of a 'Bool' Animator parameter")]
    
    [Image(typeof(IconAnimator), ColorTheme.Type.Green)]

    [Category("Animations/Set Animator Boolean")]
    
    [Parameter("Parameter Name", "The Animator parameter name to be modified")]
    [Parameter("Value", "The value of the parameter that is set")]

    [Keywords("Parameter", "Bool")]
    [Serializable]
    public class InstructionAnimatorSetBoolean : TInstructionAnimator
    {
        [SerializeField] private PropertyGetString m_Parameter = new PropertyGetString("My Parameter");
        [SerializeField] private ChangeBool m_Value = new ChangeBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => 
            $"Set Animator Parameter {this.m_Parameter} on {this.m_Animator}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Animator.Get(args);
            if (gameObject == null) return DefaultResult;

            Animator animator = gameObject.Get<Animator>();
            if (animator == null) return DefaultResult;

            int parameter = Animator.StringToHash(this.m_Parameter.Get(args));

            bool valueSource = animator.GetBool(parameter);
            bool valueTarget = this.m_Value.Get(valueSource, args);
            
            animator.SetBool(parameter, valueTarget);
            return DefaultResult;
        }
    }
}