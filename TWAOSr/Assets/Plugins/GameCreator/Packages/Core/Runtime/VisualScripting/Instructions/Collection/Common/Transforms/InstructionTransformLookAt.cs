using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Look At")]
    [Description("Rotates the transform towards the chosen target")]
    
    [Image(typeof(IconEye), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]

    [Category("Transforms/Look At")]
    
    [Parameter("Target", "The desired targeted object to look at")]

    [Keywords("Rotate", "Rotation", "See")]
    [Serializable]
    public class InstructionTransformLookAt : TInstructionTransform
    {
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectTransform.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Transform} look at {this.m_Target}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Transform transform = this.m_Transform.Get<Transform>(args);
            if (transform == null) return DefaultResult;
            
            Transform target = this.m_Target.Get<Transform>(args);
            if (target == null) return DefaultResult;
            
            transform.LookAt(target);
            return DefaultResult;
        }
    }
}