using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetAudioNone : PropertyTypeSetAudio
    {
        public override void Set(AudioClip value, Args args)
        { }
        
        public override void Set(AudioClip value, GameObject gameObject)
        { }

        public static PropertySetAudio Create => new PropertySetAudio(
            new SetAudioNone()
        );

        public override string String => "(none)";
    }
}