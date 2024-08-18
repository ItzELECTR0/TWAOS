using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Animation Clip value of a Global Name Variable")]

    [Serializable]
    public class GetAnimationGlobalName : PropertyTypeGetAnimation
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueAnimClip.TYPE_ID);

        public override AnimationClip Get(Args args) => this.m_Variable.Get<AnimationClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
