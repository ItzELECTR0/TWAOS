using System;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class AssetRelease
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] public bool available;
        [SerializeField] private AssetDate date = new AssetDate();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Available => available;
        public AssetDate Date => this.date;
    }
}