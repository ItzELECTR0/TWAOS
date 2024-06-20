using System;
using System.IO;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// AssetPostprocessor in charge of tracking creation, rename, move and deletion
    /// of assets manipulated by Sequences: <see cref="TimelineAsset"/> and Prefab assets.
    /// </summary>
    class SequencesAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (IsTimelineAsset(path))
                    ProcessImportedTimeline(path);
                else if (IsPrefab(path))
                    ProcessImportedPrefab(path);
            }

            foreach (string path in movedAssets)
            {
                if (ArrayUtility.Contains<string>(importedAssets, path))
                    continue;

                if (IsTimelineAsset(path))
                    ProcessImportedTimeline(path);
                else if (IsPrefab(path))
                    ProcessMovedPrefab(path);
            }

            // A deleted .asset file might have been a master sequence; we can't actually tell at this stage.
            // Fortunately a false positive is harmless.
            if (Array.Exists(deletedAssets, a => Path.GetExtension(a) == ".asset"))
                MasterSequenceUtility.LegacyMasterSequenceRemoved();

            // If there is at least one deleted prefab, check the indexed prefab to remove all the deleted ones.
            if (Array.Exists(deletedAssets, a => Path.GetExtension(a) == ".playable"))
                SequenceIndexer.instance.PruneDeletedElement();

            if (Array.Exists(deletedAssets, a => Path.GetExtension(a) == ".prefab"))
                SequenceAssetIndexer.instance.PruneDeletedSequenceAsset();
        }

        static bool IsTimelineAsset(string path)
        {
            return Path.GetExtension(path) == ".playable";
        }

        static void ProcessImportedTimeline(string path)
        {
            TimelineAsset timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
            SequenceIndexer.instance.TraverseAndProcess(timeline);
        }

        static bool IsPrefab(string path)
        {
            return Path.GetExtension(path) == ".prefab";
        }

        static void ProcessImportedPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!SequenceAssetUtility.IsSequenceAsset(prefab))
                return;

            if (!SequenceAssetIndexer.instance.IsRegistered(prefab))
                SequenceAssetIndexer.instance.AddSequenceAsset(prefab);
            else
                SequenceAssetIndexer.instance.UpdateSequenceAsset(prefab);
        }

        static void ProcessMovedPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!SequenceAssetUtility.IsSequenceAsset(prefab))
                return;

            if (SequenceAssetIndexer.instance.IsRegistered(prefab))
                SequenceAssetIndexer.instance.UpdateSequenceAsset(prefab);
        }
    }
}
