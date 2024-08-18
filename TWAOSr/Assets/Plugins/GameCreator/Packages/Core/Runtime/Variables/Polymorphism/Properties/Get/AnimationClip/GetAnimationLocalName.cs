using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Animation Clip value of a Local Name Variable")]

    [Serializable]
    public class GetAnimationLocalName : PropertyTypeGetAnimation
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueAnimClip.TYPE_ID);

        public override AnimationClip Get(Args args) => this.m_Variable.Get<AnimationClip>(args);

        public override string String => this.m_Variable.ToString();
    }
}
