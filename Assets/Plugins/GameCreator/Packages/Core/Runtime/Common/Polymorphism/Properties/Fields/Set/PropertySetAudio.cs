using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetAudio : TPropertySet<PropertyTypeSetAudio, AudioClip>
    {
        public PropertySetAudio() : base(new SetAudioNone())
        { }

        public PropertySetAudio(PropertyTypeSetAudio defaultType) : base(defaultType)
        { }
    }
}