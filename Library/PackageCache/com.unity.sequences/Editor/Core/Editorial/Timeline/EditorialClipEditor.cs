using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    /// <summary>
    /// The EditorialPlayableAsset clip editor. This is where the duration of the clip is adjusted to match it's
    /// content duration when needed, <see cref="EditorialClipEditor.OnClipChanged"/>.
    /// </summary>
    [CustomTimelineEditor(typeof(EditorialPlayableAsset))]
    class EditorialClipEditor : NestedTimelineClipEditor
    {
        /// <inheritdoc/>
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            var director = TimelineEditor.inspectedDirector;

            if (director == null)
                return options;

            var clipAsset = clip.asset as EditorialPlayableAsset;
            var clipDirector = clipAsset.director.Resolve(director);

            if (clipDirector == null)
                options.errorText = "Missing bindings";

            else if (clipDirector.playableAsset == null)
                options.errorText = $"Missing TimelineAsset on PlayableDirector \"{clipDirector.gameObject.name}\"";

            // Highlight clip if play head is over it.
            if (director.time >= clip.start && director.time <= clip.end)
                options.highlightColor = new Color(0.501f, 1f, 0.901f);

            return options;
        }
    }
}
