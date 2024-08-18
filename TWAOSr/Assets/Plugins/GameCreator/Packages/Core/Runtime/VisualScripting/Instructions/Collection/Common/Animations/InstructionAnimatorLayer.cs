using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Animator Layer")]
    [Description("Changes the weight of an Animator Layer")]
    
    [Image(typeof(IconAnimator), ColorTheme.Type.Blue)]

    [Category("Animations/Change Animator Layer")]
    
    [Parameter("Layer Index", "The Animator's Layer index that's being modified")]
    [Parameter("Weight", "The target Animator layer weight")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Weight")]
    [Serializable]
    public class InstructionAnimatorLayer : TInstructionAnimator
    {
        [SerializeField] private int m_LayerIndex = 1;
        [SerializeField] private ChangeDecimal m_Weight = new ChangeDecimal(1f);
        
        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Change Layer Weight {this.m_LayerIndex} on {this.m_Animator}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Animator.Get(args);
            if (gameObject == null) return;

            Animator animator = gameObject.Get<Animator>();
            if (animator == null) return;
            
            if (this.m_LayerIndex >= animator.layerCount) return;
            
            float valueSource = animator.GetLayerWeight(this.m_LayerIndex);
            float valueTarget = (float) this.m_Weight.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => animator.SetLayerWeight(this.m_LayerIndex, Mathf.Lerp(a, b, t)),
                Tween.GetHash(typeof(Animator), "weight"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}