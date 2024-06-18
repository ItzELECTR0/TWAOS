using System;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class AssetEntry
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private AssetVersion version = new AssetVersion();
        [SerializeField] private AssetRelease release = new AssetRelease();
        [SerializeField] private AssetChanges changes = new AssetChanges();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public AssetVersion Version => this.version;
        public AssetRelease Release => this.release;
        public AssetChanges Changes => this.changes;

        [field: NonSerialized] public State State { get; set; } = State.Loading;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public AssetEntry()
        { }

        public AssetEntry(State state) : this()
        {
            this.State = state;
        }
    }
}