using UnityEngine;
using UnityEditor.Timeline;
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
    [CustomTimelineEditor(typeof(StoryboardPlayableAsset))]
    class StoryboardClipEditor : ClipEditor
    {
        /// <inheritdoc cref="ClipEditor.GetClipOptions"/>
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);

            // set clip colour to green when playhead is on clip
            var director = TimelineEditor.inspectedDirector;
            if (director != null && director.time >= clip.start && director.time <= clip.end)
                options.highlightColor = options.highlightColor * 1.5f;

            return options;
        }

        /// <inheritdoc cref="ClipEditor.DrawBackground"/>
        /// <remarks>Adds thumbnail image to the clip</remarks>>
        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            var asset = clip.asset as StoryboardPlayableAsset;
            if (asset == null) return;

            var board = asset.board;
            if (board != null)
                GUI.DrawTexture(region.position, board, ScaleMode.ScaleToFit);
        }

        /// <inheritdoc cref="ClipEditor.OnCreate"/>
        /// <remarks>Sets clip duration to defaultFrameDuration</remarks>
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var storyTrack = track as StoryboardTrack;
            if (storyTrack != null)
                clip.duration = storyTrack.defaultFrameDuration;
        }
    }
}
