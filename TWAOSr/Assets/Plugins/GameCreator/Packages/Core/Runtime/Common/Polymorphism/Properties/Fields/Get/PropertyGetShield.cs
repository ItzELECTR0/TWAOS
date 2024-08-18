using System;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetShield : TPropertyGet<PropertyTypeGetShield, IShield>
    {
        public PropertyGetShield() : base(new GetShieldNone())
        { }

        public PropertyGetShield(PropertyTypeGetShield defaultType) : base(defaultType)
        { }
    }
}