using System.IO;
using System.Linq;

namespace UnityEditor.Sequences
{
    internal static class FilePathUtility
    {
        /// <summary>
        /// Characters that are not allowed in filenames. Path.GetInvalidFileNameChars is inconsistent across platforms
        /// and omits some invalid characters on macOS, so we use this instead.
        /// Copied from https://github.cds.internal.unity3d.com/unity/unity/blob/ed0ccb93f471897bfd854e129c3873e3a4dbe06f/Runtime/Utilities/PathNameUtility.cpp#L467
        /// </summary>
        static readonly char[] k_InvalidFileNameChars = "/?<>\\:*|\"".ToCharArray();

        static readonly char k_InvalidFileNameCharReplacement = '_';

        /// <summary>
        /// Trims whitespace and substitutes invalid characters in the provided file name.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>File name with invalid characters replaced.</returns>
        internal static string SanitizeFileName(string fileName)
        {
            foreach (var c in k_InvalidFileNameChars)
                fileName = fileName.Replace(c, k_InvalidFileNameCharReplacement);

            return fileName.Trim();
        }

        /// <summary>
        /// Sanitizes the specified new name by replacing invalid file characters by underscores and then verifies that
        /// it is a valid name (i.e. it is not empty or only white spaces and not equals to the old name).
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="sanitizedName"></param>
        /// <returns>Returns True if the sanitized new name is valid.</returns>
        internal static bool SanitizeAndValidateName(string oldName, string newName, out string sanitizedName)
        {
            sanitizedName = SanitizeFileName(newName);
            return !string.IsNullOrWhiteSpace(sanitizedName) && oldName != sanitizedName;
        }

        /// <summary>
        /// Generate a unique asset path based on the specified path. It also extract the unique name from the
        /// generated path.
        /// </summary>
        /// <param name="path">The path to use as a base to generate a unique path.</param>
        /// <param name="uniquePath">The unique path generated</param>
        /// <param name="uniqueName">The unique name generated. If the specified path is a path to a file with an
        /// extenstion, the unique name doesn't contain it.</param>
        internal static void GenerateUniqueAssetPathAndName(string path, out string uniquePath, out string uniqueName)
        {
            uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
            uniqueName = Path.GetFileNameWithoutExtension(uniquePath);
        }

        /// <summary>
        /// Check if the folder at the specified folder's path is empty or not. If the folder exclusively contains
        /// meta file, it is considered empty.
        /// </summary>
        /// <param name="folderPath">The path to the folder to check.</param>
        /// <param name="recursive">Whether or not the check sub-folders as well. If all the sub-folders are
        /// empty, the specified one is also considered empty. If recursive is false then sub-directories are not
        /// inspected and they are considered like regular files.</param>
        /// <returns>True if the specified folder is empty and if all its sub-folders (if any) are also empty. It is
        /// also true if all the files found are meta files.
        /// If any regular files are found or if recursive is false and there are sub-directories, it returns false.</returns>
        internal static bool IsFolderEmpty(string folderPath, bool recursive = true)
        {
            if (recursive)
            {
                foreach (var subDirector in Directory.EnumerateDirectories(folderPath))
                {
                    if (!IsFolderEmpty(subDirector, recursive))
                        return false;
                }
            }
            else if (Directory.GetDirectories(folderPath).Length > 0)
                return false;

            var files = Directory.GetFiles(folderPath);

            if (files.Length == 0 || files.Length > 0 && files.All(path => path.EndsWith(".meta")))
                return true; // Not files are found or they are only meta files, the folder is considered empty.

            return false;
        }
    }
}
