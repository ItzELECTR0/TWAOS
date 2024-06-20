using System;
using System.Collections.Generic;
using UnityEngine.Sequences;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// This utility class provide functions to help find in all registries that can exist in the Project.
    /// </summary>
    static class MasterSequenceRegistryUtility
    {
        /// <summary>
        /// Gets the Master Scene path for the specified Master Sequence timeline.
        /// </summary>
        internal static string GetMasterScene(TimelineAsset timeline)
        {
            foreach (var registry in GetRegistries())
            {
                if (!registry.Contains(timeline))
                    continue;

                var path = registry.GetScene(timeline);
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return string.Empty;
        }

        /// <summary>
        /// Assigns the provided <paramref name="scenePath"/> to the first <paramref name="timeline"/> found in a registry.
        /// Will look into the default registry first then all the others.
        /// </summary>
        internal static void SetMasterScene(TimelineAsset timeline, string scenePath)
        {
            if (timeline == null)
                throw new ArgumentNullException("timeline");

            foreach (var registry in GetRegistries())
            {
                if (!registry.Contains(timeline))
                    continue;

                registry.SetScene(timeline, scenePath);
                Save(registry);
                return;
            }

            throw new MasterSequenceNotFoundException();
        }

        /// <summary>
        /// Gets all the master sequences that are registered in the project.
        /// </summary>
        internal static IEnumerable<MasterSequenceRegistry.MasterSequence> GetMasterSequences()
        {
            foreach (var registry in GetRegistries())
            {
                bool needPruning = false;
                foreach (var masterSequence in registry.masterSequences)
                {
                    if (masterSequence.timeline != null)
                        yield return masterSequence;
                    else
                        needPruning = true;
                }

                if (needPruning)
                {
                    registry.PruneNullMasterTimelines();
                    Save(registry);
                }
            }
        }

        /// <summary>
        /// Is the specified timeline a master timeline? I.e. the timeline of a Master Sequence.
        /// </summary>
        internal static bool IsMasterTimeline(TimelineAsset timeline)
        {
            foreach (var registry in GetRegistries())
            {
                if (registry.Contains(timeline))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all entries where the master timeline is null in all registries.
        /// </summary>
        internal static void PruneRegistries()
        {
            foreach (var registry in GetRegistries())
            {
                if (registry.PruneNullMasterTimelines())
                    Save(registry);
            }
        }

        /// <summary>
        /// Gets the MasterSequenceRegistry asset in which the specified Master Sequence timeline is registered.
        /// </summary>
        internal static MasterSequenceRegistry GetRegistry(TimelineAsset timeline)
        {
            foreach (var registry in GetRegistries())
            {
                if (registry.Contains(timeline))
                    return registry;
            }

            throw new MasterSequenceNotFoundException();
        }

        /// <summary>
        /// Returns all existing registries found in the project.
        /// The default registry is always returned first.
        /// </summary>
        static IEnumerable<MasterSequenceRegistry> GetRegistries()
        {
            var defaultExists = ProjectSettings.defaultMasterSequenceRegistry != null;
            string defaultRegistryGuid = string.Empty;

            if (defaultExists)
            {
                defaultRegistryGuid = AssetDatabase.AssetPathToGUID(
                    AssetDatabase.GetAssetPath(ProjectSettings.defaultMasterSequenceRegistry));

                // Always return the default registry first.
                yield return ProjectSettings.defaultMasterSequenceRegistry;
            }

            var guids = AssetDatabase.FindAssets("t:MasterSequenceRegistry");
            foreach (var guid in guids)
            {
                // Skip the default registry when found in the database.
                if (defaultExists && guid == defaultRegistryGuid)
                    continue;

                var path = AssetDatabase.GUIDToAssetPath(guid);
                yield return AssetDatabase.LoadAssetAtPath<MasterSequenceRegistry>(path);
            }
        }

        /// <summary>
        /// Serializes the provided <paramref name="registry"/> onto disk.
        /// </summary>
        static void Save(MasterSequenceRegistry registry)
        {
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssetIfDirty(registry);
        }
    }
}
