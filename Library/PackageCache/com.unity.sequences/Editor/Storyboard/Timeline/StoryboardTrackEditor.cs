using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    // Todo: This class should be made private: https://jira.unity3d.com/browse/SEQ-542
    // Note: The documentation XML are added only to remove warning when validating the package until this class
    //       can be made private. In the meantime, it is explicitly excluded from the documentation, see
    //       Documentation > filter.yml

    /// <summary>
    ///
    /// </summary>
    [CustomTimelineEditor(typeof(StoryboardTrack))]
    class StoryboardTrackEditor : TrackEditor
    {
        /// <inheritdoc cref="TrackEditor.GetTrackOptions"/>
        /// <remarks>minimumHeight is larger to better see the thumbnail image</remarks>
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var options = base.GetTrackOptions(track, binding);
            options.minimumHeight = 40;

            return options;
        }
    }
}
