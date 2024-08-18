using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TAssetRepository : ScriptableObject
    {
        public abstract IIcon Icon { get; }
        public abstract string Name { get; }

        public abstract string RepositoryID { get; }
        public abstract string AssetPath { get; }
        public abstract int Priority { get; }
        
        public virtual bool IsFullScreen => false;
    }
}