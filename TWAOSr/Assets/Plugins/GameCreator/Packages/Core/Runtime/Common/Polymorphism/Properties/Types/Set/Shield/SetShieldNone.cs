using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetShieldNone : PropertyTypeSetShield
    {
        public override void Set(IShield value, Args args)
        { }
        
        public override void Set(IShield value, GameObject gameObject)
        { }

        public static PropertySetShield Create => new PropertySetShield(
            new SetShieldNone()
        );

        public override string String => "(none)";
    }
}