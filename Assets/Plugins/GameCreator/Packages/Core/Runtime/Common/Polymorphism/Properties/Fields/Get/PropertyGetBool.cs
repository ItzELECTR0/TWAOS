using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetBool : TPropertyGet<PropertyTypeGetBool, bool>
    {
        public PropertyGetBool() : base(new GetBoolValue())
        { }

        public PropertyGetBool(PropertyTypeGetBool defaultType) : base(defaultType)
        { }

        public PropertyGetBool(bool value) : base(new GetBoolValue(value))
        { }
    }
}