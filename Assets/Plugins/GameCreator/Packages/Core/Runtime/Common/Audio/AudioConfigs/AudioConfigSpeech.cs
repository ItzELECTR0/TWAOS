using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    [Serializable]
    public class AudioConfigSpeech : TAudioConfig
    {
        public static readonly AudioConfigSpeech Default = new AudioConfigSpeech();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private TimeMode.UpdateMode m_UpdateMode = TimeMode.UpdateMode.GameTime;
        
        [SerializeField] 
        private SpatialBlending m_SpatialBlend = SpatialBlending.Spatial;

        [SerializeField]
        private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override float SpatialBlend => m_SpatialBlend == SpatialBlending.None ? 0f : 1f;

        public override TimeMode.UpdateMode UpdateMode => this.m_UpdateMode;

        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        public override GameObject GetTrackTarget(Args args)
        {
            GameObject target = this.m_Target.Get(args);
            return GetSpeechSource(target);
        }
        
        // STATIC METHODS: ------------------------------------------------------------------------

        public static GameObject GetSpeechSource(GameObject target)
        {
            if (target == null) return null;
            
            Animator animator = target.GetComponentInChildren<Animator>();
            if (animator != null && animator.isHuman)
            {
                target = animator.GetBoneTransform(HumanBodyBones.Head).gameObject;
            }

            return target;
        }
        
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static AudioConfigSpeech Create(float volume, SpatialBlending spatialBlending, 
            GameObject target)
        {
            return new AudioConfigSpeech
            {
                m_Volume = volume,
                m_SpatialBlend = spatialBlending,
                m_Target = GetGameObjectInstance.Create(target)
            };
        }
    }
}