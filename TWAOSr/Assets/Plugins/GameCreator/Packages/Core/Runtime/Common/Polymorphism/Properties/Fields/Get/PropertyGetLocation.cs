using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetLocation : TPropertyGet<PropertyTypeGetLocation, Location>
    {
        public PropertyGetLocation() : base(new GetLocationNone())
        { }

        public PropertyGetLocation(PropertyTypeGetLocation defaultType) : base(defaultType)
        { }
    }
}