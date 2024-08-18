using System;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class AssetChanges
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private string[] @new = Array.Empty<string>();
        [SerializeField] private string[] enhanced = Array.Empty<string>();
        [SerializeField] private string[] changed = Array.Empty<string>();
        [SerializeField] private string[] removed = Array.Empty<string>();
        [SerializeField] private string[] @fixed = Array.Empty<string>();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public string[] New => this.@new;
        public string[] Enhanced => this.enhanced;
        public string[] Changed => this.changed;
        public string[] Removed => this.removed;
        public string[] Fixed => this.@fixed;
    }
}