using System;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    [Serializable]
    public class AudioConfigSoundEffect : TAudioConfig
    {
        public static readonly AudioConfigSoundEffect Default = new AudioConfigSoundEffect();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private Vector2 m_Pitch = Vector2.one;
        
        [SerializeField] 
        private float m_TransitionIn = 0.0f;
        
        [SerializeField]
        private TimeMode.UpdateMode m_UpdateMode = TimeMode.UpdateMode.GameTime;
        
        [SerializeField] 
        private SpatialBlending m_SpatialBlend = SpatialBlending.None;
        
        [SerializeField]
        private PropertyGetGameObject m_Target = GetGameObjectNone.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override float Pitch => UnityEngine.Random.Range(this.m_Pitch.x, this.m_Pitch.y);

        public override float TransitionIn => this.m_TransitionIn;

        public override float SpatialBlend => m_SpatialBlend == SpatialBlending.None ? 0f : 1f;

        public override TimeMode.UpdateMode UpdateMode => this.m_UpdateMode;

        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        public override GameObject GetTrackTarget(Args args)
        {
            return this.m_Target.Get(args);
        }
        
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static AudioConfigSoundEffect Create(float volume, Vector2 pitch, float transition,
            TimeMode.UpdateMode time, SpatialBlending spatialBlending, GameObject target)
        {
            return new AudioConfigSoundEffect
            {
                m_Volume = volume,
                m_Pitch = pitch,
                m_TransitionIn = transition,
                m_SpatialBlend = spatialBlending,
                m_Target = GetGameObjectInstance.Create(target)
            };
        }
    }
}