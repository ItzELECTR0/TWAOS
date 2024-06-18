using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class MaterialSoundDefault : IMaterialSound
    {
        private const float DEFAULT_VOLUME = 0.25f; 
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_Volume = 0.25f;
        [SerializeField] private AudioClip[] m_Variations = Array.Empty<AudioClip>();
        [SerializeField] private PoolField m_Impact = new PoolField();
        
        // MEMBERS: -------------------------------------------------------------------------------

        private int variationIndex;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Volume => this.m_Volume;

        public AudioClip Audio
        {
            get
            {
                if (this.m_Variations.Length == 0) return null;
                
                int index = UnityEngine.Random.Range(0, this.m_Variations.Length - 1);
                index += this.m_Variations.Length > 1 && index == this.variationIndex ? 1 : 0;

                this.variationIndex = index;
                return this.m_Variations[index];
            }
        }

        public PoolField Impact => this.m_Impact;
        
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static MaterialSoundDefault Create()
        {
            return new MaterialSoundDefault
            {
                m_Variations = new AudioClip[1],
                m_Volume = DEFAULT_VOLUME
            };
        }
    }
}