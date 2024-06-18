using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the Texture value of a Global List Variable")]

    [Serializable]
    public class GetTextureGlobalList : PropertyTypeGetTexture
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueTexture.TYPE_ID);

        public override Texture Get(Args args) => this.m_Variable.Get<Texture>(args);

        public override string String => this.m_Variable.ToString();
    }
}