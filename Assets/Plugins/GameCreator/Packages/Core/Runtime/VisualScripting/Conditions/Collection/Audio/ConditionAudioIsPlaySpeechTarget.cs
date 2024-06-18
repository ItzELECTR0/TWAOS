using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Speech Target Playing")]
    [Description("Returns true if the given target game object is playing any audio clip")]

    [Category("Audio/Is Speech Target Playing")]
    
    [Parameter("Target", "The game object target")]

    [Keywords("SFX", "Speech", "Audio", "Running")]
    [Image(typeof(IconFace), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class ConditionAudioIsPlaySpeechTarget : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_Target = GetGameObjectCharactersInstance.Create;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is {this.m_Target} playing Speech";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject target = this.m_Target.Get(args);
            return target != null && AudioManager.Instance.Speech.IsPlaying(target);
        }
    }
}
