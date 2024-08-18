using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Description("Sets the Animation Clip value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetAnimationLocalName : PropertyTypeSetAnimation
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueAnimClip.TYPE_ID);

        public override void Set(AnimationClip value, Args args) => this.m_Variable.Set(value, args);
        public override AnimationClip Get(Args args) => this.m_Variable.Get(args) as AnimationClip;

        public static PropertySetAnimation Create => new PropertySetAnimation(
            new SetAnimationLocalName()
        );

        public override string String => this.m_Variable.ToString();
    }
}
