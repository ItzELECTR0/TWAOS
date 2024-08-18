using System;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class LatestEntry
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private string id;
        [SerializeField] private string path;

        // PROPERTIES: ----------------------------------------------------------------------------

        public string Id => this.id;
        public string Path => this.path;
    }
}