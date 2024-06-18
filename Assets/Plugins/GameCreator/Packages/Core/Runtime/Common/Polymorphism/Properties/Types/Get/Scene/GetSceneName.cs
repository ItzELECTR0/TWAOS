using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scene Name")]
    [Category("Scene Name")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    [Description("The Scene reference by its name or path")]

    [Serializable]
    public class GetSceneName : PropertyTypeGetScene
    {
        [SerializeField] 
        private PropertyGetString m_SceneName = GetStringString.Create;

        public override int Get(Args args) => SceneReference.GetSceneIndex(this.m_SceneName.Get(args));

        public static PropertyGetScene Create => new PropertyGetScene(
            new GetSceneName()
        );

        public override string String => $"{this.m_SceneName}";
    }
}