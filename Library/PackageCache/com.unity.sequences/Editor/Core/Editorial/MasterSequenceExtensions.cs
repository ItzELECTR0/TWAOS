using System.IO;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// This class extends the MasterSequence asset management with Unity Editor basic capabilities: save, delete, rename.
    /// </summary>
    public static class MasterSequenceExtensions
    {
        /// <summary>
        /// Saves the MasterSequence asset on disk.
        /// </summary>
        /// <param name="masterSequence">The MasterSequence asset to save.</param>
        /// <param name="folder">Optional sub-folders of the Assets/Sequences folder to save the MasterSequence asset to.
        /// The method creates the specified sub-folders if they doesn't already exist. If not specified, the
        /// MasterSequence asset is saved in an eponym sub-folder.</param>
        internal static void Save(this MasterSequence masterSequence, string folder = null)
        {
            if (AssetDatabase.Contains(masterSequence))
            {
                SequencesAssetDatabase.SaveAsset(masterSequence);
                return;
            }

            folder = folder ?? masterSequence.name;

            var path = SequencesAssetDatabase.GenerateUniqueMasterSequencePath(masterSequence.name, folder);
            SequencesAssetDatabase.SaveAsset(masterSequence, path);

            masterSequence.rootSequence.Save(folder);

            EditorUtility.SetDirty(masterSequence);
            AssetDatabase.SaveAssetIfDirty(masterSequence);
        }

        /// <summary>
        /// Renames the MasterSequence asset on disk.
        /// </summary>
        /// <param name="masterSequence">The MasterSequence asset to rename.</param>
        /// <param name="newName">The new name to use.</param>
        /// <returns>True if the rename was a success. False otherwise.</returns>
        public static bool Rename(this MasterSequence masterSequence, string newName)
        {
            if (!SequencesAssetDatabase.IsRenameValid(masterSequence.name, newName))
                return false;

            newName = SequencesAssetDatabase.GenerateNewUniqueMasterSequenceName(masterSequence, newName);
            SequencesAssetDatabase.RenameAssetFolder(masterSequence, newName);

            masterSequence.rootSequence.Rename(newName);
            masterSequence.Save();

            SequencesAssetDatabase.RenameAsset(masterSequence, newName);

            return true;
        }

        /// <summary>
        /// Deletes the MasterSequence asset from disk. This also deletes the MasterSequence asset folder if it is
        /// empty after the operation.
        /// </summary>
        /// <param name="masterSequence">The MasterSequence asset to delete.</param>
        public static void Delete(this MasterSequence masterSequence)
        {
            SequenceUtility.DeleteSequence(masterSequence.rootSequence, masterSequence);

            var directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(masterSequence));

            SequencesAssetDatabase.DeleteAsset(masterSequence);
            SequencesAssetDatabase.DeleteFolder(directoryName);

            MasterSequenceRegistryUtility.PruneRegistries();
        }
    }
}
