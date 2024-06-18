using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetBool : TPropertySet<PropertyTypeSetBool, bool>
    {
        public PropertySetBool() : base(new SetBoolNone())
        { }

        public PropertySetBool(PropertyTypeSetBool defaultType) : base(defaultType)
        { }
    }
}