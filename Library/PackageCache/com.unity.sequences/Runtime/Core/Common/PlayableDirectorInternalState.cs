using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Sequences
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    internal class PlayableDirectorInternalState : MonoBehaviour
    {
        const int k_CacheSize = 5;

        public List<double> m_CachedValues = new List<double>();
        PlayableDirector m_Director;

        void Awake()
        {
            hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInBuild;
        }

        void Start()
        {
            m_Director = GetComponent<PlayableDirector>();
        }

        void Update()
        {
            if (m_Director == null)
                return;

            m_CachedValues.Add(m_Director.time);
            if (m_CachedValues.Count > k_CacheSize)
                m_CachedValues.RemoveAt(0);
        }

        double GetTime()
        {
            for (int i = m_CachedValues.Count - 1; i >= 0; --i)
            {
                if (m_CachedValues[i] > 0.0)
                    return m_CachedValues[i];
            }
            return 0;
        }

        public void RestoreTimeState()
        {
            if (m_Director == null)
                return;

            m_Director.time = GetTime();
        }
    }
}
