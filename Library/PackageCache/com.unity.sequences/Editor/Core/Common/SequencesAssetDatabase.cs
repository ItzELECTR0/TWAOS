using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Sequences
{
    // The SequencesAssetDatabase encapsulate the Unity AssetDatabase functions and can be overriden
    // to handle any way of handling assets (for example, it could connect to Shotgun or Perforce instead of using
    // the AssetDatabase from Unity).
    internal static class SequencesAssetDatabase
    {
        internal static readonly string k_SequenceBaseFolder = "Sequences";

        public static string GenerateUniqueMasterSequencePath(string name, string subFolders = "", string extension = ".asset")
        {
            return GetNewAssetPath(name, k_SequenceBaseFolder, subFolders, extension);
        }

        public static void SaveAsset<T>(T asset, string path = null) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
            }
            else
                AssetDatabase.CreateAsset(asset, path);
        }

        public static void SaveAsset(TimelineAsset asset, string path = null)
        {
            if (!string.IsNullOrEmpty(path))
                ValidatePath(path, asset, ".playable");

            SaveAsset<TimelineAsset>(asset, path);
        }

        public static bool DeleteAsset<T>(T asset) where T : Object
        {
            return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
        }

        /// <summary>
        /// Delete the folder at the given path.
        /// </summary>
        /// <param name="path">The path of the folder to delete.</param>
        /// <returns>Whether the delete of the folder was a success or not.</returns>
        public static bool DeleteFolder(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return false;

            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                if (FilePathUtility.IsFolderEmpty(directory))
                    AssetDatabase.DeleteAsset(directory);
            }

            return FilePathUtility.IsFolderEmpty(path) && AssetDatabase.DeleteAsset(path);
        }

        public static string RenameAsset<T>(T asset, string newName) where T : Object
        {
            if (asset == null)
                throw new NullReferenceException("Cannot rename null asset.");

            var path = AssetDatabase.GetAssetPath(asset);
            var folder = Path.GetDirectoryName(path);
            var newpath = Path.Combine(folder, newName + Path.GetExtension(path));

            newpath = AssetDatabase.GenerateUniqueAssetPath(newpath);
            newName = Path.GetFileNameWithoutExtension(newpath);

            var errorMessage = AssetDatabase.RenameAsset(path, newName);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new OperationCanceledException("Rename Asset failed.\n" + errorMessage);

            return newName;
        }

        public static IEnumerable<T> FindAsset<T>(string name = null) where T : Object
        {
            string filter = !string.IsNullOrEmpty(name) ? $"{name} t:{typeof(T).ToString()}" : $"t:{typeof(T).ToString()}";
            var guids = AssetDatabase.FindAssets(filter);

            foreach (var guid in guids)
                yield return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        public static string GenerateNewUniqueMasterSequenceName(MasterSequence asset, string newName)
        {
            var oldFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));
            var newFolderPath = Path.Combine(Path.GetDirectoryName(oldFolderPath), newName);
            newFolderPath = AssetDatabase.GenerateUniqueAssetPath(newFolderPath);

            return Path.GetFileName(newFolderPath);
        }

        public static string RenameAssetFolder<T>(T asset, string newName) where T : Object
        {
            var oldFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));
            var oldFolderName = Path.GetFileName(oldFolderPath);

            // Check if folder name has been manually changed by user. If so, don't rename folder
            if (oldFolderName != asset.name)
                return oldFolderPath;

            var newFolderPath = Path.Combine(Path.GetDirectoryName(oldFolderPath), newName);
            var newFolderName = Path.GetFileName(newFolderPath);
            AssetDatabase.RenameAsset(oldFolderPath, newFolderName);

            return newFolderPath;
        }

        public static bool IsRenameValid(string oldName, string newName)
        {
            return !(string.IsNullOrWhiteSpace(newName) || newName == oldName);
        }

        internal static string GetSequenceFolder(TimelineSequence sequence)
        {
            if (TimelineSequence.IsNullOrEmpty(sequence) ||
                TimelineSequence.IsNullOrEmpty(sequence.parent as TimelineSequence))
            {
                return "";
            }

            var assetPath = AssetDatabase.GetAssetPath(sequence.timeline);
            if (assetPath == "")
                return "";

            var folderPath = Path.Combine(Path.GetDirectoryName(assetPath), sequence.name);
            if (!AssetDatabase.IsValidFolder(folderPath))
                return "";

            return folderPath;
        }

        internal static void RenameSequenceFolder(TimelineSequence sequence, string newName)
        {
            var sequenceFolderPath = GetSequenceFolder(sequence);
            if (!string.IsNullOrEmpty(sequenceFolderPath))
                AssetDatabase.RenameAsset(sequenceFolderPath, newName);
        }

        /// <summary>
        /// Compute and return the path that represent the Sequence context of the given clip.
        /// e.g.: Given clip is sub-sequence with name "aa", it's sequence is "A", in the MasterSequence "Cine",
        /// Sequence path is "Cine/A/aa".
        /// </summary>
        /// <param name="contextSequence">The clip for which to compute its Sequence context path.</param>
        /// <returns>A string that represent the Sequence context path of the given clip,
        /// e.g. "<mastersequence_name>[/<sequence_name>[/<subsequence_name>]]".</returns>
        internal static string GetSequenceContextPath(Sequence contextSequence)
        {
            var path = contextSequence.parent != null ? GetSequenceContextPath(contextSequence.parent) : "";
            return Path.Combine(path, contextSequence.name);
        }

        static string GetNewAssetPath(string name, string basePath, string subFolders = "", string extension = ".asset")
        {
            string folderPath = Path.Combine("Assets", basePath);

            if (!string.IsNullOrEmpty(subFolders))
                folderPath = Path.Combine(folderPath, subFolders);

            string absoluteFolderPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), folderPath);

            string assetPath = Path.Combine(folderPath, FilePathUtility.SanitizeFileName(name) + extension);

            if (!Directory.Exists(absoluteFolderPath))
                Directory.CreateDirectory(folderPath);

            return AssetDatabase.GenerateUniqueAssetPath(assetPath);
        }

        static void ValidatePath<T>(string path, T asset, string expectedExtension)
        {
            if (Path.GetExtension(path) != expectedExtension)
                throw new ArgumentException(
                    $"Invalid path, the path for a {asset.GetType().ToString()} " +
                    $"should end with the {expectedExtension} extension.");
        }

        /// <summary>
        /// Scan all prefabs in the AssetDatabase and return the SequenceAssets.
        /// This is costly. If relying on the Indexer suffice, please use <see cref="SequenceAssetIndexer.indexes"/> instead.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GameObject> FindAllSequenceAssets()
        {
            var prefabGuids = AssetDatabase.FindAssets("glob:\"*.prefab\"");
            foreach (var guid in prefabGuids)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (go == null)
                    continue;

                if (go.GetComponent<SequenceAsset>() == null)
                    continue;

                yield return go;
            }
        }
    }
}
