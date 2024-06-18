using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("Game Objects/None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null Sprite reference")]

    [Keywords("Null", "Empty")]
    
    [Serializable]
    public class GetSpriteNone : PropertyTypeGetSprite
    {
        public override Sprite Get(Args args) => null;
        public override Sprite Get(GameObject gameObject) => null;

        public static PropertyGetSprite Create => new PropertyGetSprite(
            new GetSpriteNone()
        );

        public override string String => "None";
    }
}