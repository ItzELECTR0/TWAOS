using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]

    [Description("Sets the Animation Clip value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetAnimationLocalList : PropertyTypeSetAnimation
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueAnimClip.TYPE_ID);

        public override void Set(AnimationClip value, Args args) => this.m_Variable.Set(value, args);
        public override AnimationClip Get(Args args) => this.m_Variable.Get(args) as AnimationClip;

        public static PropertySetAnimation Create => new PropertySetAnimation(
            new SetAnimationLocalList()
        );

        public override string String => this.m_Variable.ToString();
    }
}
