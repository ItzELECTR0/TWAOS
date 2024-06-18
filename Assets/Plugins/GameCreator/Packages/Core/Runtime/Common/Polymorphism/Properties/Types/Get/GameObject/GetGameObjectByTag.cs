using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Find by Tag")]
    [Category("Game Objects/Find by Tag")]
    
    [Image(typeof(IconTag), ColorTheme.Type.Yellow)]
    [Description("Searches the scene for a Game Object with a specific tag")]

    [Serializable]
    public class GetGameObjectByTag : PropertyTypeGetGameObject
    {
        [SerializeField] protected PropertyGetString m_Tag = new PropertyGetString("");

        public override GameObject Get(Args args)
        {
            string tag = this.m_Tag.Get(args);
            return GameObject.FindWithTag(tag);
        }

        public override string String => this.m_Tag.ToString();

        public override GameObject EditorValue => GameObject.FindWithTag(this.m_Tag.ToString());
    }
}