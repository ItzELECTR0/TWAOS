using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace UnityEditor.Sequences
{
    internal static class TimelineUtility
    {
        internal static TimelinePath breadcrumb = new TimelinePath();

        internal class TimelinePath
        {
            internal struct Element
            {
                public PlayableDirector director;
                public TimelineClip hostClip;
            }
            Stack<Element> path;

            internal int count => path.Count;

            public TimelinePath()
            {
                path = new Stack<Element>();
            }

            public Element Pop()
            {
                return path.Pop();
            }

            public void Clear()
            {
                path.Clear();
            }

            public void BuildAndAppend(TimelineAsset destinationTimeline)
            {
                var sequence = SequenceIndexer.instance.GetSequence(destinationTimeline);

                var current = sequence;
                while (current != null && current.director != null)
                {
                    var director = current.director;
                    TimelineClip hostClip = null;

                    var parent = current.parent;
                    if (parent != null && parent.director != null)
                    {
                        var parentDirector = parent.director;
                        foreach (var clip in parent.timeline.GetEditorialClips())
                        {
                            var clipAsset = clip.asset as EditorialPlayableAsset;
                            if (clipAsset == null)
                                continue;

                            var currentDirector = clipAsset.director.Resolve(parentDirector);
                            if (currentDirector != null && currentDirector == director)
                            {
                                hostClip = clip;
                                break;
                            }
                        }

                        if (hostClip == null)
                            return;
                    }

                    Append(director, hostClip);
                    current = current.parent;
                }
            }

            public void Append(PlayableDirector director, TimelineClip hostClip)
            {
                path.Push(new Element() { director = director, hostClip = hostClip });
            }
        }

        public static void RefreshBreadcrumb(TimelinePath path = null)
        {
            if (path == null)
                path = breadcrumb;

            TimelinePath.Element parent = default;

            TimelineEditorWindow window = TimelineEditor.GetWindow();
            if (window == null)
                return;

            while (path.count > 0)
            {
                TimelinePath.Element drillInClip = path.Pop();
                if (parent.director != null)
                {
                    SequenceContext context = default;

                    // Look for existing context before creating a new one.
                    foreach (var child in window.navigator.GetChildContexts())
                    {
                        if (child.director == drillInClip.director)
                        {
                            context = child;
                            break;
                        }
                    }

                    if (context == default)
                        context = new SequenceContext(drillInClip.director, drillInClip.hostClip);

                    window.navigator.NavigateTo(context);
                }
                else
                {
                    // Add a PlayableDirectorInternalState only on the master timeline.
                    if (!drillInClip.director.GetComponent<PlayableDirectorInternalState>())
                        Undo.AddComponent<PlayableDirectorInternalState>(drillInClip.director.gameObject);
                    window.SetTimeline(drillInClip.director);
                }
                parent = drillInClip;
            }

            if (TimelineEditor.masterDirector
                && TimelineEditor.masterDirector.TryGetComponent<PlayableDirectorInternalState>(out PlayableDirectorInternalState component))
            {
                component.RestoreTimeState();
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
        }

        internal static double GetProjectFrameRate()
        {
            return TimelineProjectSettings.instance.defaultFrameRate;
        }

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdated;
        }

        static void OnPrefabInstanceUpdated(GameObject prefabInstance)
        {
            RefreshBreadcrumb();
        }
    }
}
