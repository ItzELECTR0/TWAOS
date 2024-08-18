using System;
using UnityEngine;
using GameCreator.Runtime.Common;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Instantiate Prefab")]
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    
    [Category("Game Objects/Instantiate Prefab")]
    [Description(
        "Creates or Activates a prefab game object when the Hotspot is enabled and " +
        "deactivates it when the Hotspot is disabled"
    )]

    [Serializable]
    public class SpotObjectsInstantiatePrefab : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField]
        protected PropertyGetGameObject m_Prefab = GetGameObjectInstance.Create();

        [SerializeField]
        protected PropertyGetDirection m_Offset = GetDirectionVector3Zero.Create();

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private GameObject m_Hint;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Instantiate {this.m_Prefab}";

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnUpdate(Hotspot hotspot)
        {
            base.OnUpdate(hotspot);

            GameObject instance = this.RequireInstance(hotspot);
            if (instance == null) return;

            Vector3 offset = this.m_Offset.Get(hotspot.Args);

            instance.transform.SetPositionAndRotation(
                hotspot.transform.position + offset,
                hotspot.transform.rotation
            );

            bool isActive = this.EnableInstance(hotspot);
            instance.SetActive(isActive);
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);
            if (this.m_Hint != null) this.m_Hint.SetActive(false);
        }

        public override void OnDestroy(Hotspot hotspot)
        {
            base.OnDestroy(hotspot);
            
            if (this.m_Hint != null)
            {
                UnityEngine.Object.Destroy(this.m_Hint);
            }
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual bool EnableInstance(Hotspot hotspot)
        {
            return hotspot.IsActive;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private GameObject RequireInstance(Hotspot hotspot)
        {
            if (this.m_Hint == null)
            {
                GameObject prefab = this.m_Prefab.Get(hotspot.Args);
                if (prefab == null) return null;
                
                this.m_Hint = UnityEngine.Object.Instantiate(
                    prefab,
                    hotspot.transform.position,
                    hotspot.transform.rotation
                );

                this.m_Hint.hideFlags = HideFlags.HideAndDontSave;
            }

            return this.m_Hint;
        }
    }
}