using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetColorNone : PropertyTypeSetColor
    {
        public override void Set(Color value, Args args)
        { }
        
        public override void Set(Color value, GameObject gameObject)
        { }

        public static PropertySetColor Create => new PropertySetColor(
            new SetColorNone()
        );

        public override string String => "(none)";
    }
}