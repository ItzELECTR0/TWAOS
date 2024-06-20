using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEditor.Sequences
{
    [Serializable]
    internal partial class SequenceNode
    {
        /// <summary>
        /// Reference to the TimelineAsset. Only one Sequence can reference a given TimelineAsset.
        /// </summary>
        [SerializeField]
        TimelineAsset m_Timeline;

        /// <summary>
        /// Reference to the TimelineAsset referencing this Sequence
        /// through an EditorialTrack.
        /// </summary>
        [SerializeReference]
        SequenceNode m_Parent;

        /// <summary>
        /// References to all children that are found in EditorialTracks.
        /// </summary>
        [SerializeReference]
        List<SequenceNode> m_Children = new List<SequenceNode>();

        [SerializeReference]
        EditorialPlayableAsset m_EditorialClipAsset;

        TimelineClip m_EditorialClip;

        GameObject m_GameObject;

        bool m_ForceCacheGameObject = false;

        internal SequenceNode() : base()
        {
            PrefabStage.prefabStageOpened += _ => m_ForceCacheGameObject = true;
            PrefabStage.prefabStageClosing += _ => m_ForceCacheGameObject = !m_ForceCacheGameObject;
        }

        internal TimelineAsset timeline
        {
            get => m_Timeline;
            set => m_Timeline = value;
        }

        internal SequenceNode parent
        {
            get => m_Parent;
            set => m_Parent = value;
        }

        internal IReadOnlyList<SequenceNode> children => m_Children;

        /// <summary>
        /// Gets the master sequence, i.e. the root sequence of the editorial structure to which this sequence
        /// belongs to. If this sequence has no parent sequence, then it is the master sequence and it is returned.
        /// </summary>
        internal SequenceNode masterSequence
        {
            get
            {
                var rootSequence = this;
                while (rootSequence.parent != null)
                    rootSequence = rootSequence.parent;

                return rootSequence;
            }
        }

        /// <summary>
        /// Reference to the clip pointing to this Sequence.
        /// </summary>
        internal TimelineClip editorialClip
        {
            get
            {
                // No editorial clip exists.
                if (m_EditorialClipAsset == null || m_Parent == null)
                    return null;

                // An editorial clip exists and is already cached.
                if (m_EditorialClip != null && m_EditorialClip.asset == m_EditorialClipAsset)
                    return m_EditorialClip;

                // An editorial clip exists and we need to try to cache it before returning it.
                // Note: for now, this never happen via our code. It would be interesting to find an actual use case
                // to end here and test it. Otherwise, this might be dead code that could be removed.
                TryCacheEditorialClip();
                return m_EditorialClip;
            }
            set
            {
                m_EditorialClip = value;
                m_EditorialClipAsset = m_EditorialClip.asset as EditorialPlayableAsset;
            }
        }

        /// <summary>
        /// Reference to the GameObject with a PlayableDirector playing this Sequence.
        /// </summary>
        internal GameObject gameObject
        {
            get
            {
                var playableDirector = m_GameObject != null ? m_GameObject.GetComponent<PlayableDirector>() : null;
                if (m_ForceCacheGameObject || m_GameObject == null ||
                    playableDirector == null || playableDirector.playableAsset != m_Timeline)
                {
                    // Try to cache the GameObject if it is not already or if the cached GameObject
                    // doesn't point to m_Timeline anymore.
                    TryCacheGameObject();
                    m_ForceCacheGameObject = false;
                }

                return m_GameObject;
            }
            set => m_GameObject = value;
        }

        /// <summary>
        /// Reference to the PlayableDirector playing this Sequence.
        /// </summary>
        internal PlayableDirector director => gameObject != null ? gameObject.GetComponent<PlayableDirector>() : null;

        /// <summary>
        /// Is the sequence valid? A sequence is invalid if its parent is invalid or if its GameObject is missing.
        /// This is used to help restrict operations (create sub-sequence, rename, record, load scenes) that wouldn't
        /// behave as expected if done on an invalid sequence.
        /// </summary>
        internal bool isValid { get; private set; } = true;

        /// <summary>
        /// Is the sequence has a corresponding GameObject in the Hierarchy window?
        /// This is used to help compute the validity of a sequence and restrict operations (create sub-sequence,
        /// delete, rename, record...) that wouldn't behave as expected if done on a sequence with no associated
        /// GameObject.
        /// E.g.: creating a sub-sequence to such sequence would not create a corresponding GameObject and
        /// Timeline bindings.
        /// </summary>
        internal bool hasGameObject { get; private set; } = true;

        /// <summary>
        /// Is the sequence's GameObject (if any) a Prefab root?
        /// This is used to help restrict operations (delete, rename) that wouldn't behave as expected if done on a
        /// Prefab root GameObject.
        /// E.g.: renaming such sequence using Sequences API would only rename the Prefab
        /// instance root and not the Prefab asset on disk.
        /// </summary>
        internal bool isPrefabRoot
        {
            get
            {
                if (gameObject == null)
                    return false;

                var result = PrefabUtility.IsAnyPrefabInstanceRoot(gameObject);

                if (!result)
                {
                    // Ensure the sequence's GameObject is not a Prefab stage root if one is open.
                    var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
                    result = currentStage != null && currentStage.prefabContentsRoot == gameObject;
                }

                return result;
            }
        }

        internal void AddChild(SequenceNode child)
        {
            if (!m_Children.Contains(child))
                m_Children.Add(child);
        }

        internal void RemoveChild(SequenceNode child)
        {
            m_Children.Remove(child);
        }

        internal void RemoveAllChildren()
        {
            m_Children.Clear();
        }

        internal IEnumerable<KeyValuePair<TimelineClip, GameObject>> GetEmptyClips()
        {
            if (director == null)
                yield break;

            foreach (var clip in timeline.GetEditorialClips())
            {
                var asset = clip.asset as EditorialPlayableAsset;
                if (asset == null)
                    continue;

                var clipDirector = asset.director.Resolve(director);
                if (clipDirector == null && asset.timeline == null)
                    yield return new KeyValuePair<TimelineClip, GameObject>(clip, null);

                else if (clipDirector != null && clipDirector.playableAsset as TimelineAsset == null)
                    yield return new KeyValuePair<TimelineClip, GameObject>(clip, clipDirector.gameObject);
            }
        }

        /// <summary>
        /// Tells if the provided <paramref name="sequence"/> is already linked to this Sequence as a child.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        internal bool IsParentOf(SequenceNode sequence)
        {
            return m_Children.Contains(sequence);
        }

        /// <summary>
        /// Gets the recording start time.
        /// This time is computed based on the TimelineSequence editorial clip start and its parent (if any) editorial
        /// clip start and clip-in values.
        /// </summary>
        /// <returns>The recording start time in second (double).</returns>
        internal double GetRecordStart()
        {
            if (m_Parent == null)
                return editorialClip.start;

            var timelineParent = m_Parent;
            var parentClipIn = timelineParent.editorialClip?.clipIn ?? 0;
            var recordStart = editorialClip.start - parentClipIn;

            return Math.Max(0, recordStart);
        }

        /// <summary>
        /// Gets the recording end time.
        /// This time is computed based on the TimelineSequence editorial clip end and its parent (if any) editorial
        /// clip end and clip-in values.
        /// </summary>
        /// <returns>The recording end time in second (double).</returns>
        internal double GetRecordEnd()
        {
            if (m_Parent == null)
                return editorialClip.end;

            var timelineParent = m_Parent;
            var parentClipIn = timelineParent.editorialClip?.clipIn ?? 0;
            var recordStart = editorialClip.start - parentClipIn;
            var recordEnd = recordStart + editorialClip.duration;

            return Math.Min(timelineParent.GetRecordEnd(), recordEnd);
        }

        internal string GetDisplayName()
        {
            if (timeline != null)
                return timeline.name;

            if (editorialClip != null)
                return editorialClip.displayName;

            if (gameObject != null)
                return gameObject.name;

            return string.Empty;
        }

        internal bool ComputeValidity()
        {
            var newLoadingStatus = gameObject != null;
            var newValidity = parent == null || parent.isValid && (newLoadingStatus || !parent.hasGameObject);

            if (newValidity && parent != null && parent.director != null && m_EditorialClipAsset != null)
            {
                // Check that the binding to this sequence's PlayableDirector is valid.
                var parentDirector = parent.director;
                var resolvedDirector = m_EditorialClipAsset.director.Resolve(parentDirector);
                if (resolvedDirector == null)
                    newValidity = false;
            }

            bool validityChanged = newValidity != isValid || newLoadingStatus != hasGameObject;

            isValid = newValidity;
            hasGameObject = newLoadingStatus;

            return validityChanged;
        }

        void TryCacheEditorialClip()
        {
            foreach (var track in m_Parent.m_Timeline.GetEditorialTracks())
                foreach (var clip in track.GetClips())
                {
                    // If multiple clip references the same asset, returns the first it finds.
                    if (clip.asset == m_EditorialClipAsset)
                    {
                        editorialClip = clip;
                        return;
                    }
                }

            editorialClip = null;
        }

        void TryCacheGameObject()
        {
            foreach (var director in ObjectsCache.FindObjectsFromScenes<PlayableDirector>())
            {
                if (director.playableAsset == timeline)
                {
                    m_GameObject = director.gameObject;
                    return;
                }
            }

            m_GameObject = null;
        }
    }
}
