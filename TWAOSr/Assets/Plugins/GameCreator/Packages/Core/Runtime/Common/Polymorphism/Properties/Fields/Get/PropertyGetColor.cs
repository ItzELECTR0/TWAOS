using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetColor : TPropertyGet<PropertyTypeGetColor, Color>
    {
        public PropertyGetColor() : base(new GetColorValue())
        { }

        public PropertyGetColor(PropertyTypeGetColor defaultType) : base(defaultType)
        { }

        public PropertyGetColor(Color value) : base(new GetColorValue(value))
        { }
    }
}