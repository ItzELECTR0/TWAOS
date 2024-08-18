using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PoolField
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private GameObject m_Prefab;

        [SerializeField] private EnablerInt m_UsePooling = new EnablerInt(false, 5);
        [SerializeField] private EnablerFloat m_Duration = new EnablerFloat(false, 10f);
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject Create(Vector3 position, Quaternion rotation, Transform parent)
        {
            if (this.m_Prefab == null) return null;
            GameObject instance;

            switch (this.m_UsePooling.IsEnabled)
            {
                case true:
                    instance = PoolManager.Instance.Pick(
                        this.m_Prefab, position, rotation, 
                        this.m_UsePooling.Value, this.m_Duration.IsEnabled ? this.m_Duration.Value : -1
                    );
                    
                    if (parent != null) instance.transform.SetParent(parent);
                    break;

                case false:
                    instance = UnityEngine.Object.Instantiate(this.m_Prefab, position, rotation, parent);
                    break;
            }

            return instance;
        }
    }
}