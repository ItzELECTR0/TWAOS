namespace GameCreator.Runtime.Common
{
    public enum LoadSceneMode
    {
        /// <summary>
        /// Loads the main and all additive scenes saved
        /// </summary>
        AllSavedScenes = 0,
        
        /// <summary>
        /// Loads the main scene but not the additive saved ones
        /// </summary>
        MainSavedScene = 1,
        
        /// <summary>
        /// Loads a specific scene, regardless of the saved scenes
        /// </summary>
        Scene = 2
    }
}