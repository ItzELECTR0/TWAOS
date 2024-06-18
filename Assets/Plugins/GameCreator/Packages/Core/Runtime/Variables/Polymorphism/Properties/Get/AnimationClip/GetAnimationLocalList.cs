using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Animation Clip value of a Local List Variable")]

    [Serializable]
    public class GetAnimationLocalList : PropertyTypeGetAnimation
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueAnimClip.TYPE_ID);

        public override AnimationClip Get(Args args) => this.m_Variable.Get<AnimationClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
