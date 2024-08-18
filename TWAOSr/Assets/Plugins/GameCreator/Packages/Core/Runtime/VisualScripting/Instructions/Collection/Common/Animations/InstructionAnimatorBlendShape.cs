using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Blend Shape")]
    [Description("Changes the value of a Blend Shape parameter")]
    
    [Image(typeof(IconFace), ColorTheme.Type.Blue)]

    [Category("Animations/Change Blend Shape")]
    
    [Parameter("Skinned Mesh", "The Skinned Mesh Renderer component attached to the game object")]
    [Parameter("Blend Shape", "Name of the Blend Shape to change")]
    [Parameter("Value", "The target value of the blend shape")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Morph", "Target")]
    [Serializable]
    public class InstructionAnimatorBlendShape : Instruction
    {
        [SerializeField] 
        protected PropertyGetGameObject m_SkinnedMesh = new PropertyGetGameObject();
        
        [SerializeField] private PropertyGetString m_BlendShape = new PropertyGetString("Smile");
        [SerializeField] private ChangeDecimal m_Value = new ChangeDecimal(1f);
        
        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => 
            $"Change Blend-Shape {this.m_BlendShape} on {this.m_SkinnedMesh}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_SkinnedMesh.Get(args);
            if (gameObject == null) return;

            SkinnedMeshRenderer skinnedMesh = gameObject.Get<SkinnedMeshRenderer>();
            if (skinnedMesh == null) return;

            string blendShapeName = this.m_BlendShape.Get(args);
            int blendShapeIndex = skinnedMesh.sharedMesh.GetBlendShapeIndex(blendShapeName);
            
            float valueSource = skinnedMesh.GetBlendShapeWeight(blendShapeIndex);
            float valueTarget = (float) this.m_Value.Get(valueSource, args);

            ITweenInput tween = new TweenInput<float>(
                valueSource,
                valueTarget,
                this.m_Transition.Duration,
                (a, b, t) => skinnedMesh.SetBlendShapeWeight(blendShapeIndex, Mathf.Lerp(a, b, t)),
                Tween.GetHash(typeof(SkinnedMeshRenderer), $"blend-shape:{blendShapeIndex}"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );
            
            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}