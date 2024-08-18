using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconTag), ColorTheme.Type.Green)]
    
    [Title("Tag")]
    [Category("Game Object/Tag")]
    
    [Description("Remembers the tag of the object")]

    [Serializable]
    public class MemoryTag : Memory
    {
        public override string Title => "Tag";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            return new TokenTag(target);
        }

        public override void OnRemember(GameObject target, Token token)
        {
            if (token is TokenTag tokenTag)
            {
                target.tag = tokenTag.Tag;
            }
        }
    }
}