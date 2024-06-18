using System;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetWeapon : TPropertyGet<PropertyTypeGetWeapon, IWeapon>
    {
        public PropertyGetWeapon() : base(new GetWeaponNone())
        { }

        public PropertyGetWeapon(PropertyTypeGetWeapon defaultType) : base(defaultType)
        { }
    }
}