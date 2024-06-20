using System;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences
{
    /// <summary>
    /// A TimelineSequence is a <see cref="Sequence"/> object associated with a TimelineAsset. This is the main object used
    /// in the <see cref="MasterSequence"/> asset to define an editorial hierarchy of sequences.
    /// </summary>
    [Serializable]
    public partial class TimelineSequence : Sequence
    {
        [SerializeField] TimelineAsset m_Timeline;

        [SerializeReference] EditorialTrack m_ChildrenTrack;
        [SerializeField] string m_DefaultChildrenTrackName = "Children";

        [SerializeReference] EditorialPlayableAsset m_EditorialClipAsset;

        TimelineSequence() : base()
        {
        }

        TimelineSequence(SequenceManager manager) : base(manager)
        {
        }

        internal static TimelineSequence CreateInstance(SequenceManager manager = null)
        {
            var clip = manager != null ? new TimelineSequence(manager) : new TimelineSequence();
            clip.InitializeTimeline();
            return clip;
        }

        void InitializeTimeline()
        {
            m_Timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            m_Timeline.name = m_Name;
        }

        /// <summary>
        /// Gets the Sequence timeline.
        /// </summary>
        public TimelineAsset timeline => m_Timeline;

        /// <summary>
        /// Get the Sequence EditorialTrack that contains children clips. It can be null.
        /// </summary>
        internal EditorialTrack childrenTrack => m_ChildrenTrack;

        /// <summary>
        /// Get or set the Sequence children track name.
        /// e.g.: Common name for children tracks are "Sequences" or "Shots".
        /// </summary>
        internal string childrenTrackName
        {
            get => m_ChildrenTrack == null ? m_DefaultChildrenTrackName : m_ChildrenTrack.name;
            set
            {
                m_DefaultChildrenTrackName = value;

                if (m_ChildrenTrack != null)
                    m_ChildrenTrack.name = value;
            }
        }

        /// <summary>
        /// Gets the Sequence editorial TimelineClip. This is the clip that belongs to the parent
        /// <see cref="TimelineSequence.childrenTrack"/> and point to the timeline of this Sequence.
        /// </summary>
        internal TimelineClip editorialClip
        {
            get
            {
                if (m_EditorialClipAsset == null || parent == null) return null;

                var timelineParent = parent as TimelineSequence;
                foreach (var editorialClip in timelineParent.childrenTrack.GetClips())
                {
                    if ((editorialClip.asset as EditorialPlayableAsset) == m_EditorialClipAsset)
                        return editorialClip;
                }

                return null;
            }
        }

        /// <inheritdoc cref="Sequence.name"/>
        /// <summary>
        /// Indicates the name of the Sequence. Changing the name of a TimelineSequence also changes the name of the
        /// timeline and the editorialClip (if it exists).
        /// </summary>
        internal override string name
        {
            set
            {
                timeline.name = value;

                if (editorialClip != null)
                    editorialClip.displayName = value;

                base.name = value;
            }
        }

        /// <summary>
        /// Indicates the framerate of the Sequence. The framerate is retrieved from the Timeline asset when possible.
        /// </summary>
        internal override float fps
        {
            get
            {
                if (m_Timeline != null)
                    return (float)m_Timeline.GetFrameRate();

                return base.fps;
            }
            set
            {
                base.fps = value;

                if (m_Timeline != null)
                    SetTimelineFpsRecursive(value);
            }
        }

        void SetTimelineFpsRecursive(float value)
        {
            if (timeline == null)
                return;

            timeline.SetFrameRate(value);

            foreach (var child in children)
            {
                var timelineChild = child as TimelineSequence;
                if (timelineChild != null && timelineChild.isFpsInherited)
                    timelineChild.SetTimelineFpsRecursive(value);
            }
        }

        /// <summary>
        /// Get or set the TimelineSequence start time. The start time is based on its editorialClip.start value
        /// when editorialClip exists.
        /// </summary>
        internal override double start
        {
            get
            {
                if (editorialClip == null)
                    return base.start;

                var timelineParent = (parent as TimelineSequence);
                if (timelineParent == null)
                {
                    // This case shouldn't happen in a valid state: a TimelineSequence can't have an
                    // EditorialClip without a parent.
                    m_Start = editorialClip.start;
                }
                else if (timelineParent.editorialClip == null)
                    m_Start = editorialClip.start;
                else
                    m_Start = editorialClip.start + timelineParent.start;

                return m_Start;
            }
            set
            {
                base.start = value;

                if (editorialClip != null)
                    editorialClip.start = value - (parent?.start ?? 0);
            }
        }

        /// <summary>
        /// Get or set the TimelineSequence duration. The duration is based on its editorialClip.duration value
        /// when editorialClip exists.
        /// </summary>
        internal override double duration
        {
            get
            {
                if (editorialClip != null)
                    return editorialClip.duration;

                return base.duration;
            }
            set => base.duration = value;
        }

        /// <summary>
        /// Gets the recording start time.
        /// This time is computed based on the TimelineSequence editorial clip start and its parent (if any) editorial
        /// clip start and clip-in values.
        /// </summary>
        /// <returns>The recording start time in second (double).</returns>
        internal double GetRecordStart()
        {
            if (parent == null)
                return start;

            var timelineParent = parent as TimelineSequence;
            var parentClipIn = timelineParent.editorialClip?.clipIn ?? 0;
            var recordStart = start - parentClipIn;

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
            if (parent == null)
                return end;

            var timelineParent = parent as TimelineSequence;
            var parentClipIn = timelineParent.editorialClip?.clipIn ?? 0;
            var recordStart = start - parentClipIn;
            var recordEnd = recordStart + duration;

            return Math.Min(timelineParent.GetRecordEnd(), recordEnd);
        }

        /// <summary>
        /// Tests if the given masterSequence clip is null or if its timeline is. If yes, this timeline masterSequence clip is
        /// unusable as is. Either it is corrupted or its initialization is not finished yet.
        /// </summary>
        /// <param name="sequence">The TimelineSequence to check if it is null or empty.</param>
        /// <returns>Returns true if the given TimelineSequence is null or empty. Returns false otherwise.</returns>
        internal static bool IsNullOrEmpty(TimelineSequence sequence)
        {
            return sequence == null || sequence.m_Timeline == null;
        }

        /// <inheritdoc cref="Sequence.UnParentClip"/>
        /// <summary>
        /// Also nullify the editorialClip.
        /// </summary>
        internal override void UnParentClip()
        {
            base.UnParentClip();
            m_EditorialClipAsset = null;
        }

        /// <summary>
        /// Adds the specified Sequence as a child of this Sequence and creates the childrenTrack if it doesn't already exist.
        /// The editorialClip of the added clip is also created in the childrenTrack.
        /// If the given clip is not a TimelineSequence, no track or clip are created.
        ///
        /// This function is used by <seealso cref="Sequence.AddChild"/>
        /// and the <seealso cref="Sequence.parent"/> setter.
        /// </summary>
        /// <param name="childClip">The child Sequence to add.</param>
        /// <returns>The actual global start time of the added clip.</returns>
        internal override double AddChildClip(Sequence childClip)
        {
            var timelineChildClip = childClip as TimelineSequence;
            if (timelineChildClip == null)
                return base.AddChildClip(childClip); // If childClip is not a TimelineSequence, there is nothing to do Timeline wise.

            m_ChildrenTrack = timeline.GetOrCreateTrack<EditorialTrack>(childrenTrackName);
            var startTime = timelineChildClip.CreateEditorialClip(m_ChildrenTrack);

            if (timelineChildClip.isFpsInherited)
                timelineChildClip.SetTimelineFpsRecursive(fps);

            // Ignore its return value. Return the one found found via the Timeline API in the
            // CreateEditorialClip function above.
            base.AddChildClip(childClip);

            // Return global start time.
            return start + startTime;
        }

        /// <summary>
        /// On top of removing the given Sequence from children, its editorialClip is removed from the
        /// childrenTrack. If the childrenTrack end up empty, it is removed as well.
        /// If the given clip is not a TimelineSequence, no track or clip are deleted.
        /// <seealso cref="Sequence.RemoveChild"/>
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        /// <param name="childClip">The child Sequence to remove.</param>
        internal override void RemoveChildClip(Sequence childClip)
        {
            base.RemoveChildClip(childClip);

            if (m_Timeline == null)
                return;

            var timelineChildClip = childClip as TimelineSequence;
            if (timelineChildClip == null)
                return;

            timeline.DeleteClip(timelineChildClip.editorialClip);

            if (childrenTrack != null && !childrenTrack.hasClips)
            {
                timeline.DeleteTrack(childrenTrack);
                m_ChildrenTrack = null;
            }
        }

        /// <summary>
        /// Create the editorial TimelineClip in the given NestedTimelineTrack track.
        /// The created clip will be named after the Sequence name and its start and duration values are based
        /// on the Sequence start and duration values.
        /// </summary>
        /// <param name="parentTrack">The NestedTimelineTrack track in which the clip is created.</param>
        double CreateEditorialClip(EditorialTrack parentTrack)
        {
            var startTime = parentTrack.end;

            var newClip = parentTrack.CreateClip<EditorialPlayableAsset>();
            newClip.displayName = name;
            newClip.start = startTime;
            newClip.duration = duration;

            m_EditorialClipAsset = newClip.asset as EditorialPlayableAsset;
            m_EditorialClipAsset.timeline = timeline;

            return startTime;
        }

        /// <summary>
        /// Get the parent clip start time to help compute the global time from the editorialClip start time
        /// (which is local).
        /// </summary>
        /// <returns>A double that represents the parent start time.</returns>
        double GetParentStart()
        {
            var timelineParent = parent as TimelineSequence;
            if (timelineParent?.editorialClip == null)
                return 0.0;

            return timelineParent.m_Start;
        }
    }
}
