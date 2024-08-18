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
    public class SetWeaponNone : PropertyTypeSetWeapon
    {
        public override void Set(IWeapon value, Args args)
        { }
        
        public override void Set(IWeapon value, GameObject gameObject)
        { }

        public static PropertySetWeapon Create => new PropertySetWeapon(
            new SetWeaponNone()
        );

        public override string String => "(none)";
    }
}