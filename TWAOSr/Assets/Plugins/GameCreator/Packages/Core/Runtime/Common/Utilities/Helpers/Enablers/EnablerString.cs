using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerString : TEnablerValue<string>
    {
        public EnablerString()
        { }

        public EnablerString(string value) : base(false, value)
        { }
        
        public EnablerString(bool isEnabled, string value) : base(isEnabled, value)
        { }
    }
}