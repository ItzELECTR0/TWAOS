using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class SpotList : TPolymorphicList<Spot>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeReference]
        private Spot[] m_Spots;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Spots.Length;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SpotList()
        {
            this.m_Spots = new Spot[] { new SpotObjectsInstantiatePrefab() };
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void OnAwake(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnAwake(hotspot);
            }
        }
        
        public void OnStart(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnStart(hotspot);
            }
        }
        
        public void OnUpdate(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnUpdate(hotspot);
            }
        }

        public void OnEnable(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnEnable(hotspot);
            }
        }

        public void OnDisable(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnDisable(hotspot);
            }
        }
        
        public void OnPointerEnter(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnPointerEnter(hotspot);
            }
        }
        
        public void OnPointerExit(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnPointerExit(hotspot);
            }
        }
        
        public void OnDestroy(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnDestroy(hotspot);
            }
        }

        public void OnGizmos(Hotspot hotspot)
        {
            foreach (Spot spot in this.m_Spots)
            {
                if (spot is not { IsEnabled: true }) continue;
                spot.OnGizmos(hotspot);
            }
        }
    }
}