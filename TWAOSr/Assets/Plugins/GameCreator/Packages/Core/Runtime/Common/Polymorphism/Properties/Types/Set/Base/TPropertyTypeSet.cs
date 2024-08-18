using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconCircleSolid), ColorTheme.Type.Green)]

    public abstract class TPropertyTypeSet<T>
    {
        public virtual void Set(T value, Args args)
        { }

        public virtual void Set(T value, GameObject gameObject)
        {
            this.Set(value, new Args(gameObject));
        }

        public virtual T Get(Args args)
        {
            return default;
        }

        public virtual T Get(GameObject gameObject)
        {
            return this.Get(new Args(gameObject));
        }

        public abstract string String { get; }
    }
}