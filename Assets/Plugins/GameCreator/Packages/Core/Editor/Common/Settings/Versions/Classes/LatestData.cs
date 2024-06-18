using System;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class LatestData
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private LatestEntry[] list = Array.Empty<LatestEntry>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public LatestEntry[] List => this.list;
        
        [field: NonSerialized] public State State { get; set; } = State.Loading;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public LatestData()
        { }

        public LatestData(State state) : this()
        {
            this.State = state;
        }
    }
}