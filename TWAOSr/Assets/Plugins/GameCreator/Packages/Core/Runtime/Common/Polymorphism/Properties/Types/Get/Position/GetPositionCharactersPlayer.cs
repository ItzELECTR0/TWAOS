using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Player Position")]
    [Category("Characters/Player Position")]
    
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]
    [Description("Returns the position of the Player character")]

    [Serializable]
    public class GetPositionCharactersPlayer : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            Transform transform = ShortcutPlayer.Transform;
            return transform != null ? transform.position : default;
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            Transform transform = ShortcutPlayer.Transform;
            return transform != null ? transform.position : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionCharactersPlayer()
        );

        public override string String => "Player";
    }
}