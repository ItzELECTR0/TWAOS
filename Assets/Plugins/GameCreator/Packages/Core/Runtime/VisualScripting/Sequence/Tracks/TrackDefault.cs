using System;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class TrackDefault : Track
    {
        [SerializeReference] private ClipDefault[] m_Clips = Array.Empty<ClipDefault>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int TrackOrder => 0;
        public override IClip[] Clips => this.m_Clips;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public TrackDefault()
        { }

        public TrackDefault(ClipDefault[] clips)
        {
            this.m_Clips = clips;
        }
    }
}