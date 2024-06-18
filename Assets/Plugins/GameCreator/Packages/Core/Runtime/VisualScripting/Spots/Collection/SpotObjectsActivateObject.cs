using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Activate Object")]
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]
    
    [Category("Game Objects/Activate Object")]
    [Description(
        "Activates a game object scene instance when the Hotspot is enabled and " +
        "deactivates it when the Hotspot is disabled"
    )]

    [Serializable]
    public class SpotObjectsActivateObject : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField]
        protected PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Activate {this.m_GameObject}";

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnUpdate(Hotspot hotspot)
        {
            base.OnUpdate(hotspot);
            
            GameObject gameObject = this.m_GameObject.Get(hotspot.Args);
            if (gameObject == null) return;

            bool isActive = this.EnableInstance(hotspot);
            gameObject.SetActive(isActive);
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);
            
            GameObject gameObject = this.m_GameObject.Get(hotspot.Args);
            if (gameObject == null) return;
            
            gameObject.SetActive(false);
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual bool EnableInstance(Hotspot hotspot)
        {
            return hotspot.IsActive;
        }
    }
}