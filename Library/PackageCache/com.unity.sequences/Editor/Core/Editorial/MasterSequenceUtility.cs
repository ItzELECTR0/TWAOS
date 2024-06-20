using System;
using System.Collections.Generic;
using UnityEngine.Sequences;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// This utility class provide functions to help find in all registries that can exist in the Project.
    /// </summary>
    static class MasterSequenceUtility
    {
        /// <summary>
        /// Event sent when a Master Sequence is manually removed from a registry.
        /// This is used to refresh the UI accordingly.
        /// </summary>
        internal static event Action masterSequencesRemoved;

        // Temp function: it won't be needed anymore with the new API.
        internal static void GetLegacyData(TimelineAsset timeline, out MasterSequence masterSequence, out TimelineSequence sequence)
        {
            foreach (var ms in GetLegacyMasterSequences())
            {
                if (ms.rootSequence.timeline == timeline)
                {
                    masterSequence = ms;
                    sequence = ms.rootSequence;
                    return;
                }

                foreach (var seq in ms.manager.sequences)
                {
                    var timelineSeq = seq as TimelineSequence;
                    if (timelineSeq.timeline == timeline)
                    {
                        masterSequence = ms;
                        sequence = timelineSeq;
                        return;
                    }
                }
            }

            masterSequence = null;
            sequence = null;
        }

        // Temp function: it won't be needed anymore with the new API.
        internal static IEnumerable<MasterSequence> GetLegacyMasterSequences()
        {
            var GUIDs = AssetDatabase.FindAssets("t:MasterSequence");
            foreach (var GUID in GUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUID);
                var masterSequence = AssetDatabase.LoadAssetAtPath<MasterSequence>(path);
                yield return masterSequence;
            }
        }

        internal static void LegacyMasterSequenceRemoved()
        {
            masterSequencesRemoved?.Invoke();
        }

        internal static bool IsLegacyMasterTimeline(TimelineAsset timeline)
        {
            GetLegacyData(timeline, out var masterSequence, out var _);
            if (masterSequence == null)
                return false;

            return masterSequence.rootSequence.timeline == timeline;
        }
    }
}
