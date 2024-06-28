using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.Audio;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Title("Sound Effect")]
    [Category("Sound Effect")]
    
    [Image(typeof(IconMusicNote), ColorTheme.Type.Blue)]
    [Serializable]
    public class AnimNotifySFX : TNotify
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private AudioClip m_AudioClip;
        [SerializeField] private AudioConfigSoundEffect m_AudioConfig = new();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public override string SubTitle => "Play [" + ( m_AudioClip ? $"{m_AudioClip.name}]" : "NO CLIP]");
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override Task Trigger(Character character)
        {
            return AudioManager.Instance.SoundEffect.Play(
                m_AudioClip, 
                m_AudioConfig,
                Args.EMPTY
            );
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}