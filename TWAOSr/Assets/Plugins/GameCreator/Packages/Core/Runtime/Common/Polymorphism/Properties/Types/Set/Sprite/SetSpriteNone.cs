using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetSpriteNone : PropertyTypeSetSprite
    {
        public override void Set(Sprite value, Args args)
        { }
        
        public override void Set(Sprite value, GameObject gameObject)
        { }

        public static PropertySetSprite Create => new PropertySetSprite(
            new SetSpriteNone()
        );

        public override string String => "(none)";
    }
}