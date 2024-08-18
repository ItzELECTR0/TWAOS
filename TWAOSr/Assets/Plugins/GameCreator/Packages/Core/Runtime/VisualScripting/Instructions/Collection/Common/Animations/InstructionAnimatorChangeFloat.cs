using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Animator Float")]
    [Description("Changes the value of a 'Float' Animator parameter")]
    
    [Image(typeof(IconAnimator), ColorTheme.Type.Green)]

    [Category("Animations/Change Animator Float")]
    
    [Parameter("Parameter Name", "The Animator parameter name to be modified")]
    [Parameter("Value", "The value of the parameter that is set")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Parameter", "Number")]
    [Serializable]
    public class InstructionAnimatorChangeFloat : TInstructionAnimator
    {
        [SerializeField] private PropertyGetString m_Parameter = new PropertyGetString("My Parameter");
        [SerializeField] private ChangeDecimal m_Value = new ChangeDecimal(1f);
        
        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => 
            $"Change Animator Parameter {this.m_Parameter} on {this.m_Animator}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Animator.Get(args);
            if (gameObject == null) return;

            Animator animator = gameObject.Get<Animator>();
            if (animator == null) return;

            int parameter = Animator.StringToHash(this.m_Parameter.Get(args));

            float valueSource = animator.GetFloat(parameter);
            float valueTarget = (float) this.m_Value.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => animator.SetFloat(parameter, Mathf.Lerp(a, b, t)),
                Tween.GetHash(typeof(Animator), $"parameter:{parameter}"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}