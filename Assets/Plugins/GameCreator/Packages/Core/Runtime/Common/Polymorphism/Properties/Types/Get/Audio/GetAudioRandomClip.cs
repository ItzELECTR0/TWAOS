using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random Audio Clip")]
    [Category("Random/Random Audio Clip")]
    
    [Image(typeof(IconDice), ColorTheme.Type.Yellow)]
    [Description("A random Audio Clip asset from a list")]

    [Serializable] [HideLabelsInEditor]
    public class GetAudioRandomClip : PropertyTypeGetAudio
    {
        [SerializeField] protected AudioClip[] m_Values = Array.Empty<AudioClip>();

        public override AudioClip Get(Args args)
        {
            if ((this.m_Values?.Length ?? 0) == 0) return null;

            int index = UnityEngine.Random.Range(0, this.m_Values.Length);
            return this.m_Values[index];
        }

        public static PropertyGetAudio Create => new PropertyGetAudio(
            new GetAudioRandomClip()
        );

        public override string String => "Random Clip";
    }
}