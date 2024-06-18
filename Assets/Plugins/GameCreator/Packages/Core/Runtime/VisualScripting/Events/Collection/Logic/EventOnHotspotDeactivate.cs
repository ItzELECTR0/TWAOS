using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Hotspot Deactivate")]
    [Category("Logic/On Hotspot Deactivate")]
    [Description("Executed when its associated Hotspot is deactivated")]

    [Image(typeof(IconHotspot), ColorTheme.Type.Red)]
    
    [Keywords("Spot")]

    [Serializable]
    public class EventOnHotspotDeactivate : Event
    {
        [SerializeField] private PropertyGetGameObject m_Hotspot = GetGameObjectSelf.Create();

        [NonSerialized] private Hotspot m_Cache;
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            this.m_Cache = this.m_Hotspot.Get<Hotspot>(this.Self);
            if (this.m_Cache == null) return;
            
            this.m_Cache.EventOnDeactivate -= this.OnDeactivate;
            this.m_Cache.EventOnDeactivate += this.OnDeactivate;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            
            if (this.m_Cache == null) return;
            this.m_Cache.EventOnDeactivate -= this.OnDeactivate;
        }

        private void OnDeactivate()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}