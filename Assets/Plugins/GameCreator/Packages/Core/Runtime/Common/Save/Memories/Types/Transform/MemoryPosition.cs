using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    
    [Title("Position")]
    [Category("Transform/Position")]
    
    [Description("Remembers the position of the object")]

    [Serializable]
    public class MemoryPosition : Memory
    {
        public override string Title => "Position";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            return new TokenPosition(target);
        }

        public override void OnRemember(GameObject target, Token token)
        {
            if (token is not TokenPosition tokenPosition) return;
            
            Character character = target.Get<Character>();
            if (character != null)
            {
                character.Driver.SetPosition(tokenPosition.Position);
                return;
            }
            
            target.transform.position = tokenPosition.Position;
        }
    }
}