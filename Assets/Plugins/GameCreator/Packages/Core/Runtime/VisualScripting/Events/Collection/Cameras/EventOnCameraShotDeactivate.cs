using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Change from Shot")]
    [Category("Cameras/On Change from Shot")]
    [Description("Executed when the Camera Shot is deactivated")]

    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    
    [Keywords("Shot", "Switch", "Cut")]

    [Serializable]
    public class EventOnCameraShotDeactivate : Event
    {
        [SerializeField] private PropertyGetGameObject m_CameraShot = GetGameObjectInstance.Create();

        [NonSerialized] private ShotCamera m_Cache;
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);

            this.m_Cache = this.m_CameraShot.Get<ShotCamera>(this.Self);
            if (this.m_Cache == null) return;
            
            this.m_Cache.EventChangeFrom -= this.OnChange;
            this.m_Cache.EventChangeFrom += this.OnChange;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            
            if (this.m_Cache == null) return;
            this.m_Cache.EventChangeFrom -= this.OnChange;
        }

        private void OnChange(TCamera camera)
        {
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}