using UnityEngine;
using UnityEditor.Timeline;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    [CustomTimelineEditor(typeof(SceneActivationPlayableAsset))]
    class SceneActivationClipEditor : ClipEditor
    {
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            var director = TimelineEditor.inspectedDirector;

            if (director != null && director.time >= clip.start && director.time <= clip.end)
                options.highlightColor *= 1.5f;

            return options;
        }

        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);

            clip.displayName = "Active";
        }
    }
}
