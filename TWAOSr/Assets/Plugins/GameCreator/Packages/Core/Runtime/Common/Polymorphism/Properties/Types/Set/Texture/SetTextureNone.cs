using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetTextureNone : PropertyTypeSetTexture
    {
        public override void Set(Texture value, Args args)
        { }
        
        public override void Set(Texture value, GameObject gameObject)
        { }

        public static PropertySetTexture Create => new PropertySetTexture(
            new SetTextureNone()
        );

        public override string String => "(none)";
    }
}