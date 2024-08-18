using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetAudio : TPropertyGet<PropertyTypeGetAudio, AudioClip>
    {
        public PropertyGetAudio() : base(new GetAudioClip())
        { }

        public PropertyGetAudio(PropertyTypeGetAudio defaultType) : base(defaultType)
        { }

        public PropertyGetAudio(AudioClip clip) : base(new GetAudioClip(clip))
        { }
    }
}