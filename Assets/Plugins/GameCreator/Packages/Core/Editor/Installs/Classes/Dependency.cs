using System;
using UnityEngine;

namespace GameCreator.Editor.Installs
{
    [Serializable]
    public struct Dependency
    {
        [SerializeField] private string m_ID;
        [SerializeField] private Version m_MinVersion;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string ID => this.m_ID;
        public Version MinVersion => this.m_MinVersion;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Dependency(string id)
        {
            this.m_ID = id;
            this.m_MinVersion = Version.Zero;
        }
        
        public Dependency(string id, Version minVersion) : this(id)
        {
            this.m_MinVersion = minVersion;
        }
    }
}