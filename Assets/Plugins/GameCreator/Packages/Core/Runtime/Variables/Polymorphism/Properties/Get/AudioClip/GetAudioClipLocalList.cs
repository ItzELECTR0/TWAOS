using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Audio Clip value of a Local List Variable")]

    [Serializable]
    public class GetAudioClipLocalList : PropertyTypeGetAudio
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueAudioClip.TYPE_ID);

        public override AudioClip Get(Args args) => this.m_Variable.Get<AudioClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
