using System;
using System.Collections.Generic;

namespace GameCreator.Runtime.Common
{
    public class Ring<T>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public T[] Buffer { get; }
        public int Length => this.Buffer.Length;

        public int Index { get; protected set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Ring()
        {
            this.Buffer = Array.Empty<T>();
            this.Index = 0;
        }

        public Ring(int capacity)
        {
            this.Buffer = new T[capacity];
        }

        public Ring(IReadOnlyList<T> array) : this(array.Count)
        {
            for (int i = 0; i < array.Count; ++i)
            {
                this.Buffer[i] = array[i];
            }
        }

        public Ring(List<T> list) : this(list.Count)
        {
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                this.Buffer[i] = list[i];
            }
        }

        public Ring(params T[] array) : this(array.Length)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                this.Buffer[i] = array[i];
            }
        }

        public Ring(IEnumerable<T> collection) : this(new List<T>(collection))
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(Action<T> action)
        {
            for (int i = 0; i < this.Length; ++i)
            {
                action.Invoke(this.Buffer[i]);
            }
        }

        public void Reset()
        {
            this.Index = 0;
        }

        public T Current()
        {
            return this.Buffer[this.Index];
        }

        public T Next()
        {
            this.Index = ++this.Index >= this.Length ? 0 : this.Index;
            return this.Current();
        }

        public T Previous()
        {
            this.Index = --this.Index < 0 ? this.Length - 1 : this.Index;
            return this.Current();
        }
    }
}