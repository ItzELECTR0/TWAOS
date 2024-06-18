using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scene Index")]
    [Category("Scene Index")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.Blue)]
    [Description("The Scene reference by its index in the Build Settings")]

    [Serializable]
    public class GetSceneIndex : PropertyTypeGetScene
    {
        [SerializeField] 
        protected PropertyGetInteger m_Index = GetDecimalInteger.Create(0);

        public override int Get(Args args) => (int) this.m_Index.Get(args);

        public static PropertyGetScene Create => new PropertyGetScene(
            new GetSceneIndex()
        );

        public override string String => this.m_Index.ToString();
    }
}