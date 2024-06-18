using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetString : TPropertySet<PropertyTypeSetString, string>
    {
        public PropertySetString() : base(new SetStringNone())
        { }

        public PropertySetString(PropertyTypeSetString defaultType) : base(defaultType)
        { }
    }
}