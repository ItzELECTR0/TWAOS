using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null Audio Clip ")]

    [Serializable] [HideLabelsInEditor]
    public class GetAudioNone : PropertyTypeGetAudio
    {
        public override AudioClip Get(Args args) => null;
        public override AudioClip Get(GameObject gameObject) => null;

        public static PropertyGetAudio Create => new PropertyGetAudio(
            new GetAudioNone()
        );

        public override string String => "None";
    }
}