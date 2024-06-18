using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Animator Integer")]
    [Description("Changes the value of a 'Integer' Animator parameter")]
    
    [Image(typeof(IconAnimator), ColorTheme.Type.Green)]

    [Category("Animations/Change Animator Integer")]
    
    [Parameter("Parameter Name", "The Animator parameter name to be modified")]
    [Parameter("Value", "The value of the parameter that is set")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Parameter", "Number")]
    [Serializable]
    public class InstructionAnimatorChangeInteger : TInstructionAnimator
    {
        [SerializeField] private PropertyGetString m_Parameter = new PropertyGetString("My Parameter");
        [SerializeField] private ChangeInteger m_Value = new ChangeInteger(7);
        
        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => 
            $"Animator {this.m_Parameter} on {this.m_Animator}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Animator.Get(args);
            if (gameObject == null) return;

            Animator animator = gameObject.Get<Animator>();
            if (animator == null) return;

            int parameter = Animator.StringToHash(this.m_Parameter.Get(args));

            int valueSource = animator.GetInteger(parameter);
            int valueTarget = this.m_Value.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => animator.SetInteger(parameter, Mathf.FloorToInt(Mathf.Lerp(a, b, t))),
                Tween.GetHash(typeof(Animator), $"parameter:{parameter}"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}