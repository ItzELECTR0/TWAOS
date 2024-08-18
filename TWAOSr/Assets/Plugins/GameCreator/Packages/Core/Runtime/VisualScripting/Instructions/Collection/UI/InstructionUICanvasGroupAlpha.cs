using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Canvas Group Alpha")]
    [Category("UI/Canvas Group Alpha")]
    
    [Image(typeof(IconUICanvasGroup), ColorTheme.Type.TextLight)]
    [Description("Changes the opacity of the Canvas Group and affects all of its children")]

    [Parameter("Canvas Group", "The Canvas Group component that changes its value")]
    [Parameter("Alpha", "The new opacity value transformation of the Canvas Group")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Serializable]
    public class InstructionUICanvasGroupAlpha : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_CanvasGroup = GetGameObjectInstance.Create();
        [SerializeField] private ChangeDecimal m_Alpha = new ChangeDecimal(1f);
        [SerializeField] private Transition m_Transition = new Transition(TimeMode.UpdateMode.UnscaledTime);

        public override string Title => $"{this.m_CanvasGroup} alpha {this.m_Alpha}";
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_CanvasGroup.Get(args);
            if (gameObject == null) return;

            CanvasGroup canvasGroup = gameObject.Get<CanvasGroup>();
            if (canvasGroup == null) return;
            
            float valueSource = canvasGroup.alpha;
            float valueTarget = (float) this.m_Alpha.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => canvasGroup.alpha = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Animator), "canvas-group-alpha"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}