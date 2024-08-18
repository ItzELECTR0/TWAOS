using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scene Asset")]
    [Category("Scene Asset")]
    
    [Image(typeof(IconUnity), ColorTheme.Type.TextNormal)]
    [Description("The Scene reference by referencing the project scene object")]

    [Serializable]
    public class GetSceneAsset : PropertyTypeGetScene
    {
        [SerializeField] protected SceneReference m_Scene = new SceneReference();

        public override int Get(Args args) => this.m_Scene.Index;
        public override int Get(GameObject gameObject) => this.m_Scene.Index;

        public static PropertyGetScene Create => new PropertyGetScene(
            new GetSceneAsset()
        );

        public override string String => this.m_Scene.ToString();
    }
}