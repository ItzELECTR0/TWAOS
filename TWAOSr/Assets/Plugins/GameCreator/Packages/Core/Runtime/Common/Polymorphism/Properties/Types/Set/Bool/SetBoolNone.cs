using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetBoolNone : PropertyTypeSetBool
    {
        public override void Set(bool value, Args args)
        { }
        
        public override void Set(bool value, GameObject gameObject)
        { }

        public static PropertySetBool Create => new PropertySetBool(
            new SetBoolNone()
        );

        public override string String => "(none)";
    }
}