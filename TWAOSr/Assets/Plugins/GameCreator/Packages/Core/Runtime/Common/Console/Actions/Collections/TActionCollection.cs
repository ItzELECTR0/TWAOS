using System.Collections.Generic;

namespace GameCreator.Runtime.Console
{
    public abstract class TActionCollection<T>
    {
        public abstract IEnumerable<T> Get { get; }
    }
}