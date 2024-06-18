using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PercentageWithLabel : TPercentage
    {
        public PercentageWithLabel() : base()
        { }
        
        public PercentageWithLabel(float unit) : base(unit)
        { }
    }
}