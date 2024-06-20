using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Fires when the user searches.
    /// </summary>
    class SearchEvent : EventBase<SearchEvent>
    {
        public string query { get; private set; }

        public static SearchEvent GetPooled(string query)
        {
            var pooled = EventBase<SearchEvent>.GetPooled();
            pooled.query = query;
            return pooled;
        }
    }
}
