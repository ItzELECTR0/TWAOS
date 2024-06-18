using System;
using System.Text;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CategoryAttribute : Attribute, ISearchable
    {
        private static readonly char[] SEPARATOR = { '/' };

        public string Name   { get; }
        public string[] Path { get; }

		public CategoryAttribute(string category)
        {
            string[] categories = category.Split(SEPARATOR);
            this.Name = categories[^1];

            Path = new string[categories.Length - 1];
            for (int i = 0; i < categories.Length - 1; ++i)
            {
                Path[i] = categories[i];
            }
        }

        public override string ToString()
        {
            return this.ToString("/");
        }

        public string ToString(string separator)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string path in this.Path)
            {
                builder.Append(path).Append(separator);
            }

            builder.Append(this.Name);
            return builder.ToString();
        }

        public string SearchText => this.ToString(" ");
        public int SearchPriority => 8;
    }
}
