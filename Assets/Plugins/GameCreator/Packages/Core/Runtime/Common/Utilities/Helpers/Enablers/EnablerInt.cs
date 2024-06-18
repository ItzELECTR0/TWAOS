using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerInt : TEnablerValue<int>
    {
        public EnablerInt()
        { }

        public EnablerInt(int value) : base(false, value)
        { }

        public EnablerInt(bool isEnabled, int value) : base(isEnabled, value)
        { }
    }
}