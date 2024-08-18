using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Audio Clip value of a Global Name Variable")]

    [Serializable]
    public class GetAudioClipGlobalName : PropertyTypeGetAudio
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueAudioClip.TYPE_ID);

        public override AudioClip Get(Args args) => this.m_Variable.Get<AudioClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
