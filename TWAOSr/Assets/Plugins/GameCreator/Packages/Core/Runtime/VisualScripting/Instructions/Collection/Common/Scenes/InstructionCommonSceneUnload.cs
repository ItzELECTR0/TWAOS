using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.VisualScripting 
{
    [Version(0, 1, 1)]

    [Title("Unload Scene")]
    [Description("Unloads an active scene")]

    [Category("Scenes/Unload Scene")]

    [Parameter(
        "Scene",
        "The scene to be unloaded"
    )]

    [Keywords("Change", "Remove")]
    [Image(typeof(IconUnity), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InstructionCommonSceneUnload : Instruction
    {
        [SerializeField] private PropertyGetScene m_Scene = new PropertyGetScene();

        // MEMBERS: -------------------------------------------------------------------------------
        
        private AsyncOperation m_AsyncOperation;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Unload scene {this.m_Scene}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            int scene = this.m_Scene.Get(args);

            this.m_AsyncOperation = SceneManager.UnloadSceneAsync(scene);
            await this.Until(() => this.m_AsyncOperation.isDone);
        }
    }
}