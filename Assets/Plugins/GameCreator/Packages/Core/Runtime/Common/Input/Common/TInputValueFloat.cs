using System;

namespace GameCreator.Runtime.Common
{
    [Title("Input Value")]
    
    [Serializable]
    public abstract class TInputValueFloat : TInputValue<float>
    {
        public abstract override float Read();
    }
}