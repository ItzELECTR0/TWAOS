using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class TitleAttribute : Attribute, ISearchable
    {
        public string Title { get; }

        public TitleAttribute(string title)
        {
            this.Title = title.Trim();
        }

        public override string ToString()
        {
            return this.Title;
        }

        public string SearchText => this.Title;
        public int SearchPriority => 10;
    }
}
