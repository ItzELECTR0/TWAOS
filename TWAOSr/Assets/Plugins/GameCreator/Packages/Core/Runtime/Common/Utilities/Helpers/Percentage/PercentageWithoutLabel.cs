using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PercentageWithoutLabel : TPercentage
    {
        public PercentageWithoutLabel() : base()
        { }
        
        public PercentageWithoutLabel(float unit) : base(unit)
        { }
    }
}