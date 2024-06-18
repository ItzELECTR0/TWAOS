using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Scale")]
    [Description("Changes the local scale of a game object over time")]
    
    [Image(typeof(IconScale), ColorTheme.Type.Yellow)]

    [Category("Transforms/Change Scale")]
    
    [Parameter("Scale", "The desired scale of the game object")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the scaling over time")]
    [Parameter("Wait to Complete", "Whether to wait until the scaling is finished or not")]
    
    [Keywords("Size", "Resize", "Grow", "Reduce", "Small", "Big")]
    [Serializable]
    public class InstructionTransformChangeScale : TInstructionTransform
    {
        [SerializeField] private ChangeScale m_Scale = new ChangeScale(Vector3.one);
        
        [SerializeField] private Transition m_Transition = new Transition();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Scale {this.m_Transform} {this.m_Scale}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return;

            Vector3 valueSource = gameObject.transform.localScale;
            Vector3 valueTarget = this.m_Scale.Get(valueSource, args);
            
            ITweenInput tween = new TweenInput<Vector3>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => gameObject.transform.localScale = Vector3.LerpUnclamped(a, b, t),
                Tween.GetHash(typeof(Transform), "scale"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}