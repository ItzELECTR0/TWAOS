using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green)]
    
    [Title("Exists")]
    [Category("Game Object/Exists")]
    
    [Description("Remembers if the object was destroyed or not")]

    [Serializable]
    public class MemoryExists : Memory
    {
        public override string Title => "Exists";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            return new TokenExists(target);
        }

        public override void OnRemember(GameObject target, Token token)
        {
            if (token is TokenExists { Exists: false })
            {
                UnityEngine.Object.Destroy(target);
            }
        }
    }
}