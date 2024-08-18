using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Animation Clip")]
    [Category("Animation Clip")]

    [Image(typeof(IconAnimationClip), ColorTheme.Type.Teal)]
    [Description("An Animation Clip asset")]

    [Serializable] [HideLabelsInEditor]
    public class GetAnimationInstance : PropertyTypeGetAnimation
    {
        [SerializeField] protected AnimationClip m_Value;

        public override AnimationClip Get(Args args) => this.m_Value;
        public override AnimationClip Get(GameObject gameObject) => this.m_Value;

        public GetAnimationInstance() : base()
        { }

        public GetAnimationInstance(AnimationClip value = null) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetAnimation Create => new PropertyGetAnimation(
            new GetAnimationInstance()
        );

        public override string String => this.m_Value != null
            ? this.m_Value.name
            : "(none)";

        public override AnimationClip EditorValue => this.m_Value;
    }
}
