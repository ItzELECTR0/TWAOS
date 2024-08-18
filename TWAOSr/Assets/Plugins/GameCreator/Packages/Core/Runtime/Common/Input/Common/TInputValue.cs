using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TInputValue<T> : TInput where T : struct
    {
        public abstract T Read();
    }
}