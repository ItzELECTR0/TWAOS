using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerDouble : TEnablerValue<double>
    {
        public EnablerDouble()
        { }

        public EnablerDouble(double value) : base(false, value)
        { }

        public EnablerDouble(bool isEnabled, double value) : base(isEnabled, value)
        { }
    }
}