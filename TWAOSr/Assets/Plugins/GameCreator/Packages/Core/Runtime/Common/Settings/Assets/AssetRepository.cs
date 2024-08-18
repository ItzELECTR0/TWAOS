using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class AssetRepository<T> : TAssetRepository where T : class, IRepository, new()
    {
        [SerializeReference] private IRepository m_Repository = new T();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string AssetPath => this.m_Repository.AssetDirectory;

        public override string RepositoryID => this.m_Repository.RepositoryID;

        public override int Priority => 10;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Get()
        {
            return this.m_Repository as T;
        }
    }
}