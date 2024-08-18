using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Parent")]
    [Description("Changes the parent of a game object")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Yellow)]

    [Category("Transforms/Set Parent")]
    
    [Parameter("Parent", "The game object that becomes the parent")]

    [Keywords("Child", "Children", "Hierarchy", "Hang", "Inherit")]
    [Serializable]
    public class InstructionTransformSetParent : TInstructionTransform
    {
        [SerializeField] private PropertyGetGameObject m_Parent = GetGameObjectTransform.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set Parent of {this.m_Transform} to {this.m_Parent}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return DefaultResult;

            GameObject parent = this.m_Parent.Get(args);
            if (parent == null) return DefaultResult;
            
            gameObject.transform.SetParent(parent.transform);
            return DefaultResult;
        }
    }
}