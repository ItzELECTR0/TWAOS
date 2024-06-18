using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Sprite value of a Global Name Variable")]

    [Serializable]
    public class GetSpriteGlobalName : PropertyTypeGetSprite
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueSprite.TYPE_ID);

        public override Sprite Get(Args args) => this.m_Variable.Get<Sprite>(args);

        public override string String => this.m_Variable.ToString();
    }
}