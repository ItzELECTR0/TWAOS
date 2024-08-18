using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    internal class PoolData
    {
        private const string CONTAINER_NAME = "{0} (pool)";

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly GameObject m_Prefab;
        [NonSerialized] private readonly Transform m_Container;

        [NonSerialized] private readonly List<PoolInstance> m_ReadyInstances;
        [NonSerialized] private readonly Dictionary<int, PoolInstance> m_RunningInstances;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int ReadyCount => this.m_ReadyInstances.Count;
        
        [field: NonSerialized] public GameObject LastGet { get; private set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public PoolData(GameObject prefab, int count)
        {
            this.m_Prefab = prefab;
            this.m_Container = new GameObject(string.Format(CONTAINER_NAME, prefab.name)).transform;
            
            this.m_Container.SetParent(PoolManager.Instance.transform);
            this.m_Container.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            
            this.m_ReadyInstances = new List<PoolInstance>(count);
            this.m_RunningInstances = new Dictionary<int, PoolInstance>();

            this.Prewarm(count);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public GameObject Get(Vector3 position, Quaternion rotation, float duration)
        {
            if (this.m_ReadyInstances.Count == 0) this.Prewarm(1);

            PoolInstance instance = this.m_ReadyInstances[0];
            this.m_ReadyInstances.RemoveAt(0);
            
            int instanceId = instance.GetInstanceID();
            this.m_RunningInstances[instanceId] = instance;
            
            instance.transform.SetPositionAndRotation(position, rotation);
            
            instance.enabled = true;
            instance.gameObject.SetActive(true);
                
            if (duration > 0f) instance.SetDuration(duration);

            this.LastGet = instance.gameObject;
            return this.LastGet;
        }
        
        public void Prewarm(int count)
        {
            int prefabId = this.m_Prefab.GetInstanceID();
            
            for (int i = 0; i < count; ++i)
            {
                PoolInstance instance = this.CreateInstance();
                instance.OnCreate(prefabId);
                
                this.m_ReadyInstances.Add(instance);
            }
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(this.m_Container);
        }
        
        public void SetDontDestroyOnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(this.m_Container);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private PoolInstance CreateInstance()
        {
            bool prevState = this.m_Prefab.activeSelf;
            this.m_Prefab.SetActive(false);

            GameObject instance = UnityEngine.Object.Instantiate(this.m_Prefab, this.m_Container);
            
            PoolInstance poolInstance = instance.GetComponent<PoolInstance>();
            if (poolInstance == null) poolInstance = instance.AddComponent<PoolInstance>();

            this.m_Prefab.gameObject.SetActive(prevState);
            return poolInstance;
        }

        // INTERNAL CALLBACKS: --------------------------------------------------------------------
        
        internal void OnDisableInstance(PoolInstance instance)
        {
            int instanceId = instance.GetInstanceID();
            if (this.m_RunningInstances.Remove(instanceId))
            {
                this.m_ReadyInstances.Add(instance);
            }
        }
        
        internal void OnDestroyInstance(PoolInstance instance)
        {
            int instanceId = instance.GetInstanceID();
            
            this.m_RunningInstances.Remove(instanceId);
            this.m_ReadyInstances.Remove(instance);
        }
    }
}