using System;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    [Serializable]
    public class AudioConfigSoundUI : TAudioConfig
    {
        public static readonly AudioConfigSoundUI Default = new AudioConfigSoundUI();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private Vector2 m_Pitch = new Vector2(0.95f, 1.05f);

        [SerializeField]
        private TimeMode.UpdateMode m_UpdateMode = TimeMode.UpdateMode.UnscaledTime;
        
        [SerializeField] 
        private SpatialBlending m_SpatialBlend = SpatialBlending.None;
        
        [SerializeField]
        private PropertyGetGameObject m_Target = GetGameObjectNone.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override float Pitch => UnityEngine.Random.Range(this.m_Pitch.x, this.m_Pitch.y);

        public override float SpatialBlend => m_SpatialBlend == SpatialBlending.None ? 0f : 1f;

        public override TimeMode.UpdateMode UpdateMode => this.m_UpdateMode;

        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        public override GameObject GetTrackTarget(Args args)
        {
            return this.m_Target.Get(args);
        }
        
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static AudioConfigSoundUI Create(float volume, Vector2 pitch)
        {
            return new AudioConfigSoundUI
            {
                m_Volume = volume,
                m_Pitch = pitch,
                m_SpatialBlend = SpatialBlending.None,
                m_Target = GetGameObjectInstance.Create((GameObject) null)
            };
        }
    }
}