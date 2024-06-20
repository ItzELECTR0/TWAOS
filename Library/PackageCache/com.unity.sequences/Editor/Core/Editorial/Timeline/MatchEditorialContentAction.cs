using System.Linq;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Sequences.Timeline;

namespace UnityEditor.Sequences.Timeline
{
    [MenuEntry("Match Editorial Content (Ripple)", MenuPriority.CustomTimelineActionSection.start)]
    [ApplyDefaultUndo]
    class MatchEditorialContent : TimelineAction
    {
        public override ActionValidity Validate(ActionContext actionContext)
        {
            var clips = actionContext.clips;
            var tracks = actionContext.tracks;

            if (!clips.Any() && !tracks.Any())
                return ActionValidity.NotApplicable;

            var hasEditorialTrackOrClip = false;
            var hasDifferentTrackOrClip = false;
            foreach (var track in tracks)
            {
                if (track is EditorialTrack)
                    hasEditorialTrackOrClip = true;
                else
                    hasDifferentTrackOrClip = true;

                if (hasEditorialTrackOrClip && hasDifferentTrackOrClip)
                    return ActionValidity.Invalid;
            }

            foreach (var clip in clips)
            {
                if (clip.asset != null && clip.asset is EditorialPlayableAsset)
                    hasEditorialTrackOrClip = true;
                else
                    hasDifferentTrackOrClip = true;

                if (hasEditorialTrackOrClip && hasDifferentTrackOrClip)
                    return ActionValidity.Invalid;
            }

            return hasEditorialTrackOrClip ? ActionValidity.Valid : ActionValidity.NotApplicable;
        }

        public override bool Execute(ActionContext actionContext)
        {
            var tracks = actionContext.tracks;
            foreach (var track in tracks)
            {
                var editorialTrack = track as EditorialTrack;
                if (editorialTrack == null)
                    continue;

                editorialTrack.UpdateClipDurationToMatchEditorialContent(TimelineEditor.inspectedDirector);
            }

            var clips = actionContext.clips;
            foreach (var clip in clips)
            {
                var editorialTrack = clip.GetParentTrack() as EditorialTrack;
                if (editorialTrack == null || tracks.Contains(editorialTrack))
                    continue;

                editorialTrack.UpdateClipDurationToMatchEditorialContent(TimelineEditor.inspectedDirector, clip);
            }

            EditorUtility.SetDirty(actionContext.timeline);

            return true;
        }

        [TimelineShortcut("Match Editorial Content", KeyCode.E, ShortcutModifiers.Shift)]
        public static void HandleShortCut(ShortcutArguments args)
        {
            Invoker.InvokeWithSelected<MatchEditorialContent>();
        }
    }
}
