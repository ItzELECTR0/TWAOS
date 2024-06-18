using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class UseRaycast
    {
        private const int BUFFER_SIZE = 20;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private bool m_UseRaycast;
        [SerializeField] private LayerMask m_LayerMask;
        
        // MEMBERS: -------------------------------------------------------------------------------

        private RaycastHit[] m_Buffer;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public UseRaycast()
        {
            this.m_UseRaycast = false;
            this.m_LayerMask = Physics.DefaultRaycastLayers;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool HasObstacle(Transform origin, Transform target)
        {
            if (!this.m_UseRaycast) return false;
            this.m_Buffer ??= new RaycastHit[BUFFER_SIZE];

            Vector3 direction = target.position - origin.position;
            float maxDistance = Vector3.Distance(origin.position, target.position);

            int hits = Physics.RaycastNonAlloc(
                origin.position, direction, 
                this.m_Buffer, maxDistance, this.m_LayerMask
            );

            for (int i = 0; i < hits; ++i)
            {
                Transform transform = this.m_Buffer[i].transform;
                if (transform.IsChildOf(origin)) continue;
                if (transform.IsChildOf(target)) continue;

                return true;
            }

            return false;
        }
    }
}