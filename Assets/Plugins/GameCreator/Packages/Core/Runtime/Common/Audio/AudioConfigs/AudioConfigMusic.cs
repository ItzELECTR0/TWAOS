using System;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    [Serializable]
    public class AudioConfigMusic : TAudioConfig
    {
        public static readonly AudioConfigMusic Default = new AudioConfigMusic();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private float m_TransitionIn = 0.0f;
        
        [SerializeField]
        private TimeMode.UpdateMode m_UpdateMode = TimeMode.UpdateMode.GameTime;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override float TransitionIn => this.m_TransitionIn;

        public override float SpatialBlend => 0f;

        public override TimeMode.UpdateMode UpdateMode => this.m_UpdateMode;

        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static AudioConfigMusic Create(float volume, float transition)
        {
            return new AudioConfigMusic
            {
                m_Volume = volume,
                m_TransitionIn = transition,
            };
        }
    }
}