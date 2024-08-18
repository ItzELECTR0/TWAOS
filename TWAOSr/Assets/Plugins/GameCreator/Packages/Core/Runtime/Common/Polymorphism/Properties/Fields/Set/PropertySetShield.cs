using System;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetShield : TPropertySet<PropertyTypeSetShield, IShield>
    {
        public PropertySetShield() : base(new SetShieldNone())
        { }

        public PropertySetShield(PropertyTypeSetShield defaultType) : base(defaultType)
        { }
    }
}