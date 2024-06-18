using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace GameCreator.Runtime.VisualScripting 
{
    [Version(0, 1, 1)]

    [Title("Load Scene")]
    [Description("Loads a new Scene")]

    [Category("Scenes/Load Scene")]

    [Parameter(
        "Scene",
        "The scene to be loaded"
    )]

    [Parameter(
        "Mode",
        "Single mode replaces all other scenes. Additive mode loads the scene on top of the others"
    )]
    
    [Parameter(
        "Async",
        "Loads the scene in the background or freeze the game until its done"
    )]
    
    [Parameter(
        "Scene Entries",
        "Define the starting location of the player and other characters after loading the scene"
    )]

    [Keywords("Change")]
    [Image(typeof(IconUnity), ColorTheme.Type.TextNormal)]
    
    [Serializable]
    public class InstructionCommonSceneLoad : Instruction
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private PropertyGetScene m_Scene = new PropertyGetScene();
        
        [SerializeField] private LoadSceneMode m_Mode = LoadSceneMode.Single;
        [SerializeField] private bool m_Async = false;
        
        [SerializeField] private SceneEntries m_SceneEntries = new SceneEntries();

        // MEMBERS: -------------------------------------------------------------------------------
        
        private AsyncOperation m_Loader;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Load{0} scene {1}{2}",
            this.m_Mode == LoadSceneMode.Additive ? " additive" : string.Empty,
            this.m_Scene,
            this.m_Async ? " (async)" : string.Empty
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override async Task Run(Args args)
        {
            int scene = this.m_Scene.Get(args);
            this.m_SceneEntries.Schedule(scene, args);
            
            if (this.m_Async)
            {
                this.m_Loader = SceneManager.LoadSceneAsync(scene, this.m_Mode);
                await this.Until(() => this.m_Loader.isDone || ApplicationManager.IsExiting);
            }
            else
            {
                SceneManager.LoadScene(scene, this.m_Mode);
            }
        }
    }
}