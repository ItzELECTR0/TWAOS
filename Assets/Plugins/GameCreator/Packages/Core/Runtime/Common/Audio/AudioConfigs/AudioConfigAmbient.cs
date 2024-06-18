using System;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    [Serializable]
    public class AudioConfigAmbient : TAudioConfig
    {
        public static readonly AudioConfigAmbient Default = new AudioConfigAmbient();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private float m_TransitionIn = 0.0f;
        
        [SerializeField]
        private TimeMode.UpdateMode m_UpdateMode = TimeMode.UpdateMode.GameTime;
        
        [SerializeField]
        private SpatialBlending m_SpatialBlend = SpatialBlending.None;

        [SerializeField]
        private PropertyGetGameObject m_Target = GetGameObjectNone.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override float TransitionIn => this.m_TransitionIn;

        public override float SpatialBlend => m_SpatialBlend == SpatialBlending.None ? 0f : 1f;

        public override TimeMode.UpdateMode UpdateMode => this.m_UpdateMode;

        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        public override GameObject GetTrackTarget(Args args)
        {
            return this.m_Target.Get(args);
        }
        
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static AudioConfigAmbient Create(float volume, float transition, 
            SpatialBlending spatialBlending, GameObject target = null)
        {
            return new AudioConfigAmbient
            {
                m_Volume = volume,
                m_TransitionIn = transition,
                m_SpatialBlend = spatialBlending,
                m_Target = GetGameObjectInstance.Create(target)
            };
        }
    }
}