using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the Animation Clip value of a Global List Variable")]

    [Serializable]
    public class GetAnimationGlobalList : PropertyTypeGetAnimation
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueAnimClip.TYPE_ID);

        public override AnimationClip Get(Args args) => this.m_Variable.Get<AnimationClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
