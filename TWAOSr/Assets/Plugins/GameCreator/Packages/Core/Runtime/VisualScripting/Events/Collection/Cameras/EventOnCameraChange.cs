using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Camera Change")]
    [Category("Cameras/On Camera Change")]
    [Description("Executed when the Camera changes to another Camera Shot")]

    [Image(typeof(IconCamera), ColorTheme.Type.Green)]
    
    [Keywords("Shot", "Switch", "Cut")]

    [Serializable]
    public class EventOnCameraChange : Event
    {
        private enum ChangeMode
        {
            AnyChange,
            OnCut,
            OnTransition
        }
        
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;
        [SerializeField] private ChangeMode m_When = ChangeMode.AnyChange;

        [NonSerialized] private TCamera m_Cache;
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            this.m_Cache = this.m_Camera.Get<TCamera>(this.Self);
            if (this.m_Cache == null) return;
            
            this.m_Cache.EventCut -= this.OnChangeCut;
            this.m_Cache.EventCut += this.OnChangeCut;
            
            this.m_Cache.EventTransition -= this.OnChangeTransition;
            this.m_Cache.EventTransition += this.OnChangeTransition;
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            
            if (this.m_Cache == null) return;
            
            this.m_Cache.EventCut -= this.OnChangeCut;
            this.m_Cache.EventTransition -= this.OnChangeTransition;
        }

        private void OnChangeCut(ShotCamera shotCamera)
        {
            if (this.m_When == ChangeMode.OnTransition) return;
            _ = this.m_Trigger.Execute(this.Self);
        }
        
        private void OnChangeTransition(ShotCamera shotCamera, float duration, Easing.Type ease)
        {
            if (this.m_When == ChangeMode.OnCut) return;
            _ = this.m_Trigger.Execute(this.Self);
        }
    }
}