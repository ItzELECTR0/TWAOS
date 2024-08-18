using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TRepository<T> : IRepository where T : class, IRepository, new()
    {
        public const string PATH_IN_RESOURCES = "Settings/";
        public const string DIRECTORY_ASSETS = RuntimePaths.DATA + "Resources/" + PATH_IN_RESOURCES;

        protected static T Instance;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string AssetDirectory => DIRECTORY_ASSETS;
        
        public abstract string RepositoryID { get; }

        public static T Get
        {
            get
            {
                if (Instance != null) return Instance;
                
                T repository = new T();
                string path = PathUtils.Combine(PATH_IN_RESOURCES, repository.RepositoryID);

                AssetRepository<T> assetRepository = Resources.Load<AssetRepository<T>>(path);
                if (assetRepository != null) repository = assetRepository.Get();
                
                Instance = repository;
                return Instance;
            }
        }
    }
}