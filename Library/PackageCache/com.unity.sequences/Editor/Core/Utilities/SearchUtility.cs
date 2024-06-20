using System.Globalization;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Helper functions for searching through sequences.
    /// </summary>
    static class SearchUtility
    {
        /// <summary>
        /// Checks if a string matches a search query (case-insensitive).
        /// </summary>
        internal static bool DoesTextMatchQuery(string text, string query)
        {
            return GetIndexOfQueryMatch(text, query) != -1;
        }

        static int GetIndexOfQueryMatch(string text, string query)
        {
            // Treat accented characters the same as non-accented versions (e.g. "e" matches "é" and vice versa).
            // https://stackoverflow.com/a/15178962
            const CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, query, compareOptions);
        }
    }
}
