using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Hotspot Activate")]
    [Category("Logic/On Hotspot Activate")]
    [Description("Executed when its associated Hotspot is activated")]

    [Image(typeof(IconHotspot), ColorTheme.Type.Green)]
    
    [Keywords("Spot")]

    [Serializable]
    public class EventOnHotspotActivate : Event
    {
        [SerializeField] private PropertyGetGameObject m_Hotspot = GetGameObjectSelf.Create();

        [NonSerialized] private Hotspot m_Cache;
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            this.m_Cache = this.m_Hotspot.Get<Hotspot>(this.Self);
            if (this.m_Cache == null) return;
            
            this.m_Cache.EventOnActivate -= this.OnActivate;
            this.m_Cache.EventOnActivate += this.OnActivate;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            
            if (this.m_Cache == null) return;
            this.m_Cache.EventOnActivate -= this.OnActivate;
        }

        private void OnActivate()
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}