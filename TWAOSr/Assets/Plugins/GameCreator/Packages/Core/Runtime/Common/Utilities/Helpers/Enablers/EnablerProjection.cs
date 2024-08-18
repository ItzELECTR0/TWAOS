using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerProjection : TEnablerValue<EnablerProjection.Type>
    {
        public enum Type
        {
            Perspective,
            Orthographic
        }
        
        public EnablerProjection()
        { }

        public EnablerProjection(Type value) : base(false, value)
        { }

        public EnablerProjection(bool isEnabled, Type value) : base(isEnabled, value)
        { }
    }
}