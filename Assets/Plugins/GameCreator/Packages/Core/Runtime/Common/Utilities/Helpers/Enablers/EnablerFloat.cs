using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerFloat : TEnablerValue<float>
    {
        public EnablerFloat()
        { }

        public EnablerFloat(float value) : base(false, value)
        { }

        public EnablerFloat(bool isEnabled, float value) : base(isEnabled, value)
        { }
    }
}