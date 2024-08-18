using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Pool Destroy")]
    [Description("Destroys an existing game object pool")]

    [Category("Game Objects/Pooling/Pool Destroy")]
    
    [Parameter("Game Object", "The Game Object reference is used as the template for the pool")]
    
    [Example(
        "Use this Instruction to dispose those pools that have been pre-warmed. " +
        "Pools created at runtime are automatically disposed when their scene is unloaded."
    )]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Red, typeof(OverlayFlame))]
    
    [Keywords("Dispose", "Destroy", "Delete", "Game Object")]
    [Serializable]
    public class InstructionGameObjectPoolDestroy : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Destroy {this.m_GameObject} Pool";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;
            
            PoolManager.Instance.Dispose(gameObject);
            return DefaultResult;
        }
    }
}