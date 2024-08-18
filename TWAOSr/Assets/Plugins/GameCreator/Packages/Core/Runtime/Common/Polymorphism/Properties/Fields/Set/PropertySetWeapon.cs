using System;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetWeapon : TPropertySet<PropertyTypeSetWeapon, IWeapon>
    {
        public PropertySetWeapon() : base(new SetWeaponNone())
        { }

        public PropertySetWeapon(PropertyTypeSetWeapon defaultType) : base(defaultType)
        { }
    }
}