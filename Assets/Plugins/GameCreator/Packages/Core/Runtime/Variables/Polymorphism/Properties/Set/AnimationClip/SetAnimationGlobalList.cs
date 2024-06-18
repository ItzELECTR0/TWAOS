using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]

    [Description("Sets the Animation Clip value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetAnimationGlobalList : PropertyTypeSetAnimation
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueAnimClip.TYPE_ID);

        public override void Set(AnimationClip value, Args args) => this.m_Variable.Set(value, args);
        public override AnimationClip Get(Args args) => this.m_Variable.Get(args) as AnimationClip;

        public static PropertySetAnimation Create => new PropertySetAnimation(
            new SetAnimationGlobalList()
        );

        public override string String => this.m_Variable.ToString();
    }
}
