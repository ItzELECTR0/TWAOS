using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Texture value of a Global Name Variable")]

    [Serializable]
    public class GetTextureGlobalName : PropertyTypeGetTexture
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueTexture.TYPE_ID);

        public override Texture Get(Args args) => this.m_Variable.Get<Texture>(args);

        public override string String => this.m_Variable.ToString();
    }
}