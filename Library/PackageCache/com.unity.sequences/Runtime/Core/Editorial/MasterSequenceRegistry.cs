using System;
using System.Collections.Generic;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences
{
    class MasterSequenceNotFoundException : Exception
    {
    }

    [CreateAssetMenu(fileName = "MasterSequenceRegistry", menuName = "Sequences/Master Sequence Registry", order = 460)]
    class MasterSequenceRegistry : ScriptableObject
    {
        [Serializable]
        internal class MasterSequence
        {
            public TimelineAsset timeline;
            public SceneReference masterScene;
        }

        [SerializeField]
        [Tooltip("List of the Master Sequences registered in the project, with references to their timeline and Master Scene.")]
        List<MasterSequence> m_MasterSequences = new List<MasterSequence>();

        internal IReadOnlyList<MasterSequence> masterSequences => m_MasterSequences;

        /// <summary>
        /// Adds a new entry in the Registry for the provided <paramref name="masterTimeline"/>.
        /// This do a pre-check to ensure that each <paramref name="masterTimeline"/> has a unique entry.
        /// </summary>
        /// <param name="masterTimeline"></param>
        /// <param name="scenePath"></param>
        internal void Register(TimelineAsset masterTimeline, string scenePath)
        {
            if (masterTimeline == null)
                throw new ArgumentNullException("masterTimeline");

            if (Contains(masterTimeline))
                return;

            m_MasterSequences.Add(new MasterSequence() { timeline = masterTimeline, masterScene = new SceneReference() { path = scenePath } });
        }

        /// <summary>
        /// Sets a new Master Scene to the specified master timeline.
        /// </summary>
        /// <param name="masterTimeline">The master timeline to update with the specified Master Scene path.</param>
        /// <param name="scene">The path of the Master Scene to associate to the specified timeline.</param>
        /// <exception cref="ArgumentNullException">If the specified timeline is null.</exception>
        /// <exception cref="MasterSequenceNotFoundException">If the specified timeline can't be found in the registry.</exception>
        internal void SetScene(TimelineAsset masterTimeline, string scene)
        {
            if (masterTimeline == null)
                throw new ArgumentNullException("masterTimeline");

            var entry = GetEntry(masterTimeline);
            entry.masterScene.path = scene;
        }

        /// <summary>
        /// Gets the file path of the Master Scene associated to the specified <paramref name="masterTimeline"/>.
        /// </summary>
        /// <param name="masterTimeline">The master timeline from which to get the Master Scene.</param>
        /// <returns>The Master Scene's path found if any, otherwise an empty string.</returns>
        /// <exception cref="ArgumentNullException">If the specified timeline is null.</exception>
        /// <exception cref="MasterSequenceNotFoundException">If the specified timeline can't be found in the registry.</exception>
        internal string GetScene(TimelineAsset masterTimeline)
        {
            if (masterTimeline == null)
                throw new ArgumentNullException("masterTimeline");

            var entry = GetEntry(masterTimeline);
            return entry.masterScene.path;
        }

        /// <summary>
        /// Removes all entries where the TimelineAsset reference is null.
        /// This happen when a TimelineAsset is deleted from the project.
        ///
        /// The user registry stay untouched, this registry is managed manually by the user and so any destructive
        /// operation is at the discretion of the user and the user only.
        /// </summary>
        internal bool PruneNullMasterTimelines()
        {
            var removed = m_MasterSequences.RemoveAll(entry => entry.timeline == null);
            if (removed > 0)
                return true;

            return false; // Nothing was removed from the base registry.
        }

        /// <summary>
        /// Tells if the instance of <see cref="MasterSequenceRegistry"/> contains an entry
        /// for the provided <see cref="TimelineAsset>"/>.
        /// </summary>
        /// <param name="masterTimeline"></param>
        /// <returns></returns>
        internal bool Contains(TimelineAsset masterTimeline)
        {
            if (masterTimeline == null)
                return false;

            return m_MasterSequences.Find(ms => ms.timeline == masterTimeline) != null;
        }

        MasterSequence GetEntry(TimelineAsset timeline)
        {
            Predicate<MasterSequence> predicate = (masterSequence) => masterSequence.timeline == timeline;
            var entry = m_MasterSequences.Find(predicate);

            if (entry == null)
                throw new MasterSequenceNotFoundException();

            return entry;
        }
    }
}
