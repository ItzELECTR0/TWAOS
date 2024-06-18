using System.IO;
using System.Text;

namespace GameCreator.Runtime.Common
{
    public static class PathUtils
    {
        public static string Combine(params string[] sections)
        {
            StringBuilder path = new StringBuilder();
            foreach (string section in sections)
            {
                if (string.IsNullOrEmpty(section)) continue;
                if (path.Length > 0 && path[^1] != '/')
                {
                    path.Append('/');
                }

                path.Append(section);
            }

            return path.ToString();
        }

        public static string PathForOS(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        public static string PathToUnix(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}