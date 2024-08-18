using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetString : TPropertyGet<PropertyTypeGetString, string>
    {
        public PropertyGetString() : base(new GetStringString())
        { }

        public PropertyGetString(PropertyTypeGetString defaultType) : base(defaultType)
        { }

        public PropertyGetString(string value) : base(new GetStringString(value))
        { }
    }
}