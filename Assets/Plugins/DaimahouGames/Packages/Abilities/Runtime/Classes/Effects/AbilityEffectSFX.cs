using System;
using System.Collections.Generic;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.Audio;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(0, 1, 1)]
    
    [Title("Sound Effect")]
    [Category("Sound Effect")]
    
    [Description("Play a sound Effect")]

    [Keywords("Sound", "SFX")]

    [Image(typeof(IconMusicNote), ColorTheme.Type.Blue)]
    [Serializable]
    public class AbilityEffectSFX : AbilityEffect
    {
        public struct AudioBuffer
        {
            private Dictionary<AudioClip, int> m_Buffer;
            private Dictionary<AudioClip, int> Buffer => m_Buffer ??= new Dictionary<AudioClip, int>();

            public void Add(AudioClip clip)
            {
                Buffer.TryGetValue(clip, out var count);
                Buffer[clip] = count + 1;
            }

            public int GetCount(AudioClip clip)
            {
                Buffer.TryGetValue(clip, out var count);
                return count;
            }
        }
        
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private int m_MaxOccurrences = 2;
        [SerializeField] private AudioClip m_AudioClip;
        [SerializeField] private AudioConfigSoundEffect m_AudioConfig = new();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => string.Format(
            "Play [{0}]",
            m_AudioClip ? $"{m_AudioClip.name}]" : "NO CLIP"
        ); 
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        protected override void Apply_Internal(ExtendedArgs args)
        {
            var buffer = args.Get<AudioBuffer>();

            if (buffer.GetCount(m_AudioClip) > m_MaxOccurrences) return;
            buffer.Add(m_AudioClip);
            
            AudioManager.Instance.SoundEffect.Play(
                m_AudioClip, 
                m_AudioConfig,
                Args.EMPTY
            );
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}