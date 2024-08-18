using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the Audio Clip value of a Global List Variable")]

    [Serializable]
    public class GetAudioClipGlobalList : PropertyTypeGetAudio
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueAudioClip.TYPE_ID);

        public override AudioClip Get(Args args) => this.m_Variable.Get<AudioClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
