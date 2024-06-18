using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Audio Clip value of a Local Name Variable")]

    [Serializable]
    public class GetAudioClipLocalName : PropertyTypeGetAudio
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueAudioClip.TYPE_ID);

        public override AudioClip Get(Args args) => this.m_Variable.Get<AudioClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
