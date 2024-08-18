using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Cameras
{
    [Title("Follow Track")]
    [Category("Follow Track")]
    [Image(typeof(IconShotTrack), ColorTheme.Type.Blue)]
    
    [Description("Follows the target from along a pre-defined path segment")]
    
    [Serializable]
    public class ShotTypeTrack : TShotTypeLook
    {
        [SerializeField] private ShotSystemTrack m_ShotSystemTrack;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public ShotSystemTrack Track => m_ShotSystemTrack;
        
        public override Vector3 Position { get; set; }
        public override Quaternion Rotation { get; set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ShotTypeTrack()
        {
            this.m_ShotSystemTrack = new ShotSystemTrack();
            
            this.m_ShotSystems.Add(this.m_ShotSystemLook.Id, this.m_ShotSystemLook);
            this.m_ShotSystems.Add(this.m_ShotSystemTrack.Id, this.m_ShotSystemTrack);
        }
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();
            this.m_ShotSystemTrack.OnUpdate(this);
        }
    }
}