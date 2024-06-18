using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class MaterialSoundTexture : TPolymorphicItem<MaterialSoundTexture>, IMaterialSound
    {
        private const float DEFAULT_VOLUME = 0.25f; 
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private string m_Name = "My Ground Type";
        [SerializeField] private Texture m_Texture;

        [SerializeField] private PoolField m_Impact = new PoolField();
        [SerializeField] private float m_Volume = 0.25f;
        [SerializeField] private AudioClip[] m_Variations = new AudioClip[1];

        // MEMBERS: -------------------------------------------------------------------------------

        private int variationIndex;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Name} ({this.m_Variations.Length})";
        
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

        public Texture Texture => this.m_Texture;

        public PoolField Impact => this.m_Impact;
        
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        public static MaterialSoundTexture Create()
        {
            return new MaterialSoundTexture
            {
                m_Variations = new AudioClip[1],
                m_Volume = DEFAULT_VOLUME
            };
        }
    }
}