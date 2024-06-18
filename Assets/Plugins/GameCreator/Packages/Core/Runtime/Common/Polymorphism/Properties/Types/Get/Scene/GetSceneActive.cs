using System;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common
{
    [Title("Active Scene")]
    [Category("Active Scene")]
    
    [Image(typeof(IconUnity), ColorTheme.Type.TextNormal)]
    [Description("The active Scene reference")]

    [Serializable]
    public class GetSceneActive : PropertyTypeGetScene
    {
        public override int Get(Args args)
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.buildIndex;
        }

        public static PropertyGetScene Create => new PropertyGetScene(
            new GetSceneActive()
        );

        public override string String => "Active Scene";
    }
}