using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Scene Loaded")]
    [Description("Returns true if the scene has been loaded")]

    [Parameter("Scene", "The Unity Scene reference used in the condition")]
    
    [Category("Scenes/Is Scene Loaded")]

    [Image(typeof(IconUnity), ColorTheme.Type.TextNormal)]
    [Serializable]
    public class ConditionScenesIsLoaded : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetScene m_Scene = new PropertyGetScene();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is scene {this.m_Scene} Loaded";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            int index = this.m_Scene.Get(args);
            Scene scene = SceneManager.GetSceneByBuildIndex(index);
            
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
