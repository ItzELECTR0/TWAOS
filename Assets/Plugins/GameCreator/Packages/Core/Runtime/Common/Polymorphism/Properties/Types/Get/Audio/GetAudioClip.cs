using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Audio Clip")]
    [Category("Audio Clip")]
    
    [Image(typeof(IconQuaver), ColorTheme.Type.Yellow)]
    [Description("An Audio Clip asset")]

    [Serializable] [HideLabelsInEditor]
    public class GetAudioClip : PropertyTypeGetAudio
    {
        [SerializeField] protected AudioClip m_Value;

        public override AudioClip Get(Args args) => this.m_Value;
        public override AudioClip Get(GameObject gameObject) => this.m_Value;

        public GetAudioClip() : base()
        { }

        public GetAudioClip(AudioClip value = null) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetAudio Create => new PropertyGetAudio(
            new GetAudioClip()
        );

        public override string String => this.m_Value != null 
            ? this.m_Value.name 
            : "(none)";

        public override AudioClip EditorValue => this.m_Value;
    }
}