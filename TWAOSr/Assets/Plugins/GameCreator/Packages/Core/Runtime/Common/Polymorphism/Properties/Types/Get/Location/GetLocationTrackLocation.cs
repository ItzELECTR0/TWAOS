using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Track Target")]
    [Category("Tracking/Track Target")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("A translation towards a specific target and an optional rotation")]
    
    [Keywords("Track", "Follow", "Towards", "Away")]
    
    [Serializable]
    public class GetLocationTrackLocation : TGetLocationTrackLocation
    {
        [SerializeField] private PropertyGetGameObject m_From = GetGameObjectSelf.Create();
        [SerializeField] private PropertyGetGameObject m_To = GetGameObjectTarget.Create();
        
        public static PropertyGetLocation Create()
        {
            GetLocationTrackLocation instance = new GetLocationTrackLocation();
            return new PropertyGetLocation(instance);
        }
        
        public override string String => $"{this.m_From} Track {this.m_To}";
        
        protected override GameObject GetFrom(Args args)
        {
            return this.m_From.Get(args);
        }

        protected override GameObject GetTo(Args args)
        {
            return this.m_To.Get(args);
        }
    }
}