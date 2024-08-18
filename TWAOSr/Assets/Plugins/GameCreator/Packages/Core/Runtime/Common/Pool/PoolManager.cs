using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
	[AddComponentMenu("")]
    [DisallowMultipleComponent]
	public class PoolManager : Singleton<PoolManager>
	{
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] private Dictionary<int, PoolData> Collection { get; set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            this.Collection = new Dictionary<int, PoolData>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject GetLastPicked(GameObject prefab)
        {
            if (prefab == null) return null;
            return this.Collection.TryGetValue(prefab.GetInstanceID(), out PoolData data)
                ? data.LastGet
                : null;
        }
        
        public GameObject Pick(GameObject prefab, int count, float duration = -1f)
        {
            if (prefab == null) return null;
            int instanceID = prefab.GetInstanceID();

            if (!this.Collection.ContainsKey(instanceID)) this.CreatePool(prefab, count);
            return this.Collection[instanceID].Get(Vector3.zero, Quaternion.identity, duration);
        }

        public GameObject Pick(GameObject prefab, Vector3 position, Quaternion rotation, int count, float duration = -1f)
        {
            if (prefab == null) return null;
            int instanceID = prefab.GetInstanceID();

            if (!this.Collection.ContainsKey(instanceID)) this.CreatePool(prefab, count);
            return this.Collection[instanceID].Get(position, rotation, duration);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null) return;
            int instanceID = prefab.GetInstanceID();
            
            if (this.Collection.TryGetValue(instanceID, out PoolData pool))
            {
                int currentPool = pool.ReadyCount;
                int prewarmCount = count - currentPool;
                
                if (prewarmCount > 0) pool.Prewarm(prewarmCount);
            }
            else
            {
                this.CreatePool(prefab, count);   
            }
        }
        
        public void Dispose(GameObject prefab)
        {
            if (prefab == null) return;
            int instanceID = prefab.GetInstanceID();
            
            if (this.Collection.TryGetValue(instanceID, out PoolData pool))
            {
                pool.Dispose();
            }
        }

        public void DontDestroyOnLoadPool(GameObject prefab)
        {
            if (prefab == null) return;
            int instanceID = prefab.GetInstanceID();
            
            if (this.Collection.TryGetValue(instanceID, out PoolData pool))
            {
                pool.SetDontDestroyOnLoad();
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void CreatePool(GameObject prefab, int count)
        {
            int instanceID = prefab.GetInstanceID();
            this.Collection.Add(instanceID, new PoolData(prefab, count));
        }
        
        // INTERNAL CALLBACKS: --------------------------------------------------------------------

        internal void OnDisableInstance(int prefabId, PoolInstance instance)
        {
            if (this.Collection.TryGetValue(prefabId, out PoolData poolData))
            {
                poolData.OnDisableInstance(instance);
                return;
            }
            
            if (instance == null) return;
            Destroy(instance.gameObject);
        }
        
        internal void OnDestroyInstance(int prefabId, PoolInstance instance)
        {
            if (this.Collection.TryGetValue(prefabId, out PoolData poolData))
            {
                poolData.OnDestroyInstance(instance);
                return;
            }
            
            if (instance == null) return;
            Destroy(instance.gameObject);
        }
    }
}