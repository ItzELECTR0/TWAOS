using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace GameCreator.Runtime.Common
{ 
	public class Trie<T> : IEnumerable<Trie<T>>
	{
        public readonly string id;
        public T Data { get; }
        
        public Trie<T> Parent { get; private set; }
        public Dictionary<string, Trie<T>> Children { get; }

        // INITIALIZE: ----------------------------------------------------------------------------

        private Trie()
        {
            this.id = string.Empty;
            this.Data = default;

            this.Children = new Dictionary<string, Trie<T>>();
        }

        public Trie(string id, T data)
		{
            this.id = id;
			this.Data = data;

            this.Children = new Dictionary<string, Trie<T>>();
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public Trie<T> AddChild(Trie<T> item)
		{
			item.Parent?.Children.Remove(item.id);
			item.Parent = this;

			if (this.Children.ContainsKey(item.id)) return null;
			
			this.Children.Add(item.id, item);
			return this.Children[item.id];
		}

        public static Trie<T> Create()
        {
            return new Trie<T>();
        }

		// STRING METHODS: ------------------------------------------------------------------------

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			BuildString(sb, this, 0);

			return sb.ToString();
		}

		public static string BuildString(Trie<T> trie)
		{
			StringBuilder sb = new StringBuilder();
			BuildString(sb, trie, 0);

			return sb.ToString();
		}

		private static void BuildString(StringBuilder sb, Trie<T> node, int depth)
		{
			sb.AppendLine(node.id.PadLeft(node.id.Length + depth));

			foreach (Trie<T> child in node)
			{
				BuildString(sb, child, depth + 1);
			}
		}

		// ENUMERATOR METHODS: --------------------------------------------------------------------

		public IEnumerator<Trie<T>> GetEnumerator()
		{
			return this.Children.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}