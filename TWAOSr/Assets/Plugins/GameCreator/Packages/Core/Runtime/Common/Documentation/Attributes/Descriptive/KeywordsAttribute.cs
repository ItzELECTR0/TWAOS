using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true
    )]
    public class KeywordsAttribute : Attribute, ISearchable
    {
        public string[] Keywords { get; }

        public KeywordsAttribute(params string[] keywords)
        {
            for (int i = 0; i < keywords.Length; ++i)
            {
                keywords[i] = keywords[i].Trim();
            }

            this.Keywords = keywords;
        }

        public string SearchText => string.Join(" ", this.Keywords);
        public int SearchPriority => 7;
    }
}
