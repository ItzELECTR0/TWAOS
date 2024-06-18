using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetNumber : TPropertySet<PropertyTypeSetNumber, double>
    {
        public PropertySetNumber() : base(new SetNumberNone())
        { }

        public PropertySetNumber(PropertyTypeSetNumber defaultType) : base(defaultType)
        { }
    }
}