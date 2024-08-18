using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class MaterialSoundsData : TPolymorphicList<MaterialSoundTexture>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] 
        private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        [SerializeReference]
        private MaterialSoundTexture[] m_MaterialSounds = Array.Empty<MaterialSoundTexture>();
        
        [SerializeField] 
        private MaterialSoundDefault m_DefaultSounds = new MaterialSoundDefault();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_MaterialSounds.Length;
        
        public LayerMask LayerMask => m_LayerMask;

        public MaterialSoundTexture[] MaterialSounds => m_MaterialSounds;

        public MaterialSoundDefault DefaultSounds => m_DefaultSounds;
    }
}