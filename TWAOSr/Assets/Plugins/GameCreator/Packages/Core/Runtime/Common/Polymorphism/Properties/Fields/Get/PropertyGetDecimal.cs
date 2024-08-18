using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetDecimal : TPropertyGet<PropertyTypeGetDecimal, double>
    {
        public PropertyGetDecimal() : base(new GetDecimalDecimal())
        { }

        public PropertyGetDecimal(PropertyTypeGetDecimal defaultType) : base(defaultType)
        { }

        public PropertyGetDecimal(double value) : base(new GetDecimalDecimal(value))
        { }
        
        public PropertyGetDecimal(float value) : base(new GetDecimalDecimal(value))
        { }
    }
}