using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Clear Parent")]
    [Description("Clears the parent of a game object")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Yellow, typeof(OverlayMinus))]

    [Category("Transforms/Clear Parent")]

    [Keywords("Child", "Children", "Hierarchy", "Orphan")]
    [Serializable]
    public class InstructionTransformClearParent : TInstructionTransform
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Clear Parent of {this.m_Transform}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return DefaultResult;

            gameObject.transform.SetParent(null);
            return DefaultResult;
        }
    }
}