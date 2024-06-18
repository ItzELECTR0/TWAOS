using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]

    [Description("Sets the Animation Clip value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetAnimationGlobalName : PropertyTypeSetAnimation
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueAnimClip.TYPE_ID);

        public override void Set(AnimationClip value, Args args) => this.m_Variable.Set(value, args);
        public override AnimationClip Get(Args args) => this.m_Variable.Get(args) as AnimationClip;

        public static PropertySetAnimation Create => new PropertySetAnimation(
            new SetAnimationGlobalName()
        );

        public override string String => this.m_Variable.ToString();
    }
}
