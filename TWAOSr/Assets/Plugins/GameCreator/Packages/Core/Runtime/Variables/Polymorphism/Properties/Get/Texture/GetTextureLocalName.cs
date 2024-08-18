using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Texture value of a Local Name Variable")]
    
    [Serializable]
    public class GetTextureLocalName : PropertyTypeGetTexture
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueTexture.TYPE_ID);

        public override Texture Get(Args args) => this.m_Variable.Get<Texture>(args);

        public override string String => this.m_Variable.ToString();
    }
}