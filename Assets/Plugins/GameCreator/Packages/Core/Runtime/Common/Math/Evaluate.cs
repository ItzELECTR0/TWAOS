using System;
using GameCreator.Runtime.Common.Mathematics;

namespace GameCreator.Runtime.Common
{
    // TODO: [11/06/2023] Remove in the next breaking change update 
    
    [Obsolete(
        "This class is deprecated and will soon be removed. " +
        "The new math library has been moved to Stats module"
    )]
    
    public static class Evaluate
    {
        public static float FromString(string expression)
        {
            return Parser.Evaluate(expression);
        }
    }
}
