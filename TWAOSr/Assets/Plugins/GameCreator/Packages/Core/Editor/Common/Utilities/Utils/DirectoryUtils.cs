using UnityEngine;
using System.IO;
using System.Text;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    public static class DirectoryUtils
    {
        public static void RequirePath(string path)
        {
            RequirePath(path, true);
        }
        
        public static void RequireFilepath(string path)
        {
            RequirePath(path, false);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static void RequirePath(string path, bool includeLast)
        {
            string[] folders = path.Split('/');
            string previous = string.Empty;

            int foldersCount = includeLast 
                ? folders.Length 
                : folders.Length - 1;
            
            for (int i = 0; i < foldersCount; i++)
            {
                string folder = folders[i];
                string trail = PathUtils.Combine(previous, folder);
                if (!AssetDatabase.IsValidFolder(trail))
                {
                    AssetDatabase.CreateFolder(previous, folder);
                }

                AssetDatabase.Refresh();
                previous = trail;
            }
        }
    }
}