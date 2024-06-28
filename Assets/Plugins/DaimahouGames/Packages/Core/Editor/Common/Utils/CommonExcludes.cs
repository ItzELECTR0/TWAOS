using System.Linq;

namespace DaimahouGames.Editor.Core
{
    public static class CommonExcludes
    {
        private const string SCRIPT = "m_Script";

        public static string[] Get() => new[] {SCRIPT};
        public static string[] Concat(params string[] exclude) => Get().Concat(exclude).ToArray();
    }
}