using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Pool Prewarm")]
    [Description("Creates or makes sure an existing game object pool has enough instances")]

    [Category("Game Objects/Pooling/Pool Prewarm")]
    
    [Parameter("Game Object", "The Game Object reference is used as the template for the pool")]
    [Parameter("Pool Size", "The size of the pool of game objects")]
    
    [Example(
        "Pre-warming a Pool moves it to the DontDestroyOnLoad scene. " +
        "This means its contents will never be destroyed even after loading new scenes. " +
        "To delete a pre-warmed pool use the Pool Destroy instruction."
    )]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Green, typeof(OverlayFlame))]
    
    [Keywords("Create", "New", "Initialize", "Game Object")]
    [Serializable]
    public class InstructionGameObjectPoolPrewarm : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();

        [SerializeField]
        private PropertyGetInteger m_PoolSize = new PropertyGetInteger(5);
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Prewarm {this.m_GameObject} with {this.m_PoolSize}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            int poolSize = (int) this.m_PoolSize.Get(args);
            if (poolSize <= 0) return DefaultResult;
            
            PoolManager.Instance.Prewarm(gameObject, poolSize);
            PoolManager.Instance.DontDestroyOnLoadPool(gameObject);
            
            return DefaultResult;
        }
    }
}