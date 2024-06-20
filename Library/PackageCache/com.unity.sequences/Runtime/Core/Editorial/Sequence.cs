using System;
using System.Collections.Generic;
using System.Data;

namespace UnityEngine.Sequences
{
    /// <summary>
    /// Sequence is a base object to define a time unit in a MasterSequence (in a cinematic).
    /// </summary>
    [Serializable]
    public class Sequence
    {
        /// <summary>
        /// The name of the Sequence.
        /// </summary>
        [SerializeField] internal string m_Name = "NewSequence";
        /// <summary>
        /// The framerate of the Sequence.
        /// </summary>
        [SerializeField] internal float m_Fps = -1.0f;
        /// <summary>
        /// The start time of the Sequence.
        /// </summary>
        [SerializeField] internal double m_Start = 0.0;
        /// <summary>
        /// The duration of the Sequence.
        /// </summary>
        [SerializeField] internal double m_Duration = 2.0;
        [SerializeField] int m_Parent = -1;
        [SerializeReference] List<int> m_Children = null;
        SequenceManager m_Manager = null;

        /// <summary>
        /// Called when the Sequence is renamed, <see cref="Sequence.name"/>.
        /// </summary>
        internal static event Action<Sequence> sequenceRenamed;

        /// <summary>
        /// Default constructor, create a default SequenceManager.
        /// </summary>
        internal Sequence()
        {
            m_Manager = new SequenceManager();
            m_Manager.Add(this);
        }

        /// <summary>
        /// Construct a Sequence using the SequenceManager provided.
        /// </summary>
        /// <param name="manager">The SequenceManager to use to handle this Sequence.</param>
        internal Sequence(SequenceManager manager)
        {
            m_Manager = manager;
            m_Manager.Add(this);
        }

        /// <summary>
        /// Get or set the name of the masterSequence clip.
        /// </summary>
        /// <remarks>When setting a new name, sequenceRenamed event is triggered.</remarks>
        internal virtual string name
        {
            get => m_Name;
            set
            {
                m_Name = value;
                sequenceRenamed?.Invoke(this);
            }
        }

        /// <summary>
        /// Get or set the fps of the Sequence. The fps is retrieved from the parent Sequence when it is not
        /// locally defined.
        /// </summary>
        /// <exception>When no fps is specified, neither locally or inherited from a potential
        /// parent.</exception>
        internal virtual float fps
        {
            get
            {
                if (!isFpsInherited)  // FPS is locally specified.
                    return m_Fps;

                if (parent == null)  // No FPS are specified, either locally or in a parent.
                    throw new DataException("No Framerate (FPS) is specified.");

                return parent.fps;  // FPS is specified by a parent.
            }
            set => m_Fps = value;
        }

        /// <summary>
        /// Indicates whether the framerate inherited its value from the parent Sequence or not.
        /// </summary>
        internal bool isFpsInherited => m_Fps < 0.0;

        /// <summary>
        /// Get or set the Sequence start time (in seconds).
        /// The Sequence start time is computed from its children start time if any.
        /// </summary>
        internal virtual double start
        {
            get
            {
                if (!hasChildren)
                    return m_Start;

                var newStart = Double.MaxValue;
                foreach (var child in children)
                    newStart = Math.Min(newStart, child.start);

                m_Start = newStart;
                return m_Start;
            }
            set => m_Start = value;
        }

        /// <summary>
        /// Get the Sequence end time (in seconds). The end time is computed from the start time and duration of
        /// the Sequence.
        /// </summary>
        internal double end => start + duration;

        /// <summary>
        /// Get or set the Sequence duration (in seconds).
        /// The Sequence duration is computed from it's children duration if any.
        /// </summary>
        internal virtual double duration
        {
            get => GetChildrenDuration();
            set => m_Duration = value;
        }

        // Gets the duration of all the children sequences. This is used in the `duration` getter and during
        // the creation of a new child Sequence.
        double GetChildrenDuration()
        {
            if (!hasChildren) return m_Duration;

            var newEnd = Double.MinValue;
            foreach (var child in children)
                newEnd = Math.Max(newEnd, child.end);

            duration = newEnd - start;
            return m_Duration;
        }

        /// <summary>
        /// Move a clip in time. Other clips (siblings, parents and children) will be impacted accordingly.
        /// </summary>
        /// <param name="newStart">The new start time of the clip.</param>
        /// <returns>The actual start time of the moved clip.</returns>
        /// <exception cref="ArgumentException">When newStart is negative.</exception>
        internal virtual double Move(double newStart)
        {
            if (newStart < 0.0)
                throw new ArgumentException($"Can't move the clip to a negative start time: {newStart}.");

            if (Math.Abs(newStart - start) < 0.00001) return newStart;

            newStart = MoveSiblings(newStart, duration);
            var deltaTime = newStart - start;
            start = newStart;

            foreach (var child in children)
                child.m_Start += deltaTime;

            parent?.RecomputeTime();

            return newStart;
        }

        /// <summary>
        /// Set the duration of the clip. Other clips (siblings and parents) will be impacted accordingly.
        /// </summary>
        /// <param name="newDuration">The new duration of the clip.</param>
        internal virtual void SetDuration(double newDuration)
        {
            if (newDuration < 0.0 || Math.Abs(newDuration - duration) < 0.00001) return;

            MoveSiblings(start, newDuration);
            m_Duration = newDuration;

            parent?.RecomputeTime();
        }

        /// <summary>
        /// Get or set the Sequence parent. When setting a parent, bi-directional links are updated (i.e. parent
        /// point to this and this point to parent).
        /// <seealso cref="Sequence.AddChild"/>
        /// <seealso cref="Sequence.RemoveChild"/>
        /// </summary>
        internal Sequence parent
        {
            get => (m_Parent < 0) ? null : manager.GetAt(m_Parent);
            set
            {
                if (parent == value) return;

                parent?.RemoveChild(this);
                value?.AddChild(this);
            }
        }

        /// <summary>
        /// Get whether the Sequence has children or not.
        /// </summary>
        internal bool hasChildren => m_Children != null && m_Children.Count > 0;

        /// <summary>
        /// Get an enumerable on the Sequence children.
        /// </summary>
        internal IEnumerable<Sequence> children
        {
            get
            {
                if (m_Children == null)
                    yield break;

                foreach (int clipIdx in m_Children)
                    yield return manager.GetAt(clipIdx);
            }
        }

        internal Sequence[] GetChildren()
        {
            var result = new List<Sequence>();
            foreach (var child in children)
            {
                result.Add(child);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get or set the Sequence manager.
        /// </summary>
        internal virtual SequenceManager manager
        {
            get => m_Manager;
            set
            {
                m_Manager = value;
                m_Manager.Add(this);  // TODO (FTV-581): This should set the manager for the whole hierarchy.
            }
        }

        /// <summary>
        /// Add the given child to the Sequence. The added child will have this as its parent.
        /// The start time and duration of the Sequence are re-computed and siblings are moved if needed.
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        /// <param name="childClip">The child Sequence to add.</param>
        internal virtual void AddChild(Sequence childClip)
        {
            if (childClip == null) return;

            var startTime = AddChildClip(childClip);
            childClip.ParentClip(this);

            childClip.start = startTime;
        }

        /// <summary>
        /// Remove the given child from the masterSequence clip. The removed child will be un-parented from this.
        /// The start time and duration of the Sequence are re-computed and siblings are moved if needed.
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        /// <param name="childClip">The child Sequence to remove.</param>
        internal virtual void RemoveChild(Sequence childClip)
        {
            if (manager.GetIndex(childClip) < 0) return;

            RemoveChildClip(childClip);
            childClip.UnParentClip();
        }

        /// <summary>
        /// Set the parent only.
        /// <seealso cref="Sequence.AddChild"/>
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        /// <param name="parentClip">The parent Sequence to set.</param>
        internal virtual void ParentClip(Sequence parentClip)
        {
            m_Parent = manager.Add(parentClip);
        }

        /// <summary>
        /// Unset the parent only.
        /// <seealso cref="Sequence.RemoveChild"/>
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        internal virtual void UnParentClip()
        {
            m_Parent = -1;
        }

        /// <summary>
        /// Add a child after all existing children (if any).
        /// <seealso cref="Sequence.AddChild"/>
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        /// <param name="childClip">The child Sequence to add.</param>
        /// <returns>The actual start time of the added clip.</returns>
        internal virtual double AddChildClip(Sequence childClip)
        {
            var childIdx = manager.Add(childClip);

            if (m_Children == null)
                m_Children = new List<int>();

            // Add the childClip after all existing children clip of this.
            var startTime = hasChildren ? start + GetChildrenDuration() : start;
            m_Children.Add(childIdx);

            return startTime;
        }

        /// <summary>
        /// Remove the child only.
        /// <seealso cref="Sequence.RemoveChild"/>
        /// <seealso cref="Sequence.parent"/>
        /// </summary>
        /// <param name="childClip">The child Sequence to remove.</param>
        internal virtual void RemoveChildClip(Sequence childClip)
        {
            int childIdx = manager.GetIndex(childClip);
            m_Children.Remove(childIdx);
        }

        /// <summary>
        /// Recompute the start time and duration of the clip based on it's children. Siblings are moved if needed.
        /// Those chances are transmit to parent.
        /// <seealso cref="Sequence.AddChild"/>
        /// <seealso cref="Sequence.RemoveChild"/>
        /// <seealso cref="Sequence.Move"/>
        /// <seealso cref="Sequence.SetDuration"/>
        /// </summary>
        void RecomputeTime()
        {
            start = MoveSiblings(start, duration);
            parent?.RecomputeTime();
        }

        /// <summary>
        /// Get the sibling Sequence that is right before this.
        /// </summary>
        /// <returns>The previous Sequence sibling.</returns>
        Sequence GetPreviousSibling()
        {
            if (parent == null) return null;

            var timeFromPrevious = Double.MaxValue;
            Sequence previousSibling = null;
            foreach (var sibling in parent.children)
            {
                if (sibling == this || sibling.start > start) continue;

                if (start - sibling.start < timeFromPrevious)
                {
                    previousSibling = sibling;
                    timeFromPrevious = start - sibling.start;
                }
            }

            return previousSibling;
        }

        /// <summary>
        /// Get the sibling Sequence that is right after this.
        /// </summary>
        /// <returns>The next Sequence sibling.</returns>
        Sequence GetNextSibling()
        {
            if (parent == null) return null;

            var timeFromNext = Double.MaxValue;
            Sequence nextSibling = null;
            foreach (var sibling in parent.children)
            {
                if (sibling == this || sibling.start < start) continue;

                if (sibling.start - start < timeFromNext)
                {
                    nextSibling = sibling;
                    timeFromNext = sibling.start - start;
                }
            }

            return nextSibling;
        }

        /// <summary>
        /// Move siblings according to given new start time and new duration. Siblings are moved (i.e. their start time
        /// may change but duration are preserved).
        /// <seealso cref="Sequence.Move"/>
        /// <seealso cref="Sequence.SetDuration"/>
        /// <seealso cref="Sequence.RecomputeTime"/>
        /// </summary>
        /// <param name="newStart">The new start time of the clip that trigger moving its siblings.</param>
        /// <param name="newDuration">The new duration of the clip that trigger moving its siblings.</param>
        /// <returns>The actual new start time for the clip moved originally.</returns>
        double MoveSiblings(double newStart, double newDuration)
        {
            var previousSibling = GetPreviousSibling();
            if (previousSibling != null && previousSibling.end > newStart)
            {
                newStart = previousSibling.end;
            }

            var nextSibling = GetNextSibling();
            if (nextSibling != null && nextSibling.start < newStart + newDuration)
            {
                nextSibling.Move(newStart + newDuration);
            }

            return newStart;
        }
    }
}
