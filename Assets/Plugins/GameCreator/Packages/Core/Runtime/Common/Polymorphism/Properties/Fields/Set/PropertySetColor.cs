using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetColor : TPropertySet<PropertyTypeSetColor, Color>
    {
        public PropertySetColor() : base(new SetColorNone())
        { }

        public PropertySetColor(PropertyTypeSetColor defaultType) : base(defaultType)
        { }
    }
}