using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null Weapon reference")]

    [Serializable]
    public class GetWeaponNone : PropertyTypeGetWeapon
    {
        public override IWeapon Get(Args args) => null;
        public override IWeapon Get(GameObject gameObject) => null;

        public static PropertyGetWeapon Create()
        {
            GetWeaponNone instance = new GetWeaponNone();
            return new PropertyGetWeapon(instance);
        }

        public override string String => "None";
    }
}